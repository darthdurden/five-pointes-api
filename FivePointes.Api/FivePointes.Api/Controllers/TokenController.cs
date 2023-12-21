using System;
using System.Net;
using System.Threading.Tasks;
using FivePointes.Api.Dtos;
using FivePointes.Common;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FivePointes.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUsersService _service;

        public TokenController(IUsersService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Consumes("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TokenDto>> Login(TokenDto token)
        {
            var loginResult = await _service.LoginUserAsync(token.Username, token.Password);
            if (!loginResult.IsSuccessful())
            {
                return new ErrorActionResult(Result.Error(HttpStatusCode.Unauthorized));
            }

            return new TokenDto
            {
                AccessToken = loginResult.Value.AccessToken
            };
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public Task Check()
        {
            return Task.CompletedTask;
        }
    }
}