using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    static class NumericSolver
    {
        static public List<Token> ReorderToPostfix(List<Token> expression)
        {
            List<Token> postfixExpression = new List<Token>();
            Stack<Token> operandsStack = new Stack<Token>(); // operands stack

            foreach (Token token in expression)
            {
                switch (token.tokenType)
                {
                    case TokenType.OPERAND:
                        postfixExpression.Add(token);
                        break;

                    case TokenType.OPEN_PARENTESIS:
                        operandsStack.Push(token);
                        break;

                    case TokenType.CLOSE_PARENTESIS:
                        // token is closed parentesis
                        while (operandsStack.Peek().tokenType != TokenType.OPEN_PARENTESIS)
                        {
                            postfixExpression.Add(operandsStack.Pop());
                        }
                        operandsStack.Pop(); // remove '('
                        break;

                    case TokenType.ARGUMENT_SEPARATOR:
                        // token is ,
                        while (operandsStack.Peek().tokenType != TokenType.OPEN_PARENTESIS)
                        {
                            postfixExpression.Add(operandsStack.Pop());
                        }
                        //stack.Pop(); // do not remove '('
                        break;

                    case TokenType.INFIX_OPERATOR:
                    case TokenType.PREFIX_OPERATOR:
                    case TokenType.POSTFIX_OPERATOR:
                    case TokenType.FUNCTION_OPERATOR:
                    case TokenType.IDENTITY_OPERATOR:

                        if (operandsStack.Count != 0 && operandsStack.Peek().group == Group.OPERATOR && ((Operator)operandsStack.Peek()).precedence >= ((Operator)token).precedence)
                        {
                            while (operandsStack.Count != 0 && operandsStack.Peek().group == Group.OPERATOR && ((Operator)operandsStack.Peek()).precedence >= ((Operator)token).precedence)
                            {
                                // token has lower priority than top of queue
                                postfixExpression.Add(operandsStack.Pop());
                            }
                            operandsStack.Push(token);
                        }
                        else
                        {
                            // token has same or higher priority than top of queue or the queue is empty
                            operandsStack.Push(token);
                        }
                        break;
                        
                    default:
                        // Unkown token, rise exception
                        break;
                }
            }
            // Push out all the tokens
            while (operandsStack.Count != 0)
            {
                postfixExpression.Add(operandsStack.Pop());
            }
            return postfixExpression;
        }


        static public T SolveNumericExpression<T>(List<Token> expression)
        {
            Stack<Token> stack = new Stack<Token>();
            int i = 0;
            while (i < expression.Count)
            {
                // stacking numbers
                while (expression[i].tokenType == TokenType.OPERAND) 
                {
                    stack.Push(expression[i]);
                    i++;
                }

                List<dynamic> operands = new List<dynamic>();
                for (int j = 0; j < ((Operator)expression[i]).operatorsCount; j++)
                {
                    operands.Add((dynamic)((Operand<T>)stack.Pop()).value);
                }
                operands.Reverse();
                T result = ((Operator)expression[i]).Apply(typeof(T) ,operands);
                stack.Push(new Operand<T>(result));

                i++;
            }
            return ((Operand<T>)stack.Pop()).value;
                
        }

    }

}
