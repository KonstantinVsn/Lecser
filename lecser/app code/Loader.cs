
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    static class Loader
    {
        public static StreamReader Load(string path)
        {
            StreamReader stream = new StreamReader(path, Encoding.Default);
            return stream;
        }
    }
}