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
                logging(depth + currdepth + 1, "PROGRAM");
                if (IsPocedireIdentifier(input[index]))
                {

                    index++;
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        logging(depth + currdepth + 2, input[index].name);
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


                    }
                    else
                        Error(depth, "(!) expected ';', but has " + input[index + 2]);
                }
                else
                    goto print;
            }
            else
                Error(depth, "(!) expected 'BEGIN' or 'PROCEDURE', but has end of file");

            print: PrettyPrint();
            return true;
        }

        public static bool IsBlock(Word begin, Word end)//not checking statement-list cource it is empty
        {
            if (begin.name == "BEGIN")
            {
                index++;
                if (end.name == "END")
                {
                    logging(depth + currdepth + 1, "<block>");
                    logging(depth + currdepth + 2, begin.name);
                    logging(depth + currdepth + 3, "<statements-list>");
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
                        if (input[index].name == IsDelimReserved(";"))
                        {
                            logging(depth + currdepth + 1, input[index].name);
                            index++;
                            if (input.Count > index)
                            {
                                if (IsBlock(input[index], input[index + 1]))
                                {
                                    if (input[index].name == IsDelimReserved(";"))
                                    {
                                        logging(depth + currdepth + 1, input[index].name);
                                        return true;
                                    }
                                }
                            }
                            else if(input.Count == index+1)
                            {
                                Console.WriteLine();
                            }
                            else
                                Error(depth + 3, "expected 'BEGIN', but has end of file");
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
            if (IsIdentifier(lecsem, 0))
            {
                logging(depth + currdepth + 4, lecsem.name);
                return true;
            }
            else
            {
                Error(depth + 3, "expected '<identifier>', but has " + lecsem.name);
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
            logging(depth + currdepth + 2, "<parameters-list>");
            if (input[index].name == IsDelimReserved("("))
            {
                index++;
                logging(depth + currdepth + 3, "(");


                if (IsDeclarationList(input[index]))
                {
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        logging(depth + currdepth + 3, ";");
                        //if (IsBlock(input[index], input[index + 1])){
                            return true;
                        //}
                        
                        
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
        public static bool IsIdentifier(Word lecsem, int depthin)
        {
            if (lecsem.code >= Resource.IDENTIFIERS_FROM && lecsem.code <= Resource.IDENTIFIERS_TO)
            {
                logging(depth + currdepth + depthin + 3, "<identifier>");
                return true;
            }
            return false;
        }
        public static bool IsAttributeList(Word lecsem)
        {
            index++;
            logging(depth + currdepth + 4, "<attributes-list>");
            if (IsAttribute(input[index]))
            {
                logging(depth + currdepth + 5, input[index].name);
                if (IsAttributeList(input[index]))
                    return true;
            }
            else if (input[index].name == IsDelimReserved(")") && IsAttribute(input[index - 1]))
            {
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
            index++;
            logging(depth +currdepth +6, "<identifiers-list>");
            if (input[index].name == IsDelimReserved(","))
            {
                logging(depth + currdepth + 7, input[index].name);
                index++;
                if (IsVarIdentifier(input[index], 2))
                {
                    logging(depth + currdepth + 8, input[index].name);
                    return IsIdentifierList(input[index]);
                }
            }
            else if (input[index].name == IsDelimReserved(":") && !IsVarIdentifier(input[index - 1],0))
                return false;
            return true;
        }

        public static bool IsDeclaration(Word lecsem)
        {
            logging(depth + currdepth + 5, "<declaration>");
            if (IsVarIdentifier(input[index], 4))
            {
                logging(depth + currdepth + 8, input[index].name);
                if (IsIdentifierList(input[index]))//in fun index++;
                {
                    if (input[index].name == IsDelimReserved(":")) // next symbol :
                    {
                        logging(depth + currdepth + 4, input[index].name);
                        index++;
                        if (IsAttribute(input[index]))//in fun index++;
                        {
                            logging(depth + currdepth + 5, input[index].name);
                            if (IsAttributeList(input[index]))
                            {
                                if (input[index].name == IsDelimReserved(")"))
                                {
                                    logging(depth + currdepth + 5, ")");
                                    return true;
                                }
                                    
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
            logging(depth + currdepth + 4, "<declarations-list>");
            if (input[index].name == IsDelimReserved(";") && input[index-1].name == IsDelimReserved(")"))
            {
                return true;
            }
            if (input[index].name == ")")
            {
                logging(depth + currdepth + 2, ")");
                Empty(depth + currdepth + 5);//todo current depth
                return true;
            }
            if (IsDeclaration(input[index]))
            {
                index++;
                return IsDeclarationList(input[index]);
            }
            else
                return Empty(depth);
        }

        public static bool IsVarIdentifier(Word lecsem, int depthin)
        {
            logging(depth + currdepth + depthin, "<variable-identifier>");
            if (IsIdentifier(lecsem, depthin+1))
            {
                //logging(depth + currdepth + 8, lecsem.name);
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
            foreach (var node in log)
            {
                SetColor(ConsoleColor.DarkGray);
                Console.Write(GetDepth(node.Item1));
                SetColor(node.Item2);
                Console.Write(node.Item2 + '\n');
            }
        }

        public static void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void SetColor(string rule)
        {
            if (!Resource.rules.Contains(rule))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
                Console.ForegroundColor = ConsoleColor.DarkGray;
            if (rule.Contains("expected"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }
        public static string GetDepth(int depth)
        {
            var dep = "";
            for (int i = 0; i < depth; i++)
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