using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FivePointes.Logic.Services
{
    internal class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly ITokenRepository _tokenRepository;

        public UsersService(IUsersRepository repository, ITokenRepository tokenRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        }

        public async Task<Result<User>> CreateUserAsync(string username, string password)
        {
            // generate a 128-bit salt using a secure PRNG
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return await _repository.CreateUserAsync(new User
            {
                Username = username,
                Salt = Convert.ToBase64String(salt),
                Hash = Convert.ToBase64String(hash)
            });
        }

        public async Task<Result<Token>> LoginUserAsync(string username, string password)
        {
            var userResult = await _repository.GetUserAsync(username);
            if (!userResult.IsSuccessful())
            {
                return userResult.AsType<Token>();
            }

            ClaimsIdentity identity;

            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(userResult.Value.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            if (Convert.ToBase64String(hash) == userResult.Value.Hash)
            {
                var identityResult = await GetIdentityAsync(userResult.Value);
                if (!identityResult.IsSuccessful())
                {
                    return identityResult.AsType<Token>();
                }

                identity = identityResult.Value;
            }
            else
            {
                return Result.Error<Token>(HttpStatusCode.Unauthorized);
            }
            
            return await _tokenRepository.GenerateTokenAsync(identity);
        }

        private async Task<Result<ClaimsIdentity>> GetIdentityAsync(IUser user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            if (user.IsAdmin)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
            
            return Result.Success(claimsIdentity);
        }
    }
}
