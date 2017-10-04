using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    public class Word
    {
        public Word(int code, string name, int col,int row)
        {
            this.code = code;
            this.name = name;
            this.col = col;
            this.row = row;
        }
        public int code { get; set; }

        public int row { get; set; }
        public int col { get; set; }
        public string name { get; set; }
    }
}