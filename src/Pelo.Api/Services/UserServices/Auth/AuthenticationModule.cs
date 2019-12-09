using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pelo.Common.Dtos.Account;

namespace Pelo.Api.Services.UserServices.Auth
{
    public class AuthenticationModule
    {
        private SecurityKey _signingKey;

        public string GenerateTokenForUser(LogonDetail user,
            string secretKey)
        {
            _signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));

            //Set issued at date
            var issuedAt = DateTime.UtcNow;
            //set the time when it expires
            var expires = DateTime.UtcNow.AddMonths(3);

            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("Id",
                    user.Id.ToString()),
                new Claim("Username",
                    user.Username),
                new Claim("DisplayName",
                    user.DisplayName+""),
                new Claim("Avatar",
                    user.Avatar+"")
            });

            var signingCredentials = new SigningCredentials(_signingKey,
                SecurityAlgorithms.HmacSha256Signature);

            //create the jwt
            var token = tokenHandler.CreateJwtSecurityToken("http://api.pelo.vn",
                "http://api.pelo.vn",
                claimsIdentity,
                issuedAt,
                expires,
                signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public JwtSecurityToken GenerateUserClaimFromJwt(string authToken,
            string secretKey)
        {
            _signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[] {"http://api.pelo.vn"},

                ValidIssuers = new[] {"http://api.pelo.vn"},
                IssuerSigningKey = _signingKey,
                LifetimeValidator = LifetimeValidator
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(authToken,
                    tokenValidationParameters,
                    out validatedToken);
            }
            catch (Exception)
            {
                return null;
            }

            return validatedToken as JwtSecurityToken;
        }

        public JwtAuthenticationIdentity PopulateUserIdentity(JwtSecurityToken userPayloadToken)
        {
            var name = userPayloadToken.Claims.FirstOrDefault(m => m.Type == "Username")
                           ?.Value ?? string.Empty;
            //string userId = ((userPayloadToken)).Claims.FirstOrDefault(m => m.Type == "nameid").Value;
            var userId = Convert.ToInt32(userPayloadToken.Claims.FirstOrDefault(c => c.Type == "Id")
                                             ?.Value ?? "0");
            return new JwtAuthenticationIdentity(name,
                userId)
            {
                Username = name,
                Id = userId
            };
        }

        private bool LifetimeValidator(DateTime? notBefore,
            DateTime? expires,
            SecurityToken token,
            TokenValidationParameters @params)
        {
            if (expires != null) return expires > DateTime.UtcNow;

            return false;
        }
    }
}