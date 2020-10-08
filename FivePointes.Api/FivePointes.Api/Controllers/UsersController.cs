using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FivePointes.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto request)
        {
            var createResult = await _service.CreateUserAsync(request.Username, request.Password);
            if(!createResult.IsSuccessful())
            {
                return new ErrorActionResult(createResult);
            }

            return new UserDto { Username = createResult.Value.Username };
        }
    }
}