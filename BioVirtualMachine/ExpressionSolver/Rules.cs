using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    /// <summary>
    /// Class that contains the syntax rules. For each token type, it specifies the token types that are allowed to preceed and follow the token itself.
    /// </summary>
    static class Rules
    {
        // SYMBOLS
        //static readonly internal char argumentSeparator = ',';
        //static readonly internal char openParentesis = '(';
        //static readonly internal char closeParentesis = ')';

        // Preambles and postambles for argumentSeparator 
        static readonly internal List<TokenType> argumentSeparatorPreamblesTypes = new List<TokenType>()
        {
            TokenType.CLOSE_PARENTESIS,
            TokenType.OPERAND
        };
        static readonly internal List<TokenType> argumentSeparatorPostamblesTypes = new List<TokenType>()
        {
            TokenType.FUNCTION_OPERATOR,
            TokenType.OPEN_PARENTESIS,
            TokenType.OPERAND,
            TokenType.PREFIX_OPERATOR
        };

        // Preambles and postambles for Open Parentesis 
        static readonly internal List<TokenType> openParentesisPreamblesTypes = new List<TokenType>()
        {
            TokenType.ARGUMENT_SEPARATOR,
            TokenType.FUNCTION_OPERATOR,
            TokenType.IDENTITY_OPERATOR,
            TokenType.OPEN_PARENTESIS,
            TokenType.OPERAND,
            TokenType.PREFIX_OPERATOR,
            TokenType.INFIX_OPERATOR
        };
        static readonly internal List<TokenType> openParentesisPostamblesTypes = new List<TokenType>()
        {
            TokenType.FUNCTION_OPERATOR,
            TokenType.OPEN_PARENTESIS,
            TokenType.OPERAND,
            TokenType.PREFIX_OPERATOR
        };

        // Preambles and postambles for Close Parentesis 
        static readonly internal List<TokenType> closeParentesisPreamblesTypes = new List<TokenType>()
        {
            TokenType.CLOSE_PARENTESIS,
            TokenType.OPERAND,
            TokenType.POSTFIX_OPERATOR
        };
        static readonly internal List<TokenType> closeParentesisPostamblesTypes = new List<TokenType>()
        {
            TokenType.ARGUMENT_SEPARATOR,
            TokenType.CLOSE_PARENTESIS,
            TokenType.INFIX_OPERATOR,
            TokenType.POSTFIX_OPERATOR
        };

        // Preambles and postambles for Function Operators 
        static readonly internal List<TokenType> functionOperatorsPreamblesTypes = new List<TokenType>()
        {
            TokenType.OPEN_PARENTESIS,
            TokenType.ARGUMENT_SEPARATOR,
            TokenType.INFIX_OPERATOR,
            TokenType.PREFIX_OPERATOR
        };
        static readonly internal List<TokenType> functionOperatorsPostamblesTypes = new List<TokenType>()
        {
        TokenType.OPEN_PARENTESIS
        };

        // Preambles and postambles for Prefix Operators
        static readonly internal List<TokenType> prefixOperatorsPreamblesTypes = new List<TokenType>()
        {
            TokenType.OPEN_PARENTESIS,
            TokenType.ARGUMENT_SEPARATOR,
            TokenType.INFIX_OPERATOR,
            TokenType.PREFIX_OPERATOR
        };
        static readonly internal List<TokenType> prefixOperatorsPostamblesTypes = new List<TokenType>()
        {
            TokenType.OPEN_PARENTESIS,
            TokenType.OPERAND,
            TokenType.PREFIX_OPERATOR,
            TokenType.FUNCTION_OPERATOR
        };

        // Preambles and postambles for Infix Operators
        static readonly internal List<TokenType> infixOperatorsPreamblesTypes = new List<TokenType>()
        {
            TokenType.CLOSE_PARENTESIS,
            TokenType.POSTFIX_OPERATOR,
            TokenType.OPERAND
        };
        static readonly internal List<TokenType> infixOperatorsPostamblesTypes = new List<TokenType>()
        {
             TokenType.OPEN_PARENTESIS,
             TokenType.OPERAND,
             TokenType.PREFIX_OPERATOR,
             TokenType.FUNCTION_OPERATOR
        };

        // Preambles and postambles for Postfix Operators
        static readonly internal List<TokenType> postfixOperatorsPreamblesTypes = new List<TokenType>()
        {
            TokenType.CLOSE_PARENTESIS,
            TokenType.OPERAND
        };
        static readonly internal List<TokenType> postfixOperatorsPostamblesTypes = new List<TokenType>()
        {
            TokenType.CLOSE_PARENTESIS,
            TokenType.INFIX_OPERATOR
        };
    }
}
