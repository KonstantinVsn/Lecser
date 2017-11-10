using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lecser.app_code
{
    class SyntaxAnalyze : ISyntaxAnalyze
    {
        public static int index = 0;
        public static int depth = 0;
        public static int currdepth = 1;
        public static List<Word> input;
        public static List<Tuple<int, string>> log = new List<Tuple<int, string>>();
        public SyntaxAnalyze(List<Word> _input)
        {
            input = _input;
        }

        public bool Analyze()
        {
            logging(depth, "<signal-program>");
            if (input[index].name == "PROGRAM")
            {
                index++;
                logging(depth + currdepth, "<program>");
                if (IsPocedireIdentifier(input[index]))
                {
                   
                    index++;
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        logging(depth + currdepth + 4, input[index].name);
                        index++;
                        currdepth++;
                        if (IsBlock(input[index], input[index + 1]))
                        {
                            if (input.Count > index + 2)
                            {
                                if (input[index + 2].name == IsDelimReserved("."))
                                {
                                    logging(depth + 2, ".");
                                    goto print;
                                }
                            }
                            else
                            {
                                Error(depth + 2, "(!) expected '.', but has end of file");
                                goto print;
                            }
                        }
                        
                        if (IsProcedure(input[index]))
                        {
                            goto print;
                        }

                        Error(depth, "(!) expected '<block>' or PROCEDURE, but has " + input[index]);
                    }
                    else
                        Error(depth, "(!) expected ';', but has " + input[index + 2]);
                }
                else
                    goto print;
            }
            else
                Error(depth, "(!) expected '<block>' or PROCEDURE, but has " + input[index]);

            print: PrettyPrint();
            return true;
        }

        public static bool IsBlock(Word begin, Word end)//not checking statement-list cource it is empty
        {
            if (begin.name == "BEGIN")
            {
                index++;
                if (end.name == "END") { 
                    logging(depth + currdepth + 1, "<block>");
                    logging(depth + currdepth + 2, begin.name);
                    logging(depth + currdepth + 3, "<statement-list>");
                    logging(depth + currdepth + 4, "<empty>");
                    logging(depth + currdepth + 2, end.name);
                    index++;
                }
                return true;
            }
            if (begin.name == "END")
            {
                Error(depth, "(!) expected 'BEGIN', but has " + begin.name);
                return false;
            }
            return false;
            

        }

        public static bool IsProcedure(Word lecsem)
        {
            if (lecsem.name == "PROCEDURE")
            {
                logging(depth + currdepth + 1, lecsem.name);
                index++;
                if (IsPocedireIdentifier(input[index]))
                {
                    index++;
                    if (IsParametrList(input[index]))
                    {
                        index++;
                        if (input[index].name == IsDelimReserved(";"))
                        {
                            logging(depth + currdepth + 1, input[index].name);
                            index++;
                            if (IsBlock(input[index], input[index+1]))
                            {
                                if (input[index].name == IsDelimReserved(";"))
                                {
                                    logging(depth + currdepth + 1, input[index].name);
                                    return true;
                                }
                                    
                            }
                        }
                    }
                    else
                        Empty(depth);
                }
            }
            return false;
        }

        public static bool IsPocedireIdentifier(Word lecsem)
        {
            logging(depth + currdepth + 2, "<procedure-identifier>");
            if (IsIdentifier(lecsem))
            {
                logging(depth + currdepth + 2, lecsem.name);
                return true;
            }
            else
            {
                Error(depth+3, "expected '<identifier>', but has " + lecsem.name);
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

        public static bool IsParametrList(Word lecsem)
        {
            logging(depth + currdepth+ 2, "<parametr-list>");
            if (input[index].name == IsDelimReserved("("))
            {
                index++;
                logging(depth + currdepth + 3, "(");


                if (IsDeclarationList(input[index]))
                {
                    if (input[index].name == IsDelimReserved(")"))
                    {
                        logging(depth + currdepth + 3, ")");
                        return true;
                    }
                }
                else
                {
                    Error(depth + 3, "expected '<parametr-list>', but has " + input[index].name);
                }
            }
            else
            {
                Error(depth + 3, "expected '(', but has " + input[index].name);
            }
            
            return false;
        }
        public static bool IsIdentifier(Word lecsem)
        {
            if (lecsem.code >= Resource.IDENTIFIERS_FROM && lecsem.code <= Resource.IDENTIFIERS_TO)
            {
                logging(depth + currdepth + 3, "identifier>");
                return true;
            }
            return false;
        }
        public static bool IsAttributeList(Word lecsem)
        {
            if (IsAttribute(lecsem))
            {
                if (IsAttributeList(lecsem))
                    return true;
            }

            return false;
        }

        public static bool IsAttribute(Word lecsem)
        {
            if (Resource.attributes.Contains(lecsem.name))
            {
                logging(depth, "<attribute>");
                return true;
            }
            return false;
        }

        public static bool IsIdentifierList(Word lecsem)
        {
            if (IsVarIdentifier(lecsem))
                return true;
            else
                Empty(depth);
            return false;
        }

        public static bool IsDeclaration(Word lecsem)
        {
            logging(depth + 2, "<declaration>");
            if (IsVarIdentifier(lecsem))
            {
                if (IsIdentifierList(lecsem))
                {
                    if (lecsem.name == IsDelimReserved(":")) // next symbol :
                    {
                        if (IsAttribute(lecsem))
                        {
                            if (IsAttributeList(lecsem))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                    Empty(depth);

                if (IsAttribute(lecsem))
                {
                    if (IsAttributeList(lecsem))
                    {

                    }
                    else
                        Empty(depth);
                }
            }
            return false;
        }

        public static bool IsDeclarationList(Word lecsem)
        {
            logging(depth + currdepth + 4, "<declaration-list");
            if (input[index].name == ")")
            {
                Empty(depth + currdepth + 5);//todo current depth
                return true;
            }
            if (IsDeclaration(input[index]))
            {
                IsDeclarationList(input[index]);
            }
            else
                return Empty(depth);
            return false;
        }

        public static bool IsVarIdentifier(Word lecsem)
        {
            if (IsIdentifier(lecsem))
            {
                logging(depth, "<attribute>");
                return true;
            }

            return false;
        }
        public static void logging(int depth, string info)
        {
            log.Add(Tuple.Create(depth, info));
        }
        public static void PrettyPrint()
        {

            var dep = 1;
            foreach (var node in log)
            {
            Console.WriteLine(GetDepth(node.Item1) + node.Item2);
        }
        }
        public static string GetDepth(int depth)
        {
            var dep = "";
            for (int i = 0; i < depth*2; i++)
            {
                dep += "| ";
            }
            return dep;
        }
        public static void Error(int depth, string info)
        {
            logging(depth, info);
        }
        public static bool Empty(int depth)
        {
            logging(depth, "<empty>");
            return true;
        }

    }

    interface ISyntaxAnalyze
    {
        bool Analyze();
    }
}