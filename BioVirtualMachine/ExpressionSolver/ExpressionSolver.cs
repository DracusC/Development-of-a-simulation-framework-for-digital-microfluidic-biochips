using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    static class ExpressionSolver
    {
        static private char decimalPoint = '.';
        static private char argumentSepaprator = ',';
        static private char openParentesis = '(';
        static private char closeParentesis = ')';
        static private List<char> implicitBinaryOperators = new List<char>(){'+', '-', '*', '/', '%', '^'}; // only symbols (at leat one in the list) 
        static private Dictionary<char, char> implicitUnaryOperators = new Dictionary<char, char>() // only simbols
        {
            {'-', 'n'},
            {'+', 'p'}
        };
        static private Dictionary<string, char> explicitOperators = new Dictionary<string, char>() // cannot contain symbols (only letters) and it has to be a string
        {
            {"floor", 'f'},
            {"ceil", 'c'},
            {"abs", 'a'},
            {"max", 'M'},
            {"min", 'm'},
            //{"tern", 't'}
        };

        // List of illegal patterns
        static private string[] illegalOperatorsPatterns = new string[] 
        {
            @".*?(?:\+|\-|\*|\/|\^|\%|\()(?:\*|\/|\^|\%|\)).*",
            @".*?(?:\))(?:\d|[a-zA-Z]).*",
            @".*?(?:\d)(?:\(|[a-zA-Z]).*",
            @".*?(?:[a-zA-Z])(?:\d|\+|\-|\*|\/|\^|\%|\)).*",
            @".*?[^0-9\)],[^0-9\+\-a-zA-Z].*",
            @".*?[^0-9]\.[^0-9].*"
        };

        static private Dictionary<char, int> operatorsCount = new Dictionary<char, int>()
        {
            {'+', 2},
            {'-', 2},
            {'*', 2},
            {'/', 2},
            {'%', 2},
            {'^', 2},
            {'n', 1},
            {'p', 1},
            {'f', 1},
            {'c', 1},
            {'a', 1},
            {'m', 2},
            {'M', 2},
            //{'t', 3},
            {'(', -1},
            {')', -1},
            {',', -1},
        };

        // Precedence of operators
        static private int Precedence(char oper)
        {
            switch (oper)
            {
                case '+': return 1;
                case '-': return 2;
                case '*': return 3;
                case '/': return 4;
                case '%': return 5;
                case '^': return 6;

                case 'a': return 8;
                case 'f': return 8;
                case 'c': return 8;
                case 'M': return 8;
                case 'm': return 8;
                //case 't': return 8;

                case 'n': return 7;
                case 'p': return 7;

            }
            return 0;
        }

        static private T ApplyOper<T>(List<T> operands, char oper)
        {
            List<dynamic> args = new List<dynamic>();
            foreach (T item in operands)
            {
                args.Add(item);
            }
            switch (oper)
            {
                case '+': return args[0] + args[1];
                case '-': return args[0] - args[1];
                case '*': return args[0] * args[1];
                case '/': return args[0] / args[1];
                case '%': return args[0] % args[1];
                case '^':
                    if (typeof(T) == typeof(int))
                    {
                        return (dynamic)(int)Math.Pow((double)args[0], (double)args[1]);
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        return (dynamic)(long)Math.Pow((double)args[0], (double)args[1]);
                    }
                    else
                    {
                        return (dynamic)Math.Pow(args[0], args[1]);
                    }
                        
                case 'm': return Math.Min(args[0], args[1]);
                case 'M': return Math.Max(args[0], args[1]);
                case 'n': return -args[0];
                case 'p': return args[0];
                case 'f': return (typeof(T) == typeof(int) || typeof(T) == typeof(long)) ? args[0] : Math.Floor(args[0]);
                case 'c': return (typeof(T) == typeof(int) || typeof(T) == typeof(long)) ? args[0] : Math.Ceiling(args[0]);
                case 'a': return Math.Abs(args[0]);
                //case 't': return args[0] / args[1] + args[2];
            }
            return default(T);
        }

        private struct Token<T>
        {
            public int type; // 0 is number, -1 is parentesis, other is # of operands for the operator
            public T value;
            public char oper;

            internal Token(int typeArg, T valueArg)
            {
                type = typeArg;
                value = valueArg;
                oper = ' ';
            }

            internal Token(int typeArg, char operArg)
            {
                type = typeArg;
                value = default(T);
                oper = operArg;
            }
        }


        // This function takes a postfix expression and retrurns a list of tokens (numbers or operators)
        static private List<Token<T>> ParseExpression<T>(string expression)
        {
            List<Token<T>> tokens = new List<Token<T>>();
            int i = 0;
            while (i < expression.Length)
            {
                string temp = "";
                if (char.IsDigit(expression[i]))
                {
                    // Found a number
                    if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
                    {
                        //Integer
                        while (i < expression.Length && char.IsDigit(expression[i]))
                        {
                            temp += expression[i];
                            i++;
                        }
                    }
                    else
                    {
                        //Double or float
                        while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i]==decimalPoint))//or is a .
                        {
                            temp += expression[i];
                            i++;
                        }
                        if(!Regex.IsMatch(temp, @"^\d+(\.\d+)?$"))
                        {
                            throw new ExpressionSolverException("Malformed expression: decimal point is misused.");
                        }
                    }
                    tokens.Add(new Token<T>(0, (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(temp))); //Convert.ToInt32(temp)));
                }
                else if (expression[i] == argumentSepaprator)
                {
                    tokens.Add(new Token<T>(operatorsCount[expression[i]], expression[i]));
                    i++;
                }
                else
                {
                    // Found an operator
                    tokens.Add(new Token<T>(operatorsCount[expression[i]], expression[i]));
                    i++;
                }
            }
            return tokens;
        }

        //
        static private string ResolveOperators<T>(string expression)
        {
            // Resolving explicitOperators
            string line = expression;
            foreach (var target in explicitOperators)
            {
                // Building the regex for int
                string oldStringRegex = @"(?<preamble>^";
                //(?<preamble>^|\(|\)|\+|\-|\*|\/|\%|\^)(?<unary>floor)(?<postamble>\()
                for (int i = 0; i < implicitBinaryOperators.Count; i++)
                {
                    oldStringRegex += @"|\" + implicitBinaryOperators[i];
                }
                oldStringRegex += @"|\" + openParentesis;
                oldStringRegex += @"|\" + argumentSepaprator;
                for (int i = 0; i < implicitUnaryOperators.Count; i++)
                {
                    oldStringRegex += @"|\" + implicitUnaryOperators.Keys.ToList()[i];
                }
                oldStringRegex += @")(?<operator>";
                oldStringRegex += target.Key;
                oldStringRegex += @")(?<postamble>\()";
                //Console.WriteLine(oldStringRegex);
                line = ReplaceString(line, oldStringRegex, target.Value.ToString());
                //Console.WriteLine(line);
            }
            // Resolving implicitUnaryOperators
            foreach (var target in implicitUnaryOperators)
            {
                // Building the regex for int
                string oldStringRegex = @"(?<preamble>^";
                //(?<preamble>^|\(|\)|\+|\-|\*|\/|\%|\^)(?<operator>\-)(?<postamble>\(|\d+|f)
                for (int i = 0; i < implicitBinaryOperators.Count; i++)
                {
                    oldStringRegex += @"|\" + implicitBinaryOperators[i];
                }
                oldStringRegex += @"|\" + openParentesis;
                oldStringRegex += @"|\" + argumentSepaprator;
                for (int i = 0; i < implicitUnaryOperators.Count; i++)
                {
                    oldStringRegex += @"|" + implicitUnaryOperators.Values.ToList()[i];
                }
                oldStringRegex += @")(?<operator>";
                oldStringRegex += @"\" + target.Key;
                oldStringRegex += @")(?<postamble>\d+";
                for (int i = 0; i < implicitUnaryOperators.Count; i++)
                {
                    oldStringRegex += @"|" + implicitUnaryOperators.Values.ToList()[i];
                }
                for (int i = 0; i < implicitUnaryOperators.Count; i++)
                {
                    oldStringRegex += @"|\" + implicitUnaryOperators.Keys.ToList()[i];
                }
                oldStringRegex += @"|\" + openParentesis;
                for (int i = 0; i < explicitOperators.Count; i++)
                {
                    oldStringRegex += @"|" + explicitOperators.Values.ToList()[i];
                }
                oldStringRegex += @")";
                //Console.WriteLine(oldStringRegex);
                line = ReplaceString(line, oldStringRegex, target.Value.ToString());
                //Console.WriteLine(line);
            }
            return line;
        }

        /*
         * This method replaces ALL occurrences of a regex match in bewteen the groups preamble and postamble
         */
        static private string ReplaceString(string line, string oldStringRegex, string newString)
        {
            Regex rgx = new Regex(oldStringRegex);
            string replacement = @"${preamble}" + newString + @"${postamble}";
            while (Regex.IsMatch(line, oldStringRegex))
            {
                line = rgx.Replace(line, replacement, 1);
            }
            return line;
        }


        static private void CheckSyntax<T>(string expression)
        {
            if (expression.Equals("+()"))
            {
                throw new ExpressionSolverException("Empity expression.");
            }
            // Check parentesis
            int parentesisCount = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if(expression[i] == closeParentesis)
                {
                    parentesisCount--;
                }
                if (expression[i] == openParentesis)
                {
                    parentesisCount++;
                }
                if (parentesisCount < 0)
                {
                    throw new ExpressionSolverException("Malformed expression: missing '('.");
                }
            }
            if (parentesisCount > 0)
            {
                throw new ExpressionSolverException("Malformed expression: missing ')'.");
            }

            // Check for illegal patterns
            foreach (string illegalPattern in illegalOperatorsPatterns)
            {
                if (Regex.IsMatch(expression, illegalPattern))
                {
                    //Console.WriteLine(illegalPattern);
                    throw new ExpressionSolverException("Malformed expression: illegal syntax.");
                }

            }

            // Removing explicit operators
            string bufferExpression = expression;
            for (int i = 0; i < explicitOperators.Count; i++)
            {
                while (bufferExpression.Contains(explicitOperators.Keys.ToList()[i] + openParentesis))
                {
                    bufferExpression = bufferExpression.Replace(explicitOperators.Keys.ToList()[i], "");
                }
            }
            for (int i = 0; i < implicitBinaryOperators.Count; i++)
            {
                while (bufferExpression.Contains(implicitBinaryOperators[i]))
                {
                    bufferExpression = bufferExpression.Replace(implicitBinaryOperators[i].ToString(), "");
                }
            }
            for (int i = 0; i < implicitUnaryOperators.Count; i++)
            {
                while (bufferExpression.Contains(implicitUnaryOperators.Keys.ToList()[i]))
                {
                    bufferExpression = bufferExpression.Replace(implicitUnaryOperators.Keys.ToList()[i].ToString(), "");
                }
            }
            while (bufferExpression.Contains(openParentesis) || bufferExpression.Contains(closeParentesis))
            {
                bufferExpression = bufferExpression.Replace(openParentesis.ToString(), "");
                bufferExpression = bufferExpression.Replace(closeParentesis.ToString(), "");
            }
            // At this point the expression should contain only numbers and ,
            if (typeof(T).Equals(typeof(int)))
            {
                //Int
                for (int i = 0; i < bufferExpression.Length; i++)
                {
                    if (!char.IsDigit(bufferExpression[i]) && bufferExpression[i] != argumentSepaprator)
                    {
                        throw new ExpressionSolverException("Malformed expression: unkown operator or decimal numbers are used in an integer expression.");
                    }
                }
            }
            else if (typeof(T).Equals(typeof(long)))
            {
                //Long
                for (int i = 0; i < bufferExpression.Length; i++)
                {
                    if (!char.IsDigit(bufferExpression[i]) && bufferExpression[i] != argumentSepaprator)
                    {
                        throw new ExpressionSolverException("Malformed expression: unkown operator or decimal numbers are used in a long integer expression.");
                    }
                }
            }
            else
            {
                //Double or float
                for (int i = 0; i < bufferExpression.Length; i++)
                {
                    if (!char.IsDigit(bufferExpression[i]) && bufferExpression[i] != argumentSepaprator && bufferExpression[i] != decimalPoint)
                    {
                        throw new ExpressionSolverException("Malformed expression: unkown operator.");
                    }
                }
            }

            
            // Check if the amount of arguments for explicit operators is correct
            foreach (var target in explicitOperators)
            {
                bufferExpression = expression;
                while (bufferExpression.Contains(target.Key + openParentesis))
                {
                    int startIndex = bufferExpression.IndexOf(target.Key + openParentesis) + target.Key.Length + 1; // index of the first carachted after target(
                    int endIndex = -1;
                    int separatorsCount = 0;
                    int indentationLevel = 0;
                    for (int i = startIndex; i < bufferExpression.Length; i++)
                    {
                        if (bufferExpression[i] == closeParentesis && indentationLevel == 0)
                        {
                            endIndex = i;
                            break;
                        }
                        else if (bufferExpression[i] == openParentesis)
                        {
                            indentationLevel++;
                        }
                        else if (bufferExpression[i] == closeParentesis)
                        {
                            indentationLevel--;
                        }
                        else if (bufferExpression[i] == argumentSepaprator && indentationLevel == 0)
                        {
                            separatorsCount++;
                        }
                    }
                    if (endIndex == -1)
                    {
                        throw new ExpressionSolverException("Malformed expression: non matching parentesis.");
                    }
                    if (operatorsCount[target.Value] != separatorsCount+1)
                    {
                        string exceptionMessage = "Malformed expression: operator " + target.Key +" expects " + operatorsCount[target.Value]  + " arguments.";
                        throw new ExpressionSolverException(exceptionMessage);
                    }
                    bufferExpression = bufferExpression.Substring(endIndex);
                    
                }
            }
            

        }


        static internal T Evaluate<T>(string expression)
        {
            // Type check
            if (!typeof(T).Equals(typeof(int)) && !typeof(T).Equals(typeof(long)) && !typeof(T).Equals(typeof(double)))
            {
                throw new ExpressionSolverException("Type not supported: only int, long, or double.");
            }

            string infixExpression = "+(" + expression + ")";

            CheckSyntax<T>(infixExpression);

            string resolvedInfixExpression = ResolveOperators<T>(infixExpression);

            //Console.WriteLine(infixExpression);
            //Console.WriteLine(resolvedInfixExpression);

            List<Token<T>> tokens = ParseExpression<T>(resolvedInfixExpression);
            List<Token<T>> output = new List<Token<T>>();
            Stack<Token<T>> stack = new Stack<Token<T>>(); // operands stack

            foreach (Token<T> token in tokens)
            {
                if (token.type == 0)
                {
                    // token is a number
                    output.Add(token);
                }
                else
                {
                    if (token.oper == openParentesis)
                    {
                        // token is open parentesis
                        stack.Push(token);
                    }
                    else if (token.oper == closeParentesis)
                    {
                        // token is closed parentesis
                        while (stack.Peek().oper != openParentesis)
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Pop(); // remove '('
                    }
                    else if (token.oper == argumentSepaprator)
                    {
                        // token is ,
                        while (stack.Peek().oper != openParentesis)
                        {
                            output.Add(stack.Pop());
                        }
                        //stack.Pop(); // do not remove '('
                    }
                    else if (stack.Count != 0 && Precedence(stack.Peek().oper) > Precedence(token.oper))
                    {
                        while (stack.Count != 0 && Precedence(stack.Peek().oper) > Precedence(token.oper))
                        {
                            // token has lower priority than top of queue
                            output.Add(stack.Pop());
                        }
                        stack.Push(token);
                    }
                    else
                    {
                        // token has same or higher priority than top of queue
                        stack.Push(token);
                    }

                }
            }
            // Push out all the tokens
            while (stack.Count != 0)
            {
                output.Add(stack.Pop());
            }

            int i = 0;
            stack.Clear();
            while (i < output.Count)
            {
                // stacking numbers
                while (output[i].type == 0)
                {
                    stack.Push(output[i]);
                    i++;
                }

                List<T> operands = new List<T>();
                for (int j = 0; j < output[i].type; j++)
                {
                    operands.Add(stack.Pop().value);
                }
                operands.Reverse();
                stack.Push(new Token<T>(0, ApplyOper(operands, output[i].oper)));

                i++;
            }
            return stack.Pop().value;
        }

        private struct TestStruct
        {
            public string test;
            public string expected;
            public char type;
            public bool exception;

            public TestStruct(string testArg, string expextedArg, char typeArg)
            {
                test = testArg;
                expected = expextedArg;
                type = typeArg;
                exception = false;
            }

            public TestStruct(string testArg, string expextedArg, char typeArg, bool exceptionArg)
            {
                test = testArg;
                expected = expextedArg;
                type = typeArg;
                exception = exceptionArg;
            }

        }


        // Testing functions, just run after every change. Everything should pass.

        // expression, expected result
        private static List<TestStruct> testVector = new List<TestStruct>()
        {
            //Exceptions
            new TestStruct("-(-10)", "Type error.", 'o', true),
            new TestStruct("-((-10)", "Parentesis.", 'i', true),
            new TestStruct("-(-10))", "Parentesis.", 'i', true),
            new TestStruct("()", "Parentesis.", 'i', true),
            new TestStruct("(())", "Parentesis.", 'i', true),
            new TestStruct("(", "Parentesis.", 'i', true),
            new TestStruct(")", "Parentesis.", 'i', true),
            new TestStruct("", "Empity.", 'i', true),

            //Unary operators and parentesis
            new TestStruct("10", "10", 'i'),
            new TestStruct("(10)", "10", 'i'),
            new TestStruct("((10))", "10", 'i'),
            new TestStruct("+10", "10", 'i'),
            new TestStruct("-10", "-10", 'i'),
            new TestStruct("--10", "10", 'i'),
            new TestStruct("---+10", "-10", 'i'),
            new TestStruct("-(-10)", "10", 'i'),
            new TestStruct("10", "10", 'd'),
            new TestStruct("(10)", "10", 'd'),
            new TestStruct("((10))", "10", 'd'),
            new TestStruct("+10", "10", 'd'),
            new TestStruct("-10", "-10", 'd'),
            new TestStruct("--10", "10", 'd'),
            new TestStruct("---+10", "-10", 'd'),
            new TestStruct("-(-10)", "10", 'd'),         
            
            //Implicit operators
            new TestStruct("10+15", "25", 'i'),
            new TestStruct("10-15", "-5", 'i'),
            new TestStruct("10/15", "0", 'i'),
            new TestStruct("100/20", "5", 'i'),
            new TestStruct("10^2", "100", 'i'),
            new TestStruct("10%3", "1", 'i'),
            new TestStruct("10+15", "25", 'l'),
            new TestStruct("10-15", "-5", 'l'),
            new TestStruct("10/15", "0", 'l'),
            new TestStruct("100/20", "5", 'l'),
            new TestStruct("10^2", "100", 'l'),
            new TestStruct("10%3", "1", 'l'),
            new TestStruct("10.0+15.1", "25.1", 'd'),
            new TestStruct("10.1-15.2", "-5.1", 'd'),
            new TestStruct("15/10", "1.5", 'd'),
            new TestStruct("100/40", "2.5", 'd'),
            new TestStruct("10.5^2", "110.25", 'd'),
           // new TestStruct("10.1%3", "1.1", 'd'),

            //Explicit operators
            new TestStruct("ceil(1)", "1", 'i'),
            new TestStruct("floor(2)", "2", 'i'),
            new TestStruct("abs(20)", "20", 'i'),
            new TestStruct("abs(-20)", "20", 'i'),
            new TestStruct("min(-1,5)", "-1", 'i'),
            new TestStruct("max(-1,5)", "5", 'i'),
            new TestStruct("ceil(1)", "1", 'l'),
            new TestStruct("floor(2)", "2", 'l'),
            new TestStruct("abs(20)", "20", 'l'),
            new TestStruct("abs(-20)", "20", 'l'),
            new TestStruct("min(-1,5)", "-1", 'l'),
            new TestStruct("max(-1,5)", "5", 'l'),
            new TestStruct("ceil(1.1)", "2", 'd'),
            new TestStruct("floor(2.1)", "2", 'd'),
            new TestStruct("abs(20.1)", "20.1", 'd'),
            new TestStruct("abs(-20.1)", "20.1", 'd'),
            new TestStruct("min(-1.1,5.1)", "-1.1", 'd'),
            new TestStruct("max(-1.1,5.1)", "5.1", 'd'),
            //new TestStruct("tern(25,5,1)", "6", 'i'),
            new TestStruct("min(-1.1,max(5.1,1.1))", "-1.1", 'd'),
            new TestStruct("min(max(5.1,1.1),-1.1)", "-1.1", 'd'),
            new TestStruct("min(max(-abs(5.1),-abs(-1.1)),max(5.1,1.1))", "-1.1", 'd'),

            new TestStruct("(min(max(-abs(4+2-1)+floor(0),-abs(-1*1)),max(20/4,1^1)))", "-1", 'i'),
            new TestStruct("(min(max(-abs(4+2-1)+floor(0),-abs(-1*1)),max(20/4,1^1)))", "-1", 'l'),
            new TestStruct("abs(25/-4)*25/4", "39.0625", 'd'),

            new TestStruct("2147483647+1", "2147483648", 'l'),

        };


        static internal void Test()
        {
            foreach (var test in testVector)
            {
                dynamic result;
                bool pass = false;
                Console.WriteLine("----------------------------------------------");
                Console.Write("Expression: ");
                Console.WriteLine(test.test);
                Console.Write("Expected: ");
                Console.WriteLine(test.expected);
                try
                {
                    if (test.type == 'i')
                    {
                        result = Evaluate<int>(test.test);
                        if (result == Convert.ToInt32(test.expected))
                        {
                            pass = true;
                        }
                    }
                    else if (test.type == 'l')
                    {
                        result = Evaluate<long>(test.test);
                        if (result == Convert.ToInt64(test.expected))
                        {
                            pass = true;
                        }
                    }
                    else if(test.type == 'd')
                    {
                        result = Evaluate<double>(test.test);
                        if (result == Convert.ToDouble(test.expected))
                        {
                            pass = true;
                        }
                    }
                    else
                    {
                        result = Evaluate<string>(test.test); // must generate exeption
                    }
                    Console.Write("Result: ");
                    Console.WriteLine(result);
                }
                catch (ExpressionSolverException ex)
                {
                    if (test.exception)
                    {
                        pass = true;
                    }
                    Console.Write("Exception: ");
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (pass)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("PASS");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAIL");
                        Console.ResetColor();
                        Console.WriteLine("Press Enter to continue the test.");
                        Console.ReadLine();
                    }
                }

            }
            Console.WriteLine("----------------------------------------------");
        }
    }

    public class ExpressionSolverException : Exception
    {
        public ExpressionSolverException(string message) : base(message)
        {
        }
    }
}
