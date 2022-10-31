using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class Barrier
    {
        private int count;
        private List<int> seenProgramCounters = new List<int>(); //List that contains the program counters of the tasks that released the barrier
        private List<int> traversedProgramCounters = new List<int>(); //List that contains the program counters of the tasks that traversed the barrier
        private bool released = false;

        // Constructor
        public Barrier(int count)
        {
            this.count = count;
        }

        internal void RegisterProgramCounter(int programCounter)
        {
            if (!seenProgramCounters.Contains(programCounter))
            {
                seenProgramCounters.Add(programCounter);
            }
        }

        internal void UpdateBarrier()
        {
            released = (seenProgramCounters.Count == count);
        }

        internal bool IsReleased()
        {
            return released;
        }

        internal void MarkTraversed(int programCounter)
        {
            if (!traversedProgramCounters.Contains(programCounter))
            {
                traversedProgramCounters.Add(programCounter);
            }
        }

        internal bool IsToBeRemoved()
        {
            return (traversedProgramCounters.Count == count);
        }
    }
}
