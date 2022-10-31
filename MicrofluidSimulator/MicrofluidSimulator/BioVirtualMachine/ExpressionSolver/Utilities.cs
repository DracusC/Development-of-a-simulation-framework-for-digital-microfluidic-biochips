using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static NewExpressionSolver.Configuration;

namespace NewExpressionSolver
{
    static class Utilities
    {
        static public T ConvertDecimalFromString<T>(string number)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(number);

        }

        /// <summary>
        /// Check if parentesis are balanced.
        /// </summary>
        /// <param name="expression">Input string</param>
        /// <returns>True if balanced, false otherwise</returns>
        static public bool AreParentesisBalanced(string expression)
        {
            int openParentesisCount = 0;
            foreach (char entry in expression)
            {
                if (entry == openParentesis)
                {
                    openParentesisCount++;
                }
                else if (entry == closeParentesis)
                {
                    openParentesisCount--;
                    if (openParentesisCount < 0)
                    {
                        return false;
                    }
                }
            }
            if (openParentesisCount == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Creates a string with the ^ simbol at the position indicated. Used to create an arrow pointing to an error in an expression.
        /// </summary>
        /// <param name="errorPositions">Index of the string where error occoured.</param>
        /// <returns>Output string</returns>
        static public string GetErrorPositionArrow(List<int> errorPositions)
        {
            if (errorPositions.Count == 0)
            {
                return "";
            }
            else
            {
                string answer = "";
                for (int i = 0; i <= errorPositions.Max(); i++)
                {
                    if (errorPositions.Contains(i))
                    {
                        answer += "^";
                    }
                    else
                    {
                        answer += " ";
                    }
                }
                return answer;
            }
        }

        /// <summary>
        /// Wraps the expression with parentesis. This must be done on the input expression before parsing.
        /// The purpose is to avoid a lot of string boundary checks.
        /// </summary>
        /// <param name="expression">Input string</param>
        /// <returns>Output string</returns>
        static public string WrapExpression(string expression)
        {
            return openParentesis + expression + closeParentesis;
        }

        /// <summary>
        /// Reduces multiple subsequent occurences of the 'syntax spacer' charachter to a single one.
        /// </summary>
        /// <param name="expression">Input string</param>
        /// <returns>Output string</returns>
        static public string CollapseSyntaxSpacers(string expression)
        {
            string answer = "";
            bool lastWasSpacer = false;
            foreach (char character in expression)
            {
                bool currentIsSpacer = character == syntaxSpacer;
                if (!lastWasSpacer && !currentIsSpacer)
                {
                    answer += character.ToString();
                }
                else if(!lastWasSpacer && currentIsSpacer)
                {
                    answer += character.ToString();
                    lastWasSpacer = true;
                }
                else if (lastWasSpacer && !currentIsSpacer)
                {
                    answer += character.ToString();
                    lastWasSpacer = false;
                }
            }
            return answer;
        }
    }
}
