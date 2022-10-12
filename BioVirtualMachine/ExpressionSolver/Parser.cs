using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static NewExpressionSolver.Configuration;
using static NewExpressionSolver.Rules;

namespace NewExpressionSolver
{
    static class Parser
    {
        // Lists of operators names orderd from long to short
        static List<string> functionOperatorsNames = functionOperators.Keys.ToList().OrderByDescending(name => name.Length).ToList();
        static List<string> prefixOperatorsNames = prefixOperators.Keys.ToList().OrderByDescending(name => name.Length).ToList();
        static List<string> postfixOperatorsNames = postfixOperators.Keys.ToList().OrderByDescending(name => name.Length).ToList();
        static List<string> infixOperatorsNames = infixOperators.Keys.ToList().OrderByDescending(name => name.Length).ToList();

        static List<string> functionOperatorsPostambles = GeneratePostambles(functionOperatorsPostamblesTypes);
        static List<string> prefixOperatorsPostambles = GeneratePostambles(prefixOperatorsPostamblesTypes);
        static List<string> infixOperatorsPostambles = GeneratePostambles(infixOperatorsPostamblesTypes);
        static List<string> postfixOperatorsPostambles = GeneratePostambles(postfixOperatorsPostamblesTypes);
        static List<string> argumentSeparatorPostambles = GeneratePostambles(argumentSeparatorPostamblesTypes);
        static List<string> openParentesisPostambles = GeneratePostambles(openParentesisPostamblesTypes);
        static List<string> closeParentesisPostambles = GeneratePostambles(closeParentesisPostamblesTypes);


        static List<string> GeneratePostambles(List<TokenType> PostableTypes)
        {
            List<string> postambles = new List<string>();
            foreach (TokenType tokenType in PostableTypes)
            {
                switch (tokenType)
                {
                    case TokenType.OPERAND:
                        postambles.Add(binaryOrHexFirstDigit.ToString());
                        postambles.AddRange(decimalDigits.Select(c => c.ToString()).ToList());
                        break;
                    case TokenType.INFIX_OPERATOR:
                        postambles.AddRange(infixOperatorsNames);
                        break;
                    case TokenType.POSTFIX_OPERATOR:
                        postambles.AddRange(postfixOperatorsNames);
                        break;
                    case TokenType.PREFIX_OPERATOR:
                        postambles.AddRange(prefixOperatorsNames);
                        break;
                    case TokenType.FUNCTION_OPERATOR:
                        postambles.AddRange(functionOperatorsNames);
                        break;
                    case TokenType.ARGUMENT_SEPARATOR:
                        postambles.Add(argumentSeparator.ToString());
                        break;
                    case TokenType.OPEN_PARENTESIS:
                        postambles.Add(openParentesis.ToString());
                        break;
                    case TokenType.CLOSE_PARENTESIS:
                        postambles.Add(closeParentesis.ToString());
                        break;
                    default:
                        break;
                }
            }
            // Adding syntax spacer to postambles 
            List<string> temporary = postambles.Select(name => syntaxSpacer.ToString() + name).ToList();
            postambles.AddRange(temporary);
            // Ordering the preambles
            postambles = postambles.OrderByDescending(name => name.Length).ToList();
            return postambles;
        }


