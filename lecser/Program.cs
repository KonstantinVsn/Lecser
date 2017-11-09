﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lecser.app_code;
namespace lecser
{
    class Programs
    {
        static void Main()
        {
            var path = "D:/5 курс/compilathor/_lecser/lecser/app data/input.txt";
            var _input = Loader.Load(path);
            var analizator = new LexAnalizator();
            var codedT = analizator.Analyze(_input);
            analizator.PrintTables();
            Console.ReadKey();
            _input.Close();
        }
    }
}
