using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewExpressionSolver
{
    public class SolverException : Exception
    {
        public SolverException(string message) : base(message)
        {
        }
    }

    public class ParserException : Exception
    {
        public List<int> errorPosition { get; } = new List<int>();

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, List<int> errorPosition) : base(message)
        {
            this.errorPosition = errorPosition;
        }
    }
}
