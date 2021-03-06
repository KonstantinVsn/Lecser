﻿using System;
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
        public static List<Word> input;
        public static List<Tuple<int, string>> log = new List<Tuple<int, string>>();
        public SyntaxAnalyze(List<Word> _input)
        {
            input = _input;
        }

        public bool Analyze()
        {
            logging(depth, "<signal-program>");
            TreeNode<string> programNode = treeRoot.AddChild("<program>");
            var isProgram = false;
            if (input[index].name == "PROGRAM")
            {
                index++;
                isProgram = true;
                TreeNode<string> programINode = programNode.AddChild(input[index].name + " - " + input[index].code);
                if (IsPocedireIdentifier(input[index], programINode))
                {
                    index++;
                    if (input[index].name == IsDelimReserved(";"))
                    {
                        programNode.AddChild(input[index].name + " - " + input[index].code);
                        index++;
                        if (input[index].name != "BEGIN" )
                        {
                            programNode.AddChild("(!) expected 'BEGIN', but has " + input[index].name);
                            goto print;
                        }
                        if (IsBlock(input[index], input[index + 1], programNode))
                        {
                                if (input[index].name == IsDelimReserved("."))
                                {
                                programNode.AddChild(input[index].name + " - " + input[index].code);
                                    //logging(depth + 2, input[index + 2].name);
                                    goto print;
                                }
                            else
                            {
                                programINode.AddChild("(!) expected '.', but has end of file");
                                goto print;
                            }
                        }
                    }
                    else
                        programNode.AddChild("(!) expected ';', but has " + input[index + 2]);
                }
                else
                    goto print;
            }
            else if(input[index].name == "PROCEDURE" && !isProgram)
            {
                TreeNode<string> programINode = programNode.AddChild("PROCEDURE");
                if (IsProcedure(input[index], programINode))
                {
                    goto print;
                }
            }
            else 
                programNode.AddChild("(!) expected 'BEGIN' , but has" + input[index].name);

            print:
            PrintTree();
             
            return true;
        }

        public static bool IsBlock(Word begin, Word end, TreeNode<string> root)
        {
            if (!Resource.reservedWords.Contains(begin.name))
            {
                root.AddChild("(!) expected 'BEGIN OR PROCEDURE', but has " + "'" + begin.name + "'");
                return false;
            }
            var blockNode = root.AddChild("<block>");

            if (begin.name == "PROCEDURE")
            {
                return false;
            }
            if (begin.name == "BEGIN")
            {
                var beginNode = blockNode.AddChild("BEGIN" + " - " + begin.code);
                index++;
                if (end.name == "END")
                {
                    var statmentNode = beginNode.AddChild("<statements-list>");
                    statmentNode.AddChild("<empty>");
                    blockNode.AddChild("END"+" - " + end.code);
                    index++;
                }
                else
                    blockNode.AddChild("(!) expected 'END', but has " + begin.name+" - " + begin.code);
                return true;
            }
            if (begin.name == "END")
            {
                blockNode.AddChild("(!) expected 'BEGIN', but has " + begin.name+" - " + begin.code);
                return false;
            }
            return false;
        }

        public static bool IsProcedure(Word lecsem, TreeNode<string> procedureNode)
        {
            if (lecsem.name == "PROCEDURE")
            {
               // var procedureNode = root.AddChild(lecsem.name);
                 
                index++;
                if (IsPocedireIdentifier(input[index], procedureNode))
                {
                    index++;
                    if (IsParametrList(input[index], procedureNode))
                    {
                        if (input[index].name == IsDelimReserved(";"))
                        {
                            procedureNode.AddChild(";"+" - "+ input[index].code);


                            index++;
                            if (input.Count > index)
                            {
                                if (IsBlock(input[index], input[index + 1], procedureNode))
                                {
                                    if(input.Count == index)
                                    {
                                        procedureNode.AddChild("expected ';', but has end of file");
                                        return true;
                                    }
                                    if (input[index].name == IsDelimReserved(";"))
                                    {
                                        procedureNode.AddChild(";" +" - "+ input[index].code);
                                         
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
                                
                            }
                        }
                        else
                            procedureNode.AddChild("expected ';', but has " + input[index].name);
                    }
                }
            }
            return false;
        }

        public static bool IsPocedireIdentifier(Word lecsem, TreeNode<string> root)
        {
            TreeNode<string> programINode = root.AddChild("<procedure-identifier>");
             
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
            var parametrListNode = root.AddChild("<parameters-list>");
            if(input[index].name != "(" && input[index+1].name != ")" && input[index+2].name != ";")
                {
                    parametrListNode.AddChild("<empty>");
                    return true;
                }
            if (input[index].name == IsDelimReserved("("))
            {
                index++;
                var startDelaration = parametrListNode.AddChild("(" + " - " + input[index].code);
                 
                if (IsDeclarationList(input[index], startDelaration))
                {
                    if (input[index].name == IsDelimReserved(")"))
                    {
                        index++;
                        parametrListNode.AddChild(")" + " - " + input[index].code);
                        return true;                 
                    }
                }
                else
                {
                    parametrListNode.AddChild("expected '<declaration-list>', but has " + input[index].name + " - " + input[index].code);
                }
            }
            else
            {
                root.AddChild("expected '(', but has " + input[index].name + " - " + input[index].code);
            }

            return false;
        }
        public static bool IsIdentifier(Word lecsem, TreeNode<string> root)
        {
            TreeNode<string> programINode = root.AddChild("<identifier>");
            if (lecsem.code >= Resource.IDENTIFIERS_FROM && lecsem.code <= Resource.IDENTIFIERS_TO)
            {
                 
                programINode.AddChild(lecsem.name + " - " + lecsem.code);
                return true;
            }
            else
            {
                programINode.AddChild("expected '<identifier>', but has " + lecsem.name + " - " + lecsem.code);
                return false;
            }
        }

        public static bool IsAttributeList(Word lecsem, TreeNode<string> root)
        {
            index++;
            var bufferNode = new TreeNode<string>("");
            var AttributeListNode = root.AddChild("<attributes-list>");
             
            if (IsAttribute(input[index], AttributeListNode))
            {
                if (IsAttributeList(input[index], AttributeListNode))
                    return true;
            }
            else if (input[index].name == IsDelimReserved(";") && IsAttribute(input[index - 1], bufferNode))
            {
                return true;
            }
            return false;
        }

        public static bool IsAttribute(Word lecsem, TreeNode<string> root)
        {
            var attributeNode = root.AddChild("<attribute>");
 
            if (Resource.attributes.Contains(lecsem.name))
            {
                
                attributeNode.AddChild(lecsem.name + " - " + lecsem.code);
                return true;
            }
            else if(lecsem.name == ";")
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
             
            if (input[index].name == IsDelimReserved(","))
            {
                var startIdentifListNode = idenListNode.AddChild("," + " - " + input[index].code);
                 
                index++;
                if (IsVarIdentifier(input[index], idenListNode))
                {
                    return IsIdentifierList(input[index], idenListNode);
                }
            }
            else if (input[index].name == IsDelimReserved(":") && !IsVarIdentifier(input[index - 1], bufferNode))
                return false;
            return true;
        }

        public static bool IsDeclaration(Word lecsem, TreeNode<string> root)
        {
            var bufferNode = new TreeNode<string>("");
            var declarationNode = root.AddChild("<declaration>");
            if (IsVarIdentifier(input[index], declarationNode))
            {
                var idenListNode = declarationNode.AddChild("<identifiers-list>");
                if (IsIdentifierList(input[index], idenListNode)) 
                {
                    if(IsIdentifier(input[index], bufferNode) == IsIdentifier(input[index -1], bufferNode))
                    {
                        idenListNode.AddChild("expected ',', but has " + input[index].name + " - " + input[index].code);
                    }
                    if (input[index].name == IsDelimReserved(":"))  
                    {
                        declarationNode.AddChild(":" + " - " + input[index].code);
                        index++;
                        if (IsAttribute(input[index], declarationNode)) 
                        {
                             
                            if (IsAttributeList(input[index], declarationNode))
                            {
                                if (input[index].name == IsDelimReserved(";"))
                                {
                                    declarationNode.AddChild(input[index].name + " - " + input[index].code);
                                    return true;
                                }
                                else
                                    declarationNode.AddChild("expected ';', but has " + input[index].name + " - " + input[index].code);

                            }
                        }
                    }
                    else
                        idenListNode.AddChild("expected ':', but has " + input[index].name);
                }
            }
            return false;
        }

        public static bool IsDeclarationList(Word lecsem, TreeNode<string> root)
        {
            var declarationListNode = root.AddChild("<declarations-list>");
             
            if (input[index].name == IsDelimReserved(";") && input[index-1].name == IsDelimReserved(")"))
            {
                declarationListNode.AddChild("<empty>");
                return true;
            }
            if (input[index].name == ")")
            {
                root.AddChild(")" + " - " + input[index].code);
                return true;
            }
            if (IsDeclaration(input[index], declarationListNode))
            {
                index++;
                if(input[index].name == IsDelimReserved(")"))
                {
                    //declarationListNode.AddChild(input[index].name);
                    return true;
                }
                return IsDeclarationList(input[index], declarationListNode);
            }
            else
                return true;
        }

        public static bool IsVarIdentifier(Word lecsem, TreeNode<string> root)
        {
            var varIdenNode = root.AddChild("<variable-identifier>");
             
            if (IsIdentifier(lecsem, varIdenNode))
            {
                 
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
                Console.ForegroundColor = ConsoleColor.Green;
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

    }

    interface ISyntaxAnalyze
    {
        bool Analyze();
    }
}