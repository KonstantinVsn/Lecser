using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    public class Analizator : IAnalizator
    {
        static CTypes type;
        static string temp;
        public int col = 0, row = 0;
        public List<Keyword> IdentifierT = new List<Keyword>();
        public List<Keyword> ConstantT = new List<Keyword>();
        public List<Keyword> DelimetrT = new List<Keyword>();
        public List<Keyword> ReservedT = new List<Keyword>();

        public void Analize(StreamReader text)
        {
            while (text.Peek() >= 0)
            {
                var symbol = (char)text.Read();
                var acsiiCode = (int)symbol;
                if (symbol == '\n')
                    row++;
                col++;
                // или буква или _
                if (IsLetterOrNum(acsiiCode) || (acsiiCode == 95))
                {
                    if (temp == null)
                    {
                        temp += symbol;
                        type = CTypes.Kewword;
                    }
                    else if (type == CTypes.Kewword)
                    {
                        temp += symbol;
                    }
                    else
                    {
                        DefineWord(temp, type, row, col, acsiiCode);
                        temp += symbol;
                        type = CTypes.Kewword;
                    }
                }

                // или делиметр
                foreach (var delim in Resource.delimiters)
                {
                    if (delim == symbol)
                    {
                        if (temp == null)
                        {
                            temp += symbol;
                            type = CTypes.Delimetr;
                        }
                        else if (type == CTypes.Delimetr)
                        {
                            temp += symbol;
                        }
                        else
                        {
                            DefineWord(temp, type, row, col, acsiiCode);
                            temp += symbol;
                            type = CTypes.Delimetr;
                        }
                    }
                }
                if ((!IsLetterOrNum(acsiiCode) || (acsiiCode == 95)) && !IsDelimetr(symbol))
                {
                    DefineWord(temp, type, row, col, acsiiCode);
                    temp = null;
                    type = CTypes.WhiteSpace;
                }

            }
        }


        public void DefineWord(string word, CTypes type, int row, int col, int code)
        {
            var l_word = new Keyword(code,row,col, word);
            temp = null;
            col = 0;
            if (type == CTypes.Kewword)
            {
                for (int j = 0; j < Resource.reservedWords.Length; j++)
                {
                    if (word == Resource.reservedWords[j])
                    {
                        for (int i = 0; i < ReservedT.Count; i++)
                        {
                            if (word == ReservedT[i].name)
                            {
                                return;
                            }
                        }
                        ReservedT.Add(l_word);
                        return;
                    }
                    else
                        type = CTypes.Identifier;
                }
            }
            switch (type)
            {
                case CTypes.Identifier:
                    for (int j = 0; j < IdentifierT.Count; j++)
                    {
                        if (word == IdentifierT[j].name)
                        {
                            return;
                        }
                    }
                    IdentifierT.Add(l_word);
                    break;
                case CTypes.Delimetr:
                    for (int j = 0; j < DelimetrT.Count; j++)
                    {
                        if (word == DelimetrT[j].name)
                        {
                            return;
                        }
                    }
                    DelimetrT.Add(l_word);
                    break;
            }
            return;
        }

        public void PrintTables()
        {
            Console.WriteLine("зарезервированые слова");
            var i = 0;
            foreach (var item in ReservedT)
                Console.WriteLine(++i + ": " + item.name);
            Console.WriteLine("\n делиметры");
            i = 0;
            foreach (var item in DelimetrT)
                Console.WriteLine(++i + ": " + item.name);
            Console.WriteLine("\n идентификаторы");
            i = 0;
            foreach (var item in IdentifierT)
                Console.WriteLine(++i + ": " + item.name);
        }
        #region private methods
        private bool IsNumber(int acsiiCode)
        {
            return ((acsiiCode >= 48) && (acsiiCode <= 57)) ? true : false;
        }

        private bool IsLetterOrNum(int acsiiCode)
        {
            return ((acsiiCode >= 65) && (acsiiCode <= 90)) || ((acsiiCode >= 97) && (acsiiCode <= 122) || ((acsiiCode >= 48) && (acsiiCode <= 57))) ? true : false;
        }

        private bool IsDelimetr(char c)
        {
            foreach (var delim in Resource.delimiters)
            {
                if (delim == c)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }



    public interface IAnalizator
    {
        void Analize(StreamReader text);
        void DefineWord(string word, CTypes type, int row, int col, int acsiiCode);
        void PrintTables();
    }
}