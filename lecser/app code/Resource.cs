﻿using System;
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

        public const int DELIMITERS_FROM = 0;
        public const int DELIMITERS_TO = 256;
        public const int MULTIPLE_DELIMITERS_FROM = 301;
        public const int MULTIPLE_DELIMITERS_TO = 400;
        public const int KEYS_FROM = 401;
        public const int KEYS_TO = 500;
        public const int CONSTANTS_FROM = 501;
        public const int CONSTANTS_TO = 1000;
        public const int IDENTIFIERS_FROM = 1001;
        public const int IDENTIFIERS_TO = 2000;
    }
}