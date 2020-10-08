using FivePointes.Api.Configuration;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class TokenRepository : ITokenRepository
    {
        private readonly JwtOptions _options;

        public TokenRepository(IOptions<JwtOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<Result<Token>> GenerateTokenAsync(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create JWToken
            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                subject: identity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(_options.DurationInHours),
                signingCredentials:
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.Default.GetBytes(_options.Secret)),
                        SecurityAlgorithms.HmacSha256Signature));

            return Task.FromResult(Result.Success(new Token {
                AccessToken = tokenHandler.WriteToken(token)
            }));
        }
    }
}
