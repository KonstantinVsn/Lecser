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
        public static TreeNode<string> treeRoot = new TreeNode<string>("<signal-program>");
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
                //logging(depth + currdepth, "<program>");
                TreeNode<string> programNode = treeRoot.AddChild("<program>");
                //logging(depth + currdepth + 1, "PROGRAM");
                TreeNode<string> programINode = programNode.AddChild("PROGRAM");
                if (IsPocedireIdentifier(input[index], programINode))
                {
                    index++;
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        //logging(depth + currdepth + 2, input[index].name);
                        programINode.AddChild(input[index].name);
                        index++;
                        currdepth++;
                        if (IsBlock(input[index], input[index + 1], programINode))
                        {
                            if (input.Count > index + 2)
                            {
                                if (input[index + 2].name == IsDelimReserved("."))
                                {
                                    programINode.AddChild(input[index + 2].name);
                                    logging(depth + 2, input[index + 2].name);
                                    goto print;
                                }
                            }
                            else
                            {
                                Error(depth + 2, "(!) expected '.', but has end of file");
                                goto print;
                            }
                        }

                        if (IsProcedure(input[index], programINode))
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

            print:
            PrintTree();
            //PrettyPrint();
            return true;
        }

        public static bool IsBlock(Word begin, Word end, TreeNode<string> root)//not checking statement-list cource it is empty
        {
            if (begin.name == "BEGIN")
            {
                var blockNode = root.AddChild("<block>");
                var beginNode = blockNode.AddChild("BAGIN");
                index++;
                if (end.name == "END")
                {
                    var statmentNode = beginNode.AddChild("<statements-list>");
                    statmentNode.AddChild("<empty>");
                    blockNode.AddChild("END");
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

        public static bool IsProcedure(Word lecsem, TreeNode<string> root)
        {
            if (lecsem.name == "PROCEDURE")
            {
                var procedureNode = root.AddChild(lecsem.name);
                //logging(depth + currdepth + 1, lecsem.name);
                index++;
                if (IsPocedireIdentifier(input[index], procedureNode))
                {
                    index++;
                    if (IsParametrList(input[index], procedureNode))
                    {
                        if (input[index].name == IsDelimReserved(";"))
                        {
                            procedureNode.AddChild(";");
                            //logging(depth + currdepth + 1, input[index].name);
                            index++;
                            if (input.Count > index)
                            {
                                if (IsBlock(input[index], input[index + 1], procedureNode))
                                {
                                    if (input[index].name == IsDelimReserved(";"))
                                    {
                                        procedureNode.AddChild(";");
                                        //(depth + currdepth + 1, input[index].name);
                                        return true;
                                    }
                                }
                            }
                            else if (input.Count == index + 1)
                            {
                                Console.WriteLine();
                            }
                            else
                            {
                                procedureNode.AddChild("expected 'BEGIN', but has end of file");
                               // Error(depth + 3, "expected 'BEGIN', but has end of file");
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsPocedireIdentifier(Word lecsem, TreeNode<string> root)
        {
            TreeNode<string> programINode = root.AddChild("<procedure-identifier>");
            //logging(depth + currdepth + 2, "<procedure-identifier>");
             return IsIdentifier(lecsem, programINode) ? true: false;
            
        }

        public static string IsDelimReserved(string delim)
        {
            if (Resource.delimiters.Contains(delim[0]))
            {
                return delim;
            }
            return null;
        }

        public static bool IsParametrList(Word lecsem, TreeNode<string> root)
        {
            //logging(depth + currdepth + 2, "<parameters-list>");
            var parametrListNode = root.AddChild("<parameters-list>");
            if (input[index].name == IsDelimReserved("("))
            {
                index++;
                var startDelaration = parametrListNode.AddChild("(");
                //logging(depth + currdepth + 3, "(");


                if (IsDeclarationList(input[index], startDelaration))
                {
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        //parametrListNode.AddChild(";");
                        parametrListNode.AddChild(")");
                        //logging(depth + currdepth + 3, ";");
                        //if (IsBlock(input[index], input[index + 1])){
                        return true;
                        //}
                        
                        
                    }
                }
                else
                {
                    parametrListNode.AddChild("expected '<declaration-list>', but has " + input[index].name);
                    //Error(depth + 3, "expected '<declaration-list>', but has " + input[index].name);
                }
            }
            else
            {
                root.AddChild("expected '(', but has " + input[index].name);
                //Error(depth + 3, "expected '(', but has " + input[index].name);
            }

            return false;
        }
        public static bool IsIdentifier(Word lecsem, TreeNode<string> root)
        {
            TreeNode<string> programINode = root.AddChild("<identifier>");
            if (lecsem.code >= Resource.IDENTIFIERS_FROM && lecsem.code <= Resource.IDENTIFIERS_TO)
            {
                // logging(depth + currdepth + depthin + 3, "<identifier>");
                programINode.AddChild(lecsem.name);
                return true;
            }
            else
            {
                programINode.AddChild("expected '<identifier>', but has " + lecsem.name);
                return false;
            }
        }
        public static bool IsAttributeList(Word lecsem, TreeNode<string> root)
        {
            index++;
            var bufferNode = new TreeNode<string>("");
            var AttributeListNode = root.AddChild("<attributes-list>");
            //logging(depth + currdepth + 4, "<attributes-list>");
            if (IsAttribute(input[index], AttributeListNode))
            {
                logging(depth + currdepth + 5, input[index].name);
                if (IsAttributeList(input[index], AttributeListNode))
                    return true;
            }
            else if (input[index].name == IsDelimReserved(")") && IsAttribute(input[index - 1], bufferNode))
            {
                return true;
            }
            return false;
        }

        public static bool IsAttribute(Word lecsem, TreeNode<string> root)
        {
            var attributeNode = root.AddChild("<attribute>");
            //if (root.Children.Count == 1)
            //{
            //    attributeNode.AddChild("<empty>");
            //    return true;
            //}
            if (Resource.attributes.Contains(lecsem.name))
            {
                
                attributeNode.AddChild(lecsem.name);
                return true;
            }
            else if(lecsem.name == ")")
            {
                if (root.Level > 2)
                {
                    attributeNode.AddChild("<empty>");
                    return false;
                }
            }
            return false;
        }

        public static bool IsIdentifierList(Word lecsem, TreeNode<string> idenListNode)
        {
            index++;
            var bufferNode = new TreeNode<string>("");
            //logging(depth +currdepth +6, "<identifiers-list>");
            if (input[index].name == IsDelimReserved(","))
            {
                var startIdentifListNode = idenListNode.AddChild(",");
                //logging(depth + currdepth + 7, input[index].name);
                index++;
                if (IsVarIdentifier(input[index], idenListNode))
                {
                    //root.AddChild(input[index].name);
                    //logging(depth + currdepth + 8, input[index].name);
                    return IsIdentifierList(input[index], idenListNode);
                }
            }
            else if (input[index].name == IsDelimReserved(":") && !IsVarIdentifier(input[index - 1], bufferNode))
                return false;
            return true;
        }

        public static bool IsDeclaration(Word lecsem, TreeNode<string> root)
        {
            logging(depth + currdepth + 5, "<declaration>");
            var declarationNode = root.AddChild("<declaration>");
            if (IsVarIdentifier(input[index], declarationNode))
            {
                //logging(depth + currdepth + 8, input[index].name);
                var idenListNode = declarationNode.AddChild("<identifiers-list>");
                if (IsIdentifierList(input[index], idenListNode))//in fun index++;
                {
                    if (input[index].name == IsDelimReserved(":")) // next symbol :
                    {
                        //logging(depth + currdepth + 4, input[index].name);
                        declarationNode.AddChild(":");
                        index++;
                        if (IsAttribute(input[index], declarationNode))//in fun index++;
                        {
                            //logging(depth + currdepth + 5, input[index].name);
                            if (IsAttributeList(input[index], declarationNode))
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

                /*if (IsAttribute(lecsem))
                {
                    if (IsAttributeList(lecsem))
                    {

                    }
                    else
                        Empty(depth);
                }*/
            }
            return false;
        }

        public static bool IsDeclarationList(Word lecsem, TreeNode<string> root)
        {
            var declarationListNode = root.AddChild("<declarations-list>");
            //logging(depth + currdepth + 4, "<declarations-list>");
            if (input[index].name == IsDelimReserved(";") && input[index-1].name == IsDelimReserved(")"))
            {
                declarationListNode.AddChild("<empty>");
                return true;
            }
            if (input[index].name == ")")
            {
                root.AddChild(")");
                //logging(depth + currdepth + 2, ")");
                //Empty(depth + currdepth + 5);//todo current depth
                return true;
            }
            if (IsDeclaration(input[index], declarationListNode))
            {
                index++;
                return IsDeclarationList(input[index], root);
            }
            else
                return Empty(depth);
        }

        public static bool IsVarIdentifier(Word lecsem, TreeNode<string> root)
        {
            var varIdenNode = root.AddChild("<variable-identifier>");
            //logging(depth + currdepth + depthin, "<variable-identifier>");
            if (IsIdentifier(lecsem, varIdenNode))
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

        public static void PrintTree()
        {
            foreach (TreeNode<string> node in treeRoot)
            {
                string indent = CreateIndent(node.Level);
                SetColor(ConsoleColor.DarkGray);
                Console.Write(indent);
                SetColor(node.Data);
                Console.Write((node.Data ?? "null"));
                Console.Write('\n');
            }
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