using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    /// <summary>
    /// This class holds the configuration of the Expression Solver
    /// New operators can be added here
    /// </summary>
    static class Configuration
    {
        // SYNTAX 
        static readonly internal char syntaxSpacer = ' ';

        // SYMBOLS
        static readonly internal char argumentSeparator = ',';
        static readonly internal char openParentesis = '(';
        static readonly internal char closeParentesis = ')';

        // NUMBERS
        static readonly internal char decimalPoint = '.';
        static readonly internal char hexadecimalSymbol = 'x';
        static readonly internal char binarySymbol = 'b';
        static readonly internal char binaryOrHexFirstDigit = '0';
        static readonly internal List<char> decimalDigits = new List<char>() {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        static readonly internal List<char> hexadecimalDigits = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        static readonly internal List<char> binaryDigits = new List<char>() { '0', '1' };


        /// <summary>
        /// List of infix operators. Example: 12+52 or 15/12 
        /// </summary>
        static readonly internal Dictionary<string, InfixOperator> infixOperators = new Dictionary<string, InfixOperator>()
        {
            {"+", new InfixOperator(Addition, 1, 2, "Addition")},
            {"-", new InfixOperator(Subtraction, 1, 2, "Subtraction")},
            {"*", new InfixOperator(Multiplication, 2, 2, "Multiplication")},
            {"/", new InfixOperator(Division, 2, 2, "Division")},
            {"%", new InfixOperator(Reminder, 2, 2, "Reminder")},
            {"^", new InfixOperator(Power, 3, 2, "Power")},
            //{"azz", new InfixOperator(Addition, 1, 2, "azz")}
        };


        /// <summary>
        /// List of funtion operators. Example: sin(12) or max(12,52) 
        /// </summary>
        static readonly internal Dictionary<string, FunctionOperator> functionOperators = new Dictionary<string, FunctionOperator>()
        {
            {"floor",  new FunctionOperator(Floor, 5, 1, "Floor")},
            {"ceil",  new FunctionOperator(Ceil, 5, 1, "Ceil")},
            {"abs",  new FunctionOperator(Absolute, 5, 1, "Absolute")},
            {"max",  new FunctionOperator(Maximum, 5, 2, "Maximum")},
            {"min",  new FunctionOperator(Minimum, 5, 2, "Minimum")}
        };


        /// <summary>
        /// List of prefix operators. Example: -12 or +15 
        /// </summary>
        static readonly internal Dictionary<string, PrefixOperator> prefixOperators = new Dictionary<string, PrefixOperator>()
        {
            {"+", new PrefixOperator(Positive, 4, 1, "Positive")},
            {"-", new PrefixOperator(Negative, 4, 1, "Negative")}
        };


        /// <summary>
        /// List of postfix operators. Example: 12!
        /// </summary>
        static readonly internal Dictionary<string, PostfixOperator> postfixOperators = new Dictionary<string, PostfixOperator>()
        {
            {"!", new PostfixOperator(Factorial, 6, 1, "Factorial")}
        };

        // INFIX
        internal static dynamic Power(Type type, List<dynamic> operands)
        {
            if (type == typeof(int))
            {
                return (int)Math.Pow((double)operands[0], (double)operands[1]);
            }
            else if (type == typeof(long))
            {
                return (long)Math.Pow((double)operands[0], (double)operands[1]);
            }
            else
            {
                return Math.Pow(operands[0], operands[1]);
            }
        }

        internal static dynamic Reminder(Type type, List<dynamic> operands)
        {
            return operands[0] % operands[1];
        }

        internal static dynamic Division(Type type, List<dynamic> operands)
        {
            return operands[0] / operands[1];
        }

        internal static dynamic Multiplication(Type type, List<dynamic> operands)
        {
            return operands[0] * operands[1];
        }

        internal static dynamic Subtraction(Type type, List<dynamic> operands)
        {
            return operands[0] - operands[1];
        }

        internal static dynamic Addition(Type type, List<dynamic> operands)
        {
            return operands[0] + operands[1];
        }

        //FUNCTION
        internal static dynamic Minimum(Type type, List<dynamic> operands)
        {
            return Math.Min(operands[0], operands[1]);
        }

        internal static dynamic Maximum(Type type, List<dynamic> operands)
        {
            return Math.Max(operands[0], operands[1]);
        }

        internal static dynamic Absolute(Type type, List<dynamic> operands)
        {
            return Math.Abs(operands[0]);
        }

        internal static dynamic Ceil(Type type, List<dynamic> operands)
        {
            if (type == typeof(int) || type == typeof(long))
            {
                return operands[0];
            }
            else
            {
                return Math.Ceiling(operands[0]);
            }
        }

        internal static dynamic Floor(Type type, List<dynamic> operands)
        {
            if (type == typeof(int) || type == typeof(long))
            {
                return operands[0];
            }
            else
            {
                return Math.Floor(operands[0]);
            }
        }

        //PREFIX
        internal static dynamic Negative(Type type, List<dynamic> operands)
        {
            return -operands[0];
        }

        internal static dynamic Positive(Type type, List<dynamic> operands)
        {
            return operands[0];
        }

        //POSTFIX
        internal static dynamic Factorial(Type type, List<dynamic> operands)
        {
            if ((type == typeof(int) || type == typeof(long)) && operands[0] >= 0)
            {
                if (operands[0] == 0)
                {
                    return 1;
                }
                else
                {
                    dynamic result = operands[0];
                    for (dynamic factor = operands[0] - 1;  factor > 1; factor--)
                    {
                        result = result * factor;
                        factor--;
                    }
                    return result;
                }
            }
            else
            {
                throw new SolverException("Factorial cannot be computed on negative or non integer numbers.");
            }
        }

    }
}
