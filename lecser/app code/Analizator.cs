﻿using System;
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
        int col, row = 1;
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
                var cur_symbol = (char)text.Read();

                var acsiiCode = (int)cur_symbol;
                if (cur_symbol == ' ')
                {
                    DefineWord(temp, type, acsiiCode, col - temp.Length + 1, row);
                    goto start;
                }
                col++;
                if (cur_symbol == '\n')
                {
                    col = 0;
                    row++;
                }
                if (IsLetterOrNum(acsiiCode))// если буква
                {
                    if (temp == null)
                    {
                        temp += cur_symbol;
                        type = CTypes.Keyword;
                    }
                    else if (type == CTypes.Keyword)//при встрече вророго раза буквы,продолжеем читать слово
                    {
                        temp += cur_symbol;
                    }
                    else
                    {
                        DefineWord(temp, type, acsiiCode, col - temp.Length, row);
                        temp += cur_symbol;
                        type = CTypes.Keyword;
                    }
                }
                // если симаол делисметров
                if (IsDelimetr(cur_symbol))
                {
                    if (temp == null)
                    {
                        temp += cur_symbol;
                        type = CTypes.Delimetr;
                    }
                    else if (type == CTypes.Delimetr)
                    {
                        if (IsComment(temp + cur_symbol))//проверяем на возможность коентария
                        {
                            DefineWord(temp + cur_symbol, type, acsiiCode, col - temp.Length, row);//если коментарий то записываем в таблицу
                            start_COM:
                            do
                            {
                                cur_symbol = (char)text.Read();
                                if(cur_symbol == '\uffff')
                                {
                                    return;
                                }
                                    
                                col++;
                            } while (cur_symbol != '*');//игнорируем вунтри клментария

                            temp += cur_symbol;
                            cur_symbol = (char)text.Read();
                            if (IsComment(temp + cur_symbol))
                            {
                                DefineWord(temp + cur_symbol, type, acsiiCode, col - temp.Length, row);
                            }
                            else
                            {
                                temp = null;
                                goto start_COM;
                                cur_symbol = (char)text.Read();
                                Error();//ошибка елли коментрарий не закрыт
                            }
                            goto start;
                        }
                        else
                        {
                            DefineWord(temp, type, acsiiCode, col - temp.Length, row);

                            temp += cur_symbol;
                        }
                    }
                    else
                    {
                        DefineWord(temp, type, acsiiCode, col - temp.Length, row);
                        temp += cur_symbol;
                        type = CTypes.Delimetr;
                    }
                }
                if ((!IsLetterOrNum(acsiiCode)) && !IsDelimetr(cur_symbol) && temp != null)//если не буква и не делиметр
                {
                    DefineWord(temp, type, acsiiCode, col - temp.Length, row);
                    if (cur_symbol != '\r' && cur_symbol != '\n')//если символ не входит в граматику - ошибка
                    {
                        UnexpectedToken(cur_symbol,row,col);
                    }
                }
            }
        }


        public void DefineWord(string word, CTypes type, int code, int col, int row)
        {
            if (temp == null)
                return;
            code = GetCode(type, code);
            var l_word = new Word(code, word, col, row);
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
                    {
                        type = CTypes.Identifier;//если слово не заресервированое то считаем его идентификаторо 
                        if (Char.IsDigit(l_word.name.First()))
                        {
                            var l_constant = "";
                            var l_identif = "";
                            for (var i = 0; i < l_word.name.Length; i++)
                            {
                                if (Char.IsDigit(l_word.name[i]))
                                {
                                    l_constant += l_word.name[i];
                                }
                                else
                                {
                                    do
                                    {
                                        i++;
                                        l_identif += l_word.name[i];
                                    } while (IsLetter(l_word.name[i]));
                                    break;
                                }
                            }

                            var constant_word = new Word(code, l_constant, col, row);
                            ConstantT.Add(constant_word);
                            var identif_word = new Word(code, l_identif, col+ l_constant.Length, row);
                            IdentifierT.Add(identif_word);
                            FillCodeT(constant_word);
                            FillCodeT(identif_word);
                            Console.WriteLine("[" + l_word.row + "," + l_word.col + "] '" + l_word.name + "' ERROR : Unexpected token (started from num)");
                            return;
                        }
                    }
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
                Console.WriteLine(++i + ": " + "[" + item.row + ", " + item.col + "] <" + item.code + "> " + item.name);
            Console.WriteLine("\n делиметры");
            i = 0;
            foreach (var item in DelimetrT)
                Console.WriteLine(++i + ": " + "[" + item.row + ", " + item.col + "] <" + item.code + "> " + item.name);
            Console.WriteLine("\n идентификаторы");
            i = 0;
            foreach (var item in IdentifierT)
                Console.WriteLine(++i + ": " + "[" + item.row + ", " + item.col + "] <" + item.code + "> " + item.name);
            i = 0;
            Console.WriteLine("\n констант");
            foreach (var item in ConstantT)
                Console.WriteLine(++i + ": " + "[" + item.row + ", " + item.col + "] <" + item.code + "> " + item.name);

            i = 0;
            Console.WriteLine("\n закодированое");
            foreach (var item in CodeT)
                Console.Write(" " + item);
        }

        #region private methods

        private void Error()
        {
            Console.WriteLine("ERROR" + "'"+ "'" + "[" + row + "," + col + "] " + "EXPECTED TOKEN ')'");
        }
        private void UnexpectedToken(char c, int row, int col)
        {
            Console.WriteLine( "ERROR" + "'" +c+ "'" +"["+row+","+col+"] " + "UNEXPECTED TOKEN");
        }
        private bool IsNumber(int acsiiCode)
        {
            return ((acsiiCode >= 48) && (acsiiCode <= 57)) ? true : false;
        }

        private bool IsLetterOrNum(int acsiiCode)
        {
            return ((acsiiCode >= 65) && (acsiiCode <= 90)) || ((acsiiCode >= 97) && (acsiiCode <= 122) || ((acsiiCode >= 48) && (acsiiCode <= 57))) ? true : false;
        }

        private bool IsNumber(string str)
        {
            var temp = (char)str[0];
            return ((temp >= 48) && (temp <= 57)) ? true : false;
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

        private bool IsLetter(int acsiiCode)
        {
            return ((acsiiCode >= 65) && (acsiiCode <= 90)) || ((acsiiCode >= 97) && (acsiiCode <= 122)) ? true : false;
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
        void DefineWord(string word, CTypes type, int acsiiCode, int col, int row);
        void PrintTables();
    }
}