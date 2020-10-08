using System;
using System.Collections.Generic;
using System.Text;

namespace FivePointes.Common
{
    public static class ResultExtensions
    {
        public static bool IsSuccessful(this Result result)
        {
            return ((int)result.Status) < 400;
        }

        public static Result<TNewType> AsType<TNewType>(this Result result)
        {
            return Result.Error<TNewType>(result.Status);
        }
    }
}
