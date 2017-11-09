using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    class SyntaxAnalyze : ISyntaxAnalyze
    {
        public int index = 0;

        public void Anylyze(List<Word> input)
        {

            if (input[index].name == "PROGRAM")
            {
                if (IsPocedireIdentifier(input[index + 1]))
                {
                    if (input[index + 2].name == IsDelimReserved(";"))
                    {
                        var curIndex = index + 3;
                        if (IsBlock(input[curIndex], input[curIndex + 1]))
                        {

                        }
                        if (IsProcedure(input[curIndex]))
                        {
                            if (IsPocedireIdentifier(input[curIndex + 1]))
                            {
                                if (IsParametrList())
                                {
                                    if (IsDeclarationList())
                                    {

                                    }
                                    else
                                        Empty();
                                }
                                else
                                    Empty();
                                //index++;
                                if (IsBlock(input[curIndex]))
                                {

                                }
                            }
                        }
                        Error();
                    }
                    else
                        Error();
                }
                else
                    Error();
            }
            else
                Error();

        }

        public static bool IsPocedireIdentifier(Word lecsem)
        {
            if (lecsem.code >= Resource.IDENTIFIERS_FROM && lecsem.code <= Resource.IDENTIFIERS_TO)
            {
                return true;
            }
            return false;
        }

        public static string IsDelimReserved(string delim)
        {
            if (Resource.delimiters.Contains(delim[0]))
            {
                return delim;
            }
            return null;
        }
        public static bool IsBlock(Word begin, Word end)
        {
            if (begin.name == "BEGIN" && end.name == "END")
            {
                return true;
            }
            return false;
        }
        public static bool IsProcedure(Word lecsem)
        {
            if (lecsem.name == "PROCEDURE")
            {
                return true;
            }
            return false;
        }
        public static bool IsBlock(Word lecsem)
        {
            if (lecsem.name == "BEGIN")
            {
                return true;
            }
            return false;
        }
        public static bool IsParametrList()
        {

            return false;
        }
        public static bool IsIdentifier()
        {

            return false;
        }
        public static bool IsAttributeList()
        {
            if (IsAttribute())
            {
                if (IsAttributeList())
                    return true;
            }
            
            return false;
        }
        
        public static bool IsAttribute()
        {

            return false;
        }

        public static bool IsIdentifierList()
        {
            if (IsVarIdentifier())
                return true;
            else
                Empty();
            return false;
        }

        public static bool IsDeclaration()
        {
            if (IsVarIdentifier())
            {
                if (IsIdentifierList())
                {
                    if (true) // next symbol :
                    {
                        if (IsAttribute())
                        {
                            if (IsAttributeList())
                            {
                                return true;
                            }
                        }


                    }
                }
            }
            return false;
        }

        public static bool IsDeclarationList()
        {
            if (IsDeclaration())
            {

            }
            return false;
        }

        public static bool IsVarIdentifier()
        {
            if (IsIdentifier())
                return true;
            return false;
        }
        public static void Error() { }
        public static void Empty() { }

    }

    interface ISyntaxAnalyze
    {
        void Anylyze(List<Word> input);
    }
}