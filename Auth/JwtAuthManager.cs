using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Netrunner.Auth
{
    //public class JwtAuthManager
    //{
    //    public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
    //    private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens; // can store in a database or a distributed cache
    //    private readonly NetrunnerConfig _config;
    //    private readonly byte[]? _secret;

    //    public JwtAuthManager(NetrunnerConfig config)
    //    {
    //        _config = config;
    //        _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
    //        if (config.Jwt.Secret != null)
    //            _secret = Encoding.UTF8.GetBytes(config.Jwt.Secret);
    //    }

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
    //    public void RemoveRefreshTokenByUserName(string userName)
    //    {
    //        var refreshTokens = _usersRefreshTokens.Where(x => x.Value.UserName == userName).ToList();
    //        foreach (var refreshToken in refreshTokens)
    //        {
    //            _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
    //        }
    //    }

    //    public JwtAuthResult GenerateTokens(string username, ICollection<Claim> claims, DateTime now)
    //    {
    //        throw new NotImplementedException();
    //        //var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
    //        //var jwtToken = new JwtSecurityToken(
    //        //    _config.Jwt.Issuer,
    //        //    shouldAddAudienceClaim ? _config.Jwt.Audience : string.Empty,
    //        //    claims,
    //        //    expires: now.AddMinutes(_config.Jwt.AccessTokenExpiration),
    //        //    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
    //        //var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

    //        //var refreshToken = new RefreshToken
    //        //{
    //        //    UserName = username,
    //        //    TokenString = GenerateRefreshTokenString(),
    //        //    ExpireAt = now.AddMinutes(_config.Jwt.RefreshTokenExpiration)
    //        //};
    //        //_usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

    //        //return new JwtAuthResult
    //        //{
    //        //    AccessToken = accessToken,
    //        //    RefreshToken = refreshToken
    //        //};
    //    }

    //    public JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now)
    //    {
    //        throw new NotImplementedException();
    //        //var (principal, jwtToken) = DecodeJwtToken(accessToken);
    //        //if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
    //        //{
    //        //    throw new SecurityTokenException("Invalid token");
    //        //}

    //        //var userName = principal.Identity?.Name;
    //        //if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
    //        //{
    //        //    throw new SecurityTokenException("Invalid token");
    //        //}

    //        //if (userName == null || existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < now)
    //        //{
    //        //    throw new SecurityTokenException("Invalid token");
    //        //}

    //        //return GenerateTokens(userName, principal.Claims.ToArray(), now); // need to recover the original claims
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

    //    private static string GenerateRefreshTokenString()
    //    {
    //        var randomNumber = new byte[32];
    //        using var randomNumberGenerator = RandomNumberGenerator.Create();
    //        randomNumberGenerator.GetBytes(randomNumber);
    //        return Convert.ToBase64String(randomNumber);
    //    }
    //}
}