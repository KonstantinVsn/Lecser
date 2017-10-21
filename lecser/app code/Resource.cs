using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    class Resource
    {
        public static char[] delimiters = { ';', '(', ')', ':', ',', '>', '<', '.'};
        public static string[] multi_delimiters = { "(*", "*)" };
        public static string[] reservedWords = { "PROGRAM", "PROCEDURE", "INTEGER", "EXT", "FLOAT", "BLOCKFLOAT", "EXT",
                                             "SIGNAL", "NO", "ATTR ", "BEGIN","ALWAYSE","EMPTY", "END" };
        public static string[] Identificators = { "SOMEPROG", "PARAMETER1", "PARAMETER2", "PROCEDURE1", "PROCEDURE3",  "PARAMETER3",
                                            "PARAMETER4", "PROCEDURE2", "PARAMETER21", "PARAMETER22", "PARAMETER23" };
    }
}