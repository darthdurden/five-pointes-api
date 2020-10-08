using AutoMapper;
using FivePointes.Data;
using FivePointes.Common;
using FivePointes.Logic.Models;
using FivePointes.Logic.Ports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FivePointes.Api.Adapters
{
    public class UsersRepository : IUsersRepository
    {
        private readonly FivePointesDbContext _context;
        private readonly IMapper _mapper;

        public UsersRepository(FivePointesDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<User>> CreateUserAsync(User user)
        {
            try
            {
                var result = _context.Users.Add(_mapper.Map<Data.Models.User>(user));

                await _context.SaveChangesAsync();

                return Result.Created(_mapper.Map<User>(result.Entity));
            }
            catch (Exception e)
            {
                return Result.Exception<User>(e);
            }
        }

        public async Task<Result<User>> GetUserAsync(string username)
        {
            try
            {
                var user = await _context.Users.Where(x => x.Username == username).SingleOrDefaultAsync();
                if(user != null)
                {
                    return Result.Success(_mapper.Map<User>(user));
                }
                else
                {
                    return Result.Error<User>(HttpStatusCode.NotFound);
                }
            } 
            catch (Exception e)
            {
                return Result.Exception<User>(e);
            }
        }
    }
}
