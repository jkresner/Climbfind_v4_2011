using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation.Exceptions
{
    public class IllegalCheckInException : Exception
    {
        public IllegalCheckInException(string message) : base(message) { }
    }
}
