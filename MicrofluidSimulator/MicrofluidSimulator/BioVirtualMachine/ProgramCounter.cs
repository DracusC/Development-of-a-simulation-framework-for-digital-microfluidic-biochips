using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class ProgramCounter
    {
        private int programCounter;

        public ProgramCounter()
        {
            programCounter = 0;
        }

        public ProgramCounter(int initialValue)
        {
            programCounter = initialValue;
        }

        public int getProgramCounter()
        {
            return programCounter;
        }

        public void setProgramCounter(int value)
        {
            programCounter = value;
        }

        public void IncrementProgramCounter()
        {
            programCounter = programCounter + 1;
        }
    }
}
