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
        public CxExceptionTypes? ExceptionType { get; set; }

        public CxException(string? message = null)
            :base(message)
        {
        }

        public CxException(CxExceptionTypes exceptionType, string? message = null)
            :this(message)
        {
            ExceptionType = exceptionType;
        }

        public CxException(CxExceptionTypes exceptionType, string messageResourceKey, params object?[]? args)
            :this ()
        {
            ExceptionType = exceptionType;
        }

        public CxException(string defaultMessage, CxExceptionTypes exceptionType, string messageResourceKey, params object?[]? args)
            :this (defaultMessage)
        {
            ExceptionType = exceptionType;
        }

        public CxException(string messageResourceKey, params object?[]? args)
            : this()
        {
            MessageResourceKey = messageResourceKey;
            MessageResourceArgs = args;
        }
    }
}
