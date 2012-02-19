using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetFrameworkExtensions
{
    public static class ExceptionExtensions
    {
        private static Exception GetBaseException(this Exception appException)
        {
            Exception baseException = appException;

            //-- Navigate down to find the first Exception
            while (baseException.InnerException != null)
            {
                baseException = baseException.InnerException;
            }

            return (baseException);
        } 
    }
}
