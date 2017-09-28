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
        static string temp;//временная 
        int keywordCount = 1, identifCount = 1;
        public List<Word> IdentifierT = new List<Word>();//таблица идентификаторов 
        public List<Word> ConstantT = new List<Word>();//таблица констант  
        public List<Word> DelimetrT = new List<Word>();//таблица делиметров
        public List<Word> ReservedT = new List<Word>();//таблица зарезервированых слов
        public List<int> CodeT = new List<int>();

        public void Analize(StreamReader text)
        {
            while (text.Peek() >= 0)
            {
                start:
                var symbol = (char)text.Read();
                var acsiiCode = (int)symbol;
                if (symbol == ' ')
                {
                    DefineWord(temp, type, acsiiCode);
                    goto start;
                }
                if (IsLetterOrNum(acsiiCode))// если буква
                {
                    if (temp == null)
                    {
                        temp += symbol;
                        type = CTypes.Keyword;
                    }
                    else if (type == CTypes.Keyword)//при встрече вророго раза буквы,продолжеем читать слово
                    {
                        temp += symbol;
                    }
                    else
                    {
                        DefineWord(temp, type, acsiiCode);
                        temp += symbol;
                        type = CTypes.Keyword;
                    }
                }
                foreach (var delim in Resource.delimiters)// если симаол делисметров
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
                            if (IsComment(temp + symbol))//проверяем на возможность коентария
                            {
                                DefineWord(temp + symbol, type, acsiiCode);//если коментарий то записываем в таблицу

                                do
                                {
                                    symbol = (char)text.Read();
                                } while (symbol != '*');//игнорируем вунтри клментария

                                temp += symbol;
                                symbol = (char)text.Read();
                                if (IsComment(temp + symbol))
                                    DefineWord(temp + symbol, type, acsiiCode);
                                else
                                {
                                    Error();//ошибка елли коментрарий не закрыт
                                    return;
                                }
                                goto start;
                            }
                            else
                            {
                                DefineWord(temp, type, acsiiCode);
                                temp += symbol;
                            }
                        }
                        else
                        {
                            DefineWord(temp, type, acsiiCode);
                            temp += symbol;
                            type = CTypes.Delimetr;
                        }
                        break;
                    }
                }
                if ((!IsLetterOrNum(acsiiCode)) && !IsDelimetr(symbol))//если не буква и не делиметр
                {
                    DefineWord(temp, type, acsiiCode);
                    if (symbol != '\r' && symbol != '\n')//если символ не входит в граматику - ошибка
                    {
                        Error();
                        return;
                    }
                }

            }

        }


        public void DefineWord(string word, CTypes type, int code)
        {
            if (temp == null) return;
            code = GetCode(type, code);
            var l_word = new Word(code, word);
            temp = null;
            if (type == CTypes.Keyword)//добавляем в таблицу если ключевое слово
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
                        l_word.code = GetCode(type, code);
                        ReservedT.Add(l_word);
                        FillCodeT(l_word);
                        return;
                    }
                    else
                        type = CTypes.Identifier;//если слово не заресервированое то считаем его идентификаторо 
                }
            }
            switch (type)
            {
                case CTypes.Identifier: //добавляем идентфикаторы
                    for (int j = 0; j < IdentifierT.Count; j++)
                    {
                        if (word == IdentifierT[j].name)
                        {
                            return;
                        }
                    }
                    FillCodeT(l_word);
                    IdentifierT.Add(l_word);
                    break;
                case CTypes.Delimetr://добавляем делиметры
                    for (int j = 0; j < DelimetrT.Count; j++)
                    {
                        if (word == DelimetrT[j].name)
                        {
                            return;
                        }
                    }
                    FillCodeT(l_word);
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
            i = 0;
            Console.WriteLine("\n закодированое");
            foreach (var item in CodeT)
                Console.Write(" " + item);
        }

        #region private methods

        private void Error()
        {
            Console.WriteLine("ERROR");
        }

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

        private bool IsComment(string delimetr)
        {
            foreach (var delim in Resource.multi_delimiters)
            {
                if (delim == delimetr)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetCode(CTypes type, int code)
        {
            if (type == CTypes.Keyword)
            {
                code = 399 + keywordCount;
                keywordCount++;
            }
            if (type == CTypes.Identifier)
            {
                code = 1000 + identifCount;
                identifCount++;
            }
            return code;
        }

        private void FillCodeT(Word word)
        {
            CodeT.Add(word.code);
        }

        #endregion
    }



    public interface IAnalizator
    {
        void Analize(StreamReader text);
        void DefineWord(string word, CTypes type, int acsiiCode);
        void PrintTables();
    }
}