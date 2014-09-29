using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebTestDsl.Scripting
{
    internal static class DSLParser
    {
        public static string[] ToCSharp(string[] scriptLines)
        {
            var csharpLines = new string[scriptLines.Length];
            var variables = new HashSet<string>{ "http" };
            for (int i = 0; i < scriptLines.Length; i++)
            {
                var scriptLine = scriptLines[i];
                if (scriptLine.StartsWith("#wl"))
                {
                    scriptLine = scriptLine.Replace("#wl", "console.write_line");
                }
                else if (scriptLine.StartsWith("#w"))
                {
                    scriptLine = scriptLine.Replace("#w", "console.write");
                }
                else if (scriptLine.StartsWith("##"))
                {
                    csharpLines[i] = scriptLine.Remove(0, 2);
                    continue;
                }
                var regex = new Regex("\"[^\"\r\n]*\"");
                var litStringMatches = regex.Matches(scriptLine);
                for(var mi = 0; mi < litStringMatches.Count; mi++)
                {
                    scriptLine = scriptLine.Replace(litStringMatches[mi].Value, "{" + mi + "}");    
                }

                scriptLine = scriptLine.ToLower();
                
                var csharpLine = string.Empty;
                var chunks = scriptLine.Split(' ');
                for (int j = 0; j < chunks.Length; j++)
                {
                    for (var mi = 0; mi < litStringMatches.Count; mi++)
                    {
                        chunks[j] = chunks[j].Replace("{" + mi + "}", litStringMatches[mi].Value);
                    }
                }

                for (int j = 0; j < chunks.Length; j++)
                {
                    if (!scriptLine.StartsWith("when", StringComparison.OrdinalIgnoreCase) && IsDslMethod(chunks[j])) //check for method call
                    {
                        //kind of crappy but assume a method can only be at the end of a line for now
                        csharpLine += ConvertMethod(chunks[j], chunks, j + 1, variables);
                        break;
                    }

                    if (chunks[j].Equals("var", StringComparison.OrdinalIgnoreCase))
                    {
                        if(j != 0)
                            throw new InvalidOperationException("var can only be at the beginning of a statement");

                        variables.Add(chunks[1]);
                        csharpLine = chunks[0] + " " + chunks[1] + " " + chunks[2];
                        if (chunks[3].Contains(".") && !chunks[3].Contains("\""))
                            csharpLine += ConvertMethod(chunks[3], chunks, 4, variables);
                        else
                        {
                            for (int k = 4; k < chunks.Length; k++)
                            {
                                csharpLine += chunks[k] + " ";
                            }
                        }

                        break;
                    }

                    bool isVariable;
                    if (IsDslMethod(chunks[j], variables, out isVariable) && isVariable)
                    {
                        chunks[j] = chunks[j].Split('.')[0] + "." + chunks[j].Split('.')[1].ToPascalCase();
                    }

                    csharpLine += chunks[j] + " ";
                }

                csharpLine = csharpLine
                    .Replace("when ", "if(")
                    .Replace(" then", ")")
                    .Replace("is", "==")
                    .Replace("begin", "{")
                    .Replace("end", "}");

                if (!csharpLine.StartsWith("if") && !csharpLine.StartsWith("{") && !csharpLine.StartsWith("}"))
                    csharpLine += ";";

                csharpLines[i] = csharpLine;
            }

            var newCharpLines = new List<string>(csharpLines);
            newCharpLines.Insert(0, "var http = Http.Create();"); //automatically create an instance of the Http object
            return newCharpLines.ToArray();
        }

        private static string ConvertMethod(string dslMethod, string[] chunks, int paramStart, HashSet<string> variables = null)
        {
            var firstPart = dslMethod.Split('.')[0];
            firstPart = variables != null && variables.Contains(firstPart) ? firstPart : firstPart.ToPascalCase();
            var result = firstPart + "." + dslMethod.Split('.')[1].ToPascalCase() + "("; //split the class and method name to format seperately
            for (int k = paramStart; k <= chunks.Length - 1; k++)
            {
                bool paramIsVariable;

                result += IsDslMethod(chunks[k], variables, out paramIsVariable) && paramIsVariable ? chunks[k].Split('.')[0] + "." + chunks[k].Split('.')[1].ToPascalCase() : chunks[k];
                if (k + 1 < chunks.Length)
                    result += ", ";
                else
                    result += ")";
            }

            return result;
        }

        private static string ToPascalCase(this string source)
        {
            return String.Join(String.Empty, source.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1)).ToArray());
        }

        private static bool IsDslMethod(string chunk, HashSet<string> variables, out bool isVariable)
        {
            var isMethod = chunk.Contains(".") && !chunk.Contains("\"");
            isVariable = variables != null && isMethod && variables.Contains(chunk.Split('.')[0]);

            return isMethod;
        }

        private static bool IsDslMethod(string chunk)
        {
            bool isVariable;
            return IsDslMethod(chunk, null, out isVariable);
        }
    }
}
