using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FivePointes.Common
{
    public class Result
    {
        protected Result(HttpStatusCode status)
        {
            Status = status;
        }

        protected Result(HttpStatusCode status, params string[] errors) : this(status)
        {
            Errors = errors;
        }

        protected Result(Exception exception) : this(HttpStatusCode.InternalServerError)
        { 
            while(exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            Errors = new[] { exception.Message };
        }

        public HttpStatusCode Status { get; set; }

        public IEnumerable<string> Errors { get; set; } = new string[] { };

        public static Result Success()
        {
            return new Result(HttpStatusCode.OK);
        }

        public static Result<TResult> Success<TResult>(TResult value)
        {
            return new Result<TResult>(HttpStatusCode.OK, value);
        }

        public static Result<TResult> Created<TResult>(TResult value)
        {
            return new Result<TResult>(HttpStatusCode.Created, value);
        }

        public static Result Error(HttpStatusCode errorCode)
        {
            return new Result(errorCode);
        }

        public static Result Error(HttpStatusCode errorCode, params string[] errors)
        {
            return new Result(errorCode, errors);
        }

        public static Result Error(HttpStatusCode errorCode, IEnumerable<string> errors)
        {
            return new Result(errorCode, errors.ToArray());
        }


        public static Result<TResult> Error<TResult>(HttpStatusCode errorCode)
        {
            return new Result<TResult>(errorCode, default);
        }

        public static Result<TResult> Error<TResult>(HttpStatusCode errorCode, params string[] errors)
        {
            return new Result<TResult>(errorCode, default, errors);
        }

        public static Result Exception(Exception e)
        {
            return new Result(e);
        }

        public static Result<TResult> Exception<TResult>(Exception e)
        {
            return new Result<TResult>(e);
        }
    }
}
