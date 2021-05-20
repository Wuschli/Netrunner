﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;
using MongoDB.Driver;
using Netrunner.Shared.Auth;
using Netrunner.Shared.Internal;
using Netrunner.Shared.Internal.Auth;
using Netrunner.Shared.Services;
using WampSharp.V2.Core.Contracts;

namespace Netrunner.Auth
{
    internal class Authenticator : IAuthService, IAuthenticator
    {
        private const int HashLength = 128; // in bytes

        private readonly JwtAuthManager _jwtAuthManager;
        private readonly IMongoCollection<ApplicationUser> _users;
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public Authenticator(Config config)
        {
            _jwtAuthManager = new JwtAuthManager(config);
            var mongoClient = new MongoClient(config.Database.ConnectionString);
            var database = mongoClient.GetDatabase(config.Database.DatabaseName);
            _users = database.GetCollection<ApplicationUser>(config.Database.UsersCollectionName);
        }

        public async Task<AuthenticationResponse> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser(request.Username);
            var result = await CreateUserAsync(user, request.Password);
            if (result.Succeeded)
            {
                return await AuthenticateAsync(user);
            }

            if (result.Errors != null)
                return new AuthenticationResponse
                {
                    Error = string.Join(",", result.Errors.Select(error => error.Description)),
                    Successful = false
                };

            return new AuthenticationResponse
            {
                Error = "Error!",
                Successful = false
            };
        }

        public async Task<AuthenticationResponse> Login(LoginRequest request)
        {
            var user = await PasswordSignInAsync(request.Username, request.Password);
            if (user != null)
            {
                return await AuthenticateAsync(user);
            }

            return new AuthenticationResponse
            {
                Error = "Bad Credentials",
                Successful = false
            };
        }

        public Task<AuthenticationResult> Authenticate(string realm, string authId, AuthenticationDetails details)
        {
            throw new WampException("netrunner.auth.invalid_ticket");
        }

        private async Task<OperationResult> CreateUserAsync(ApplicationUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                return OperationResult.Create(false, "Username must not be empty");

            if (await FindUserByName(user.Username) != null)
                return OperationResult.Create(false, "User with this username already exists");

            var saltBytes = new byte[HashLength];
            Rng.GetBytes(saltBytes);

            var hash = await HashPassword(password, saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            user.PasswordHash = hash;
            user.Salt = salt;
            user.Roles = new List<string>();

            await _users.InsertOneAsync(user);
            return OperationResult.Create(true);
        }

        private async Task<ApplicationUser?> PasswordSignInAsync(string username, string password)
        {
            var user = await FindUserByName(username);
            if (user == null)
                return null;

            var saltBytes = Convert.FromBase64String(user.Salt);
            var hash = await HashPassword(password, saltBytes);

            if (hash == user.PasswordHash)
                return user;
            return null;
        }

        private async Task<string> HashPassword(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argon2 = new Argon2d(passwordBytes)
            {
                DegreeOfParallelism = 16,
                MemorySize = 8192,
                Iterations = 40,
                Salt = salt
            };

            var hash = await argon2.GetBytesAsync(HashLength);
            return Convert.ToBase64String(hash);
        }

        private async Task<AuthenticationResponse> AuthenticateAsync(ApplicationUser user)
        {
            var payload = new TokenPayload
            {
                Name = user.Username,
                NameIdentifier = user.Id,
                Roles = user.Roles
            };

            var jwtResult = await _jwtAuthManager.GenerateTokens(user.Username, payload, DateTimeOffset.UtcNow);

            var response = new AuthenticationResponse
            {
                Username = user.Username,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken?.TokenString,
                Successful = true
            };
            return response;
        }

        private async Task<ApplicationUser?> FindUserByName(string username)
        {
            var user = await _users.Find(u => u.NormalizedUsername == username.ToUpperInvariant()).FirstOrDefaultAsync();
            return user;
        }
    }
}