using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetFrameworkExtensions
{
    public static class StreamExtensions
    {
        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }
    }
}
