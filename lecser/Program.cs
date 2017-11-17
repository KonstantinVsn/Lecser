using System;
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
            var path = @"input.txt";
            var _input = Loader.Load(path);
            var lAnalizator = new LexAnalizator();
            var codedT = lAnalizator.Analyze(_input);
            lAnalizator.PrintTables();
            Console.WriteLine("=====================Lexical analyze=======================\n");
            var sAnalizator = new SyntaxAnalyze(codedT);
            sAnalizator.Analyze();

            

            
            Console.ReadKey();
            _input.Close();
        }
       
    }
}
