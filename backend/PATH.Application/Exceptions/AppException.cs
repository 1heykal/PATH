using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Application.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; set; }

        public AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
