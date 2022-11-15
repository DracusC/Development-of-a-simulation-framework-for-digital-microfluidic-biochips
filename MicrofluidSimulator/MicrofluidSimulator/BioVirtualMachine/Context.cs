using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    /* 
     * The class Context groups all the informations of a current task. This includes InstructionMemory, DataMemory, DMFPlatform, ProgramCounter.
     */
    class Context
    {
        internal static int baseContextID = 0;
        internal ProgramCounter programCounter;
        internal int contextID;

        internal InstructionMemory instructionMemory;
        internal DataMemory dataMemory;
        internal IDMFPlatform dMFPlatform;

        internal List<int> electrodesToSet = new List<int>();
        internal List<int> electrodesToClear = new List<int>();
        internal Dictionary<int, int> devicesToWrite = new Dictionary<int, int>();

        public Context(InstructionMemory instructionMemory, DataMemory dataMemory, IDMFPlatform dMFPlatform, int startProgramCounter)
        {
            programCounter = new ProgramCounter(startProgramCounter);
            this.contextID = baseContextID;
            System.Threading.Interlocked.Increment(ref baseContextID);
            this.instructionMemory = instructionMemory;
            this.dataMemory = dataMemory;
            this.dMFPlatform = dMFPlatform;
        }

        public int getContextID()
        {
            return contextID;
        }

    }
}
