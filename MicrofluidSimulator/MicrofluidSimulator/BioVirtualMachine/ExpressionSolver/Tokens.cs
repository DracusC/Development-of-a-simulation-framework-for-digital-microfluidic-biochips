using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    public enum TokenType
    {
        OPERAND,
        INFIX_OPERATOR,
        POSTFIX_OPERATOR,
        PREFIX_OPERATOR,
        FUNCTION_OPERATOR,
        ARGUMENT_SEPARATOR,
        OPEN_PARENTESIS,
        CLOSE_PARENTESIS,
        IDENTITY_OPERATOR
    }

    public enum Group
    {
        OPERAND,
        OPERATOR,
        SYMBOL
    }

    class Token
    {
        internal TokenType tokenType;
        internal Group group;
    }

    class Operand<T> : Token
    {
        internal Type type;
        internal T value;
        internal string text;
        internal Operand(T value)
        {
            tokenType = TokenType.OPERAND;
            this.value = value;
            this.text = value.ToString();
            type = typeof(T);
        }
        internal Operand(string value)
        {
            tokenType = TokenType.OPERAND;
            group = Group.OPERAND;
            this.text = value;
        }

        public override string ToString()
        {
            string answer = "OPERAND - " + text;
            return answer;
        }
    }

    class Operator : Token
    {
        internal delegate dynamic ApplyDelegate(Type type, List<dynamic> operands);
        internal ApplyDelegate Apply;
        internal uint precedence { get; }
        internal uint operatorsCount { get; }
        internal string name { get; }

        internal Operator(ApplyDelegate ApplyFunction, uint precedence, uint operatorsCount, string name)
        {
            group = Group.OPERATOR;
            Apply = ApplyFunction;
            this.precedence = precedence;
            this.operatorsCount = operatorsCount;
            this.name = name;
        }
    }

    class InfixOperator : Operator
    {
        internal InfixOperator(ApplyDelegate ApplyFunction, uint precedence, uint operatorsCount, string name) : base(ApplyFunction, precedence, operatorsCount, name)
        {
            tokenType = TokenType.INFIX_OPERATOR;
        }

        public override string ToString()
        {
            string answer = "INFIX_OPERATOR - " + name;
            return answer;
        }
    }

    class PrefixOperator : Operator
    {
        internal PrefixOperator(ApplyDelegate ApplyFunction, uint precedence, uint operatorsCount, string name) : base(ApplyFunction, precedence, operatorsCount, name)
        {
            tokenType = TokenType.PREFIX_OPERATOR;
        }

        public override string ToString()
        {
            string answer = "PREFIX_OPERATOR - " + name;
            return answer;
        }
    }

    class PostfixOperator : Operator
    {
        internal PostfixOperator(ApplyDelegate ApplyFunction, uint precedence, uint operatorsCount, string name) : base(ApplyFunction, precedence, operatorsCount, name)
        {
            tokenType = TokenType.POSTFIX_OPERATOR;
        }

        public override string ToString()
        {
            string answer = "POSTFIX_OPERATOR - " + name;
            return answer;
        }
    }

    class FunctionOperator : Operator
    {
        internal FunctionOperator(ApplyDelegate ApplyFunction, uint precedence, uint operatorsCount, string name) : base(ApplyFunction, precedence, operatorsCount, name)
        {
            tokenType = TokenType.FUNCTION_OPERATOR;
        }

        public override string ToString()
        {
            string answer = "FUNCTION_OPERATOR - " + name;
            return answer;
        }
    }

    // This is needed for correct funtonality. It does not have a symbol.
    class IdentityOperator : Operator
    {
        internal IdentityOperator() : base(CopyValue, uint.MaxValue, 1, "")
        {
            tokenType = TokenType.IDENTITY_OPERATOR;
        }

        internal static dynamic CopyValue(Type type, List<dynamic> operands)
        {
            return operands[0];
        }

        public override string ToString()
        {
            string answer = "IDENTITY_OPERATOR";
            return answer;
        }
    }

    class ArgumentSeparator : Token
    {
        public override string ToString()
        {
            string answer = "ARGUMENT SEPARATOR";
            return answer;
        }

        internal ArgumentSeparator()
        {
            tokenType = TokenType.ARGUMENT_SEPARATOR;
            group = Group.SYMBOL;
        }
    }

    class OpenParentesis : Token
    {
        public override string ToString()
        {
            string answer = "OPEN PARENTESIS";
            return answer;
        }

        internal OpenParentesis()
        {
            tokenType = TokenType.OPEN_PARENTESIS;
            group = Group.SYMBOL;
        }
    }

    class CloseParentesis : Token
    {
        public override string ToString()
        {
            string answer = "CLOSE PARENTESIS";
            return answer;
        }

        internal CloseParentesis()
        {
            tokenType = TokenType.CLOSE_PARENTESIS;
            group = Group.SYMBOL;
        }
    }

}