        public static List<Token> Parse<T>(string expression)
        {
            // Checking for parentesis correctness
            if (!Utilities.AreParentesisBalanced(expression))
            {
                string exceptionMessage = "Parentesis are not balanced.";
                throw new ParserException(exceptionMessage);
            }

            // Start parsing
            List<Token> tokens = new List<Token>();
            tokens.Add(new IdentityOperator());
            List<int> errorPositions = new List<int>();
            int i = 0;
            while (i < expression.Length)
            {
                bool tokenFound = false;
                ///// Checking for syntax spacers
                if (expression[i] == syntaxSpacer)
                {
                    tokenFound = true;
                    i++;
                }
                ///// Checking for argumentSeparator
                else if (expression[i] == argumentSeparator)
                {
                    foreach (TokenType preambleType in argumentSeparatorPreamblesTypes)
                    {
                        foreach (string postamble in argumentSeparatorPostambles)
                        {
                            if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", argumentSeparator.ToString(), postamble))
                            {
                                tokens.Add(new ArgumentSeparator());
                                tokenFound = true;
                                i++;
                                break;
                            }
                        }
                        if (tokenFound) break;
                    }
                }
                ///// Checking for openParentesis
                else if (expression[i] == openParentesis)
                {
                    foreach (TokenType preambleType in openParentesisPreamblesTypes)
                    {
                        foreach (string postamble in openParentesisPostambles)
                        {
                            if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", openParentesis.ToString(), postamble))
                            {
                                tokens.Add(new OpenParentesis());
                                tokenFound = true;
                                i++;
                                break;
                            }
                        }
                        if (tokenFound) break;
                    }
                }
                ///// Checking for closeParentesis
                else if (expression[i] == closeParentesis)
                {
                    foreach (TokenType preambleType in closeParentesisPreamblesTypes)
                    {
                        if (i == expression.Length - 1 && tokens[tokens.Count - 1].tokenType == preambleType)
                        {
                            tokens.Add(new CloseParentesis());
                            tokenFound = true;
                            i++;
                            break;
                        }
                        else
                        {
                            foreach (string postamble in closeParentesisPostambles)
                            {
                                if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", closeParentesis.ToString(), postamble))
                                {
                                    tokens.Add(new CloseParentesis());
                                    tokenFound = true;
                                    i++;
                                    break;
                                }
                            }
                            if (tokenFound) break;
                        }
                    }
                }
                ///// Checking for numbers
                else if (CharIsDecimalDigit(expression[i]))
                {
                    // Identify is the number is decimal, binary, or hexadecimal
                    bool isBinary = false;
                    bool isHexadecimal = false;
                    bool isDecimal = false;
                    if (i + 2 < expression.Length)
                    {
                        if (expression[i] == binaryOrHexFirstDigit && expression[i + 1] == hexadecimalSymbol && CharIsHexadecimalDigit(expression[i + 2])) //0x?
                        {
                            isHexadecimal = true;
                        }
                        else if (expression[i] == binaryOrHexFirstDigit && expression[i + 1] == binarySymbol && CharIsBinaryDigit(expression[i + 2]))
                        {
                            isBinary = true;
                        }
                        else
                        {
                            isDecimal = true;
                        }
                    }
                    else
                    {
                        isDecimal = true;
                    }

                    // Collecting the number
                    string number = "";
                    // Number is decimal
                    if (isDecimal)
                    {
                        while (CharIsDecimalDigit(expression[i]) || expression[i]==decimalPoint)
                        {
                            number += expression[i];
                            i++;
                        }
                        T value = Utilities.ConvertDecimalFromString<T>(number);
                        tokens.Add(new Operand<T>(value));
                    }
                    // Number is binary
                    if (isBinary)
                    {
                        i += 2; // Skip the 0b
                        while (CharIsBinaryDigit(expression[i]))
                        {
                            number += expression[i];
                            i++;
                        }
                        // TODO: Processing and saving
                        tokens.Add(new Operand<T>(number));
                    }
                    // Number is hex
                    if (isHexadecimal)
                    {
                        i += 2; // Skip the 0x
                        while (CharIsHexadecimalDigit(expression[i]))
                        {
                            number += expression[i];
                            i++;
                        }
                        // TODO: Processing and saving
                        tokens.Add(new Operand<T>(number));
                    }
                    
                    tokenFound = true;
                }
             ///// Checking for operators 
                else
                {
                    // Searching for function operators
                    foreach (string functionOperatorsName in functionOperatorsNames)
                    {
                        foreach (TokenType preambleType in functionOperatorsPreamblesTypes)
                        {
                            foreach (string postamble in functionOperatorsPostambles)
                            {
                                if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", functionOperatorsName, postamble))
                                {
                                    // A function operator has been found
                                    tokens.Add(functionOperators[functionOperatorsName]);
                                    tokenFound = true;
                                    i += functionOperatorsName.Length;
                                    break;
                                }
                            }
                            if (tokenFound) break;
                        }
                    }

                    // Searching for prefix operators
                    if (!tokenFound)
                    {
                        foreach (string prefixOperatorsName in prefixOperatorsNames)
                        {
                            foreach (TokenType preambleType in prefixOperatorsPreamblesTypes)
                            {
                                foreach (string postamble in prefixOperatorsPostambles)
                                {
                                    if (tokens[tokens.Count-1].tokenType == preambleType && StringContains(expression, i, "", prefixOperatorsName, postamble))
                                    {
                                        tokens.Add(prefixOperators[prefixOperatorsName]);
                                        tokenFound = true;
                                        i += prefixOperatorsName.Length;
                                        break;
                                    }
                                }
                                if (tokenFound) break;
                            }
                        }
                    }

                    // Searching for postfix operators
                    if (!tokenFound)
                    {
                        foreach (string postfixOperatorsName in postfixOperatorsNames)
                        {
                            foreach (TokenType preambleType in postfixOperatorsPreamblesTypes)
                            {
                                foreach (string postamble in postfixOperatorsPostambles)
                                {
                                    if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", postfixOperatorsName, postamble))
                                    {
                                        tokens.Add(postfixOperators[postfixOperatorsName]);
                                        tokenFound = true;
                                        i += postfixOperatorsName.Length;
                                        break;
                                    }
                                }
                                if (tokenFound) break;
                            }
                        }
                    }

                    // Searching for infix operators
                    if (!tokenFound)
                    {
                        foreach (string infixOperatorsName in infixOperatorsNames)
                        {
                            foreach (TokenType preambleType in infixOperatorsPreamblesTypes)
                            {
                                foreach (string postamble in infixOperatorsPostambles)
                                {
                                    if (tokens[tokens.Count - 1].tokenType == preambleType && StringContains(expression, i, "", infixOperatorsName, postamble))
                                    {
                                        tokens.Add(infixOperators[infixOperatorsName]);
                                        tokenFound = true;
                                        i += infixOperatorsName.Length;
                                        break;
                                    }
                                }
                                if (tokenFound) break;
                            }
                        }
                    }
                }
                // The search has failed
                if (!tokenFound)
                {
                    errorPositions.Add(i-1);
                    i++;
                }
            }
            // There are errors
            if (errorPositions.Count != 0)
            {
                string exceptionMessage = "Syntax error at position:";
                foreach (int errorPosition in errorPositions)
                {
                    exceptionMessage += " " + errorPosition.ToString();
                }
                exceptionMessage += ".";
                throw new ParserException(exceptionMessage, errorPositions);
            }
            return tokens;
        }

        private static bool CharIsSymbol(char character)
        {
            return character == openParentesis || character == closeParentesis || character == argumentSeparator;
        }


        private static bool CharIsHexadecimalDigit(char character)
        {
            return hexadecimalDigits.Contains(character);
        }


        private static bool CharIsBinaryDigit(char character)
        {
            return binaryDigits.Contains(character);
        }


        private static bool CharIsDecimalDigit(char character)
        {
            return decimalDigits.Contains(character);
        }


        private static bool CharIsLetter(char character)
        {
            return Char.IsLetter(character);
        }        

        private static bool StringContains(string testString, int startIndex, string preamble, string target, string postamble)
        {
            // Check is preamble + target + postable fits the testString
            if (startIndex - preamble.Length < 0 || startIndex + target.Length + postamble.Length > testString.Length || startIndex < 0)
            {
                return false;
            }
            string subTestString = testString.Substring(startIndex - preamble.Length, preamble.Length + target.Length + postamble.Length);
            return subTestString.Equals(preamble + target + postamble);
        }
    }
}
