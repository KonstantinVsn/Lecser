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
            var path = "D:/5 курс/compilathor/_lecser/lecser/app data/input.txt";
            var _input = Loader.Load(path);
            while (_input.Peek() >= 0)
            {
                Analizator.Analize((char)_input.Read());
            }
            _input.Close();
        }
    }
}
