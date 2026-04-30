using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API
{
    public static class CxTokenHelper
    {
        public static string GenerateJSONWebToken(Claim[] claims, string key, string issuer, int? expiresMinutes = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer,
              issuer,
              claims,
              expires: expiresMinutes.HasValue ? DateTime.Now.AddMinutes(expiresMinutes.Value) : null,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
