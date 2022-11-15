using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    class NewExpressionSolver
    {

        public static void Test()
        {
            string expression = "(min(max(-abs(5+2-1!)+floor(0),-abs(-1*1)),max(20/ 4,1^1)))";
            //string expression = "(3+2)";
            try
            {
                EvaluateNumericExpression<int>(expression);
                Console.WriteLine(expression);
                Console.WriteLine();
            }
            catch (ParserException ex)
            {
                if (ex.errorPosition.Count != 0)
                {
                    Console.WriteLine("ParserException: " + ex.Message);
                    Console.WriteLine(expression);
                    Console.WriteLine(Utilities.GetErrorPositionArrow(ex.errorPosition));
                }
                else
                {
                    Console.WriteLine("ParserException: " + ex.Message);
                }
            }

        }


        static public T EvaluateNumericExpression<T>(string expression)
        {
            // Type check
            if (!typeof(T).Equals(typeof(int)) && !typeof(T).Equals(typeof(long)) && !typeof(T).Equals(typeof(double)))
            {
                throw new SolverException("Type not supported: only int, long, or double.");
            }

            expression = Utilities.CollapseSyntaxSpacers(expression);
            expression = Utilities.WrapExpression(expression);

            //CheckSyntax<T>(expression);

            List<Token> tokens = Parser.Parse<T>(expression);

            foreach (var item in tokens)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine();

            tokens = NumericSolver.ReorderToPostfix(tokens);

            foreach (var item in tokens)
            {
                Console.WriteLine(item.ToString());
            }

            T res = NumericSolver.SolveNumericExpression<T>(tokens);
            Console.WriteLine(res);

            return default(T);
        }
    }
}
