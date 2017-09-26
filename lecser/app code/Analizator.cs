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
        public List<string> tableI;
        public List<string> tableC;
        public List<string> tableS;
        public List<string> tableK;

        public void Analize(StreamReader text)
        {
            while (text.Peek() >= 0)
            {
                var symbol = (char)text.Read();
                var acsiiCode = (int)symbol;

                // или буква или _
                if (IsLetter(acsiiCode) || (acsiiCode == 95))
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
                        DefineWord(temp, type);
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
                            DefineWord(temp, type);
                            temp += symbol;
                            type = CTypes.Delimetr;
                        }
                    }
                }
                if((!IsLetter(acsiiCode) || (acsiiCode == 95)) && !IsDelimetr(symbol))
                {
                    DefineWord(temp, type);
                    temp = null;
                    type = CTypes.WhiteSpace;
                }

            }
        }


        public void DefineWord(string word, CTypes type)
        {
            if (word != null)
            {
                Console.WriteLine(word, " -- ", type);
            }
            temp = null;
            //for (int j = 0; j < Resource.reservedWords.Length; j++)
            //{
            //    if (word == Resource.reservedWords[j])
            //    {
            //        for (int i = 0; i < tableK.Count; i++)
            //        {
            //            if (temp == tableK[i])
            //            {
            //                return;
            //            }
            //        }
            //        tableK.Add(temp);
            //        return;
            //    }
            //}
            //switch (type)
            //{
            //    case CTypes.Identifier:
            //        for (int j = 0; j < tableI.Count; j++)
            //        {
            //            if (temp == tableI[j])
            //            {
            //                return;
            //            }
            //        }
            //        tableI.Add(temp);
            //        break;
            //    case CTypes.Constant:
            //        for (int j = 0; j < tableC.Count; j++)
            //        {
            //            if (temp == tableC[j])
            //            {
            //                return;
            //            }
            //        }
            //        tableC.Add(temp);
            //        break;
            //    case CTypes.Delimetr:
            //        for (int j = 0; j < tableS.Count; j++)
            //        {
            //            if (temp == tableS[j])
            //            {
            //                return;
            //            }
            //        }
            //        tableS.Add(temp);
            //        break;
            //}
            //return;
        }
        #region private methods
        private bool IsNumber(int acsiiCode)
        {
            return ((acsiiCode >= 48) && (acsiiCode <= 57)) ? true : false;
        }

        private bool IsLetter(int acsiiCode)
        {
            return ((acsiiCode >= 65) && (acsiiCode <= 90)) || ((acsiiCode >= 97) && (acsiiCode <= 122)) ? true : false;
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
        void DefineWord(string word, CTypes type);
    }
}