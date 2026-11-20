using CruxIT.Library.API;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CruxIT.Library.API.Tests;

public sealed class CxTokenHelperTests
{
    private const string SigningKey = "test-signing-key-that-is-long-enough-for-hmac-sha256";

    [Fact]
    public void GenerateJSONWebToken_EncodesIssuerAudienceAndClaims()
    {
        var claims = new[]
        {
            new Claim("sub", "user-42"),
            new Claim("role", "admin")
        };

        var tokenText = CxTokenHelper.GenerateJSONWebToken(claims, SigningKey, "test-issuer");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenText);

        Assert.Equal("test-issuer", token.Issuer);
        Assert.Contains("test-issuer", token.Audiences);
        Assert.Equal("user-42", token.Claims.Single(claim => claim.Type == "sub").Value);
        Assert.Equal("admin", token.Claims.Single(claim => claim.Type == "role").Value);
        Assert.Equal(DateTime.MinValue, token.ValidTo);
    }

    [Fact]
    public void GenerateJSONWebToken_SetsRequestedExpirationAndValidSignature()
    {
        var before = DateTime.UtcNow;
        var tokenText = CxTokenHelper.GenerateJSONWebToken([], SigningKey, "test-issuer", 5);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenText);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "test-issuer",
            ValidateAudience = true,
            ValidAudience = "test-issuer",
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey))
        };

        new JwtSecurityTokenHandler().ValidateToken(tokenText, validationParameters, out _);

        Assert.InRange(token.ValidTo, before.AddMinutes(4), before.AddMinutes(6));
    }
}
