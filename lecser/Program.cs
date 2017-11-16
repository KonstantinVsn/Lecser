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
            //var path = "D:/5 курс/compilathor/_lecser/lecser/app data/input.txt";
            //var _input = Loader.Load(path);
            //var lAnalizator = new LexAnalizator();
            //var codedT = lAnalizator.Analyze(_input);
            //lAnalizator.PrintTables();
            //Console.WriteLine("=====================Lexical analyze=======================\n");
            //var sAnalizator = new SyntaxAnalyze(codedT);
            //sAnalizator.Analyze();

            TreeNode<string> treeRoot = SampleData.GetSet1();
            foreach (TreeNode<string> node in treeRoot)
            {
                string indent = CreateIndent(node.Level);
                Console.WriteLine(indent + (node.Data ?? "null"));
            }

            
            Console.ReadKey();
            //_input.Close();
        }
        private static String CreateIndent(int depth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("| ");
            }
            return sb.ToString();
        }
    }
}
