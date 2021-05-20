﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Netrunner.Shared.Internal.Auth;
using Newtonsoft.Json;

namespace Netrunner.Auth
{
    public class JwtAuthManager
    {
        private readonly Config _config;

        private readonly string? _secret;
        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens; // can store in a database or a distributed cache

        public JwtAuthManager(Config config)
        {
            _config = config;
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _secret = config.Jwt.Secret;
        }

        //    // optional: clean up expired refresh tokens
        //    public void RemoveExpiredRefreshTokens(DateTime now)
        //    {
        //        var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
        //        foreach (var expiredToken in expiredTokens)
        //        {
        //            _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
        //        }
        //    }

        //    // can be more specific to ip, user agent, device name, etc.
        //    public void RemoveRefreshTokenByUsername(string username)
        //    {
        //        var refreshTokens = _usersRefreshTokens.Where(x => x.Value.Username == username).ToList();
        //        foreach (var refreshToken in refreshTokens)
        //        {
        //            _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
        //        }
        //    }

        public Task<JwtAuthResult> GenerateTokens(string username, TokenPayload payload, DateTimeOffset now)
        {
            var expiration = now.AddMinutes(_config.Jwt.AccessTokenExpiration);
            payload.Expiration ??= expiration.ToUnixTimeSeconds();
            payload.IssuedAt ??= now.ToUnixTimeSeconds();
            payload.Issuer ??= _config.Jwt.Issuer;
            payload.Audience ??= _config.Jwt.Audience;

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer(GetJsonSerializer());
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var accessToken = encoder.Encode(payload, _secret);

            var refreshToken = new RefreshToken
            {
                Username = username,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(_config.Jwt.RefreshTokenExpiration)
            };
            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

            return Task.FromResult(new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        private JsonSerializer GetJsonSerializer()
        {
            return new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            };
        }

        //    public JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now)
        //    {
        //        throw new NotImplementedException();
        //        //var (principal, jwtToken) = DecodeJwtToken(accessToken);
        //        //if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        //        //{
        //        //    throw new SecurityTokenException("Invalid token");
        //        //}

        //        //var username = principal.Identity?.Type;
        //        //if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
        //        //{
        //        //    throw new SecurityTokenException("Invalid token");
        //        //}

        //        //if (username == null || existingRefreshToken.Username != username || existingRefreshToken.ExpireAt < now)
        //        //{
        //        //    throw new SecurityTokenException("Invalid token");
        //        //}

        //        //return GenerateTokens(username, principal.Claims.ToArray(), now); // need to recover the original claims
        //    }

        //    //public (ClaimsPrincipal principal, JwtSecurityToken?) DecodeJwtToken(string token)
        //    //{
        //    //    if (string.IsNullOrWhiteSpace(token))
        //    //    {
        //    //        throw new SecurityTokenException("Invalid token");
        //    //    }

        //    //    var principal = new JwtSecurityTokenHandler()
        //    //        .ValidateToken(token,
        //    //            new TokenValidationParameters
        //    //            {
        //    //                ValidateIssuer = true,
        //    //                ValidIssuer = _config.Jwt.Issuer,
        //    //                ValidateIssuerSigningKey = true,
        //    //                IssuerSigningKey = new SymmetricSecurityKey(_secret),
        //    //                ValidAudience = _config.Jwt.Audience,
        //    //                ValidateAudience = true,
        //    //                ValidateLifetime = true,
        //    //                ClockSkew = TimeSpan.FromMinutes(1)
        //    //            },
        //    //            out var validatedToken);
        //    //    return (principal, validatedToken as JwtSecurityToken);
        //    //}

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public class JwtAuthResult
    {
        public string? AccessToken { get; set; }

        public RefreshToken? RefreshToken { get; set; }
    }

    public class RefreshToken
    {
        public string? Username { get; set; } // can be used for usage tracking
        // can optionally include other metadata, such as user agent, ip address, device name, and so on

        public string? TokenString { get; set; }

        public DateTimeOffset ExpireAt { get; set; }
    }
}