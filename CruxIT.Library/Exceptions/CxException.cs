using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.Exceptions
{
    public class CxException : Exception
    {
        public string? MessageResourceKey { get; set; }
        public object?[]? MessageResourceArgs { get; set; }
        public CxHttpStatusCode? StatusCode { get; set; }

        public CxException(string? message = null)
            :base(message)
        {
        }

        public CxException(CxHttpStatusCode statusCode, string? message = null)
            :this(message)
        {
            StatusCode = statusCode;
        }

        public CxException(CxHttpStatusCode statusCode, string messageResourceKey, params object?[]? args)
            :this ()
        {
            StatusCode = statusCode;
        }

        public CxException(string defaultMessage, CxHttpStatusCode statusCode, string messageResourceKey, params object?[]? args)
            :this (defaultMessage)
        {
            StatusCode = statusCode;
        }

        public CxException(string messageResourceKey, params object?[]? args)
            : this()
        {
            MessageResourceKey = messageResourceKey;
            MessageResourceArgs = args;
        }
    }
}
