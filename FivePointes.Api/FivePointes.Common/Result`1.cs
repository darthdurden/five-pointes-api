using System;
using System.Collections.Generic;
using System.Net;

namespace FivePointes.Common
{
    public class Result<T> : Result
    {
        protected internal Result(HttpStatusCode status, T value) : base(status)
        {
            Value = value;
        }

        protected internal Result(HttpStatusCode status, T _, params string[] errors) : base(status, errors) { }

        protected internal Result(Exception exception) : base(exception) { }

        public T Value { get; set; }
    }
}
