using FivePointes.Api.Dtos;
using FivePointes.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FivePointes.Api
{
    public class ErrorActionResult : ActionResult
    {
        private readonly Result _result;

        public ErrorActionResult(Result result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var errors = _result.Errors;
            if(!errors.Any())
            {
                errors = new[] { _result.Status.ToString() };
            }

            var objectResult = new ObjectResult(new ErrorDto { Errors = errors })
            {
                StatusCode = (int)_result.Status
            };

            return objectResult.ExecuteResultAsync(context);
        }
    }
}
