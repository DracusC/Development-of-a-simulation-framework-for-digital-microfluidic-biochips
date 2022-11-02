using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    partial class Engine
    {
        private int tickCounter = 0;
        private List<Context> contexts = new List<Context>(); // List of active contexts
        private List<Context> contextsToStop = new List<Context>();
        private List<Context> contextsToStart = new List<Context>();
        private List<int> electrodesToSet = new List<int>();
        private List<int> electrodesToClear = new List<int>();
        private Dictionary<int, int> devicesToWrite = new Dictionary<int, int>();
        private Dictionary<int, int> devicesToAsyncWrite = new Dictionary<int, int>();
        private Dictionary<int, int> devicesToAsyncRead = new Dictionary<int, int>();
        private Dictionary<int, int> devicesToAsyncReadTargetPointers = new Dictionary<int, int>();
        private Dictionary<int, Barrier> barriers = new Dictionary<int, Barrier>();
        bool clearAllElectrodes = false;

        private InstructionMemory instructionMemory;
        private DataMemory dataMemory;
        private IDMFPlatform dMFPlatform;

        VMConfiguration vMConfiguration;
        ILogger logger;

        private bool lastInstructionInTick;

        public Engine(VMConfiguration vMConfiguration, InstructionMemory instructionMemory, DataMemory dataMemory, IDMFPlatform dMFPlatform)
        {
            this.instructionMemory = instructionMemory;
            this.dataMemory = dataMemory;
            this.dMFPlatform = dMFPlatform;
            this.vMConfiguration = vMConfiguration;
            this.logger = new InactiveLogger();
            contexts.Add(new Context(instructionMemory, dataMemory, dMFPlatform, 0));
            PopulateExecuteTable();
        }

        public Engine(VMConfiguration vMConfiguration, InstructionMemory instructionMemory, DataMemory dataMemory, IDMFPlatform dMFPlatform, ILogger logger)
        {
            this.instructionMemory = instructionMemory;
            this.dataMemory = dataMemory;
            this.dMFPlatform = dMFPlatform;
            this.vMConfiguration = vMConfiguration;
            this.logger = logger;
            contexts.Add(new Context(instructionMemory, dataMemory, dMFPlatform, 0));
            PopulateExecuteTable();
        }

        /*
         * This fucntion needs to be called once every tick.
         * It manages the execution of the instructions. 
         * Updates to the electrodes happen at the end of the tick. 
         * Return true if there are still active contexts and false if no active contexts are left.;
         */
        public bool Execute()
        {
            logger.Info("----Executing tick: {0}-------------------------------", tickCounter);
            tickCounter++;

            clearAllElectrodes = false;
            
            // Execution all the active contexts (each context hosts a task)
            foreach (Context currentContext in contexts)
            {
                lastInstructionInTick = false; // gets to true when the TICK instruction is encountered
                logger.Info("Executing context: {0}", currentContext.getContextID());
                if (!currentContext.instructionMemory.IsAddressUsed(currentContext.programCounter.getProgramCounter()))
                {
                    // Reached the end of the program without the TSTOP instruction
                    
                    string exceptionMessage = "Reached end of program without TSTOP instruction." + "End at PC: " + currentContext.programCounter.getProgramCounter();
                    logger.Error(exceptionMessage);
                    throw new BioVirtualMachineException(exceptionMessage);
                }
                // Executing at least one instruction (not checking if the first one is synchronous
                Instruction currentInstruction = currentContext.instructionMemory.ReadAddress(currentContext.programCounter.getProgramCounter());
                logger.Info("Execute instruction: \nPC: {0} I: {1}", currentContext.programCounter.getProgramCounter(), currentInstruction.ToString());
                ExecuteInstruction(currentContext, currentInstruction);
                currentContext.programCounter.IncrementProgramCounter();

                // Executing intructions 
                if (!currentContext.instructionMemory.IsAddressUsed(currentContext.programCounter.getProgramCounter()))
                {
                    // Reached the end of the program
                    string exceptionMessage = "Reached end of program without TSTOP instruction." + "End at PC: " + currentContext.programCounter.getProgramCounter();
                    logger.Error(exceptionMessage);
                    throw new BioVirtualMachineException(exceptionMessage);
                }
                currentInstruction = currentContext.instructionMemory.ReadAddress(currentContext.programCounter.getProgramCounter());
                while (!lastInstructionInTick)
                {
                    logger.Info("Execute instruction: \nPC: {0} I: {1}", currentContext.programCounter.getProgramCounter(), currentInstruction.ToString());
                    ExecuteInstruction(currentContext, currentInstruction);
                    currentContext.programCounter.IncrementProgramCounter();
                    if (!lastInstructionInTick)
                    {
                        if (!currentContext.instructionMemory.IsAddressUsed(currentContext.programCounter.getProgramCounter()))
                        {
                            // Reached the end of the program
                            string exceptionMessage = "Reached end of program without TSTOP instruction." + "End at PC: " + currentContext.programCounter.getProgramCounter();
                            logger.Error(exceptionMessage);
                            throw new BioVirtualMachineException(exceptionMessage);
                        }
                        currentInstruction = currentContext.instructionMemory.ReadAddress(currentContext.programCounter.getProgramCounter());
                    }
                }
            }

            // Cleaning asynchronus read/write lists
            devicesToAsyncWrite.Clear();
            devicesToAsyncRead.Clear();
            devicesToAsyncReadTargetPointers.Clear();

            // Check the clearAllFlag
            if (clearAllElectrodes)
            {
                foreach (Context context in contexts)
                {
                    if(context.electrodesToSet.Count != 0 || context.electrodesToClear.Count != 0)
                    {
                        string exceptionMessage = "The instruction CLRALL is attempting to clear all electrodes in the same tick when other electrodes are indipendently cleared or set.";
                        throw new BioVirtualMachineException(exceptionMessage);
                    }
                }
                dMFPlatform.ClearAllElectrodes();
            }
            

            // Set and clear electrodes
            CheckAndMergeElectrodesLists();
            if (electrodesToSet.Count != 0 || electrodesToClear.Count != 0)
            {
                dMFPlatform.UpdateElectrodes(electrodesToClear, electrodesToSet);
                electrodesToSet.Clear();
                electrodesToClear.Clear();
            }

            // Write devices
            CheckAndMergeDevicesToWriteDictionary();
            if (devicesToWrite.Count != 0)
            {
                dMFPlatform.WriteDevices(devicesToWrite);
                devicesToWrite.Clear();
            }

            // Update the barriers list
            for (int i = barriers.Count - 1; i >= 0; i--)
            {
                KeyValuePair<int, Barrier> barrier = barriers.ElementAt(i);
                if (barrier.Value.IsToBeRemoved())
                {
                    barriers.Remove(barrier.Key);
                }
                else
                {
                    barrier.Value.UpdateBarrier();
                }
            }

            //Stopping and starting tasks
            foreach (Context contextToStop in contextsToStop)
            {
                contexts.Remove(contextToStop);
                if (!vMConfiguration.DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES)
                {
                    dataMemory.RemovePrivateDataMemory(contextToStop.getContextID()); //Remove this line to keep track of the mem of all contexts.
                }
            }
            contextsToStop.Clear();
            foreach (Context contextToStart in contextsToStart)
            {
                contexts.Add(contextToStart);
            }
            contextsToStart.Clear();

            // Waiting for the DMF plaftom to apply the changes
            dMFPlatform.WaitForDMFPlatform();

            // Are there more contexts?
            if (contexts.Count == 0)
            {
                logger.Info("------------------------------------------------------------");
                return false;
            }
            else
            {
                return true;
            }
            
        }

        /*
         * Support function that checks if different contexts attempt to modify the same electrodes and then merges all of them in one single list.
         * Check if there are conflicts between contexts: different contexts should not control the same electrode (not even if they both try to set it or clear it)
         */
        private void CheckAndMergeElectrodesLists()
        {
            for (int i = 0; i < contexts.Count - 1; i++)
            {
                for (int j = i + 1; j < contexts.Count; j++)
                {
                    List<int> commonElectrodes = contexts[i].electrodesToClear.Intersect(contexts[j].electrodesToClear).ToList();
                    if (commonElectrodes.Count != 0)
                    {
                        string exceptionMessage = "Context " + contexts[i].contextID + " and context " + contexts[j].contextID + " tried to clear the same electrode ID: ";
                        foreach (int electrode in commonElectrodes)
                        {
                            exceptionMessage = exceptionMessage + electrode.ToString() + " ";
                        }
                        throw new BioVirtualMachineException(exceptionMessage);
                    }

                    commonElectrodes = contexts[i].electrodesToSet.Intersect(contexts[j].electrodesToClear).ToList();
                    if (commonElectrodes.Count != 0)
                    {
                        string exceptionMessage = "Context " + contexts[i].contextID + " tried to set the electrode that context " + contexts[j].contextID + " tried to clear. Electrode ID: ";
                        foreach (int electrode in commonElectrodes)
                        {
                            exceptionMessage = exceptionMessage + electrode.ToString() + " ";
                        }
                        throw new BioVirtualMachineException(exceptionMessage);
                    }

                    commonElectrodes = contexts[i].electrodesToClear.Intersect(contexts[j].electrodesToSet).ToList();
                    if (commonElectrodes.Count != 0)
                    {
                        string exceptionMessage = "Context " + contexts[i].contextID + " tried to clear the electrode that context " + contexts[j].contextID + " tried to set. Electrode ID: ";
                        foreach (int electrode in commonElectrodes)
                        {
                            exceptionMessage = exceptionMessage + electrode.ToString() + " ";
                        }
                        throw new BioVirtualMachineException(exceptionMessage);
                    }

                    commonElectrodes = contexts[i].electrodesToSet.Intersect(contexts[j].electrodesToSet).ToList();
                    if (commonElectrodes.Count != 0)
                    {
                        string exceptionMessage = "Context " + contexts[i].contextID + " and context " + contexts[j].contextID + " tried to set the same electrode ID: ";
                        foreach (int electrode in commonElectrodes)
                        {
                            exceptionMessage = exceptionMessage + electrode.ToString() + " ";
                        }
                        throw new BioVirtualMachineException(exceptionMessage);
                    }
                }
            }
            foreach (Context context in contexts)
            {
                electrodesToClear.AddRange(context.electrodesToClear);
                context.electrodesToClear.Clear();
                electrodesToSet.AddRange(context.electrodesToSet);
                context.electrodesToSet.Clear();
            }

        }

        
        /*
         * Support function that checks if different contexts attempt to modify the same device address and then merges all of them in one single dictionary
         */
        private void CheckAndMergeDevicesToWriteDictionary()
        {
            // Check if there are conflicts between contexts: different contexts should write the same address (not even if they both try to write it to the same value)
            for (int i = 0; i < contexts.Count - 1; i++)
            {
                for (int j = i + 1; j < contexts.Count; j++)
                {
                    List<int> devicesAddresses_i = new List<int>(contexts[i].devicesToWrite.Keys);
                    List<int> devicesAddresses_j = new List<int>(contexts[j].devicesToWrite.Keys);
                    List<int> commonDevices = devicesAddresses_i.Intersect(devicesAddresses_j).ToList();
                    if (commonDevices.Count != 0)
                    {
                        string exceptionMessage = "Context " + contexts[i].contextID + " and context " + contexts[j].contextID + " tried to write the same device address: ";
                        foreach (int devicesAddress in commonDevices)
                        {
                            exceptionMessage = exceptionMessage + devicesAddress.ToString() + " ";
                        }
                        throw new BioVirtualMachineException(exceptionMessage);
                    }

                }
            }
            foreach (Context context in contexts)
            {
                foreach (KeyValuePair<int,int> entry in context.devicesToWrite)
                {
                    devicesToWrite.Add(entry.Key,entry.Value);
                }
                context.devicesToWrite.Clear();
            }
        }

        /*
         * Generating the instructions execution table
         */
        internal delegate void ISADelegate(Context context, Instruction instruction);
        private Dictionary<int, ISADelegate> executeTable = new Dictionary<int, ISADelegate>();

        void PopulateExecuteTable()
        {
            foreach (KeyValuePair<string, int> item in ISA.GetOpcodeTable())
            {
                executeTable.Add(item.Value, GenerateByName(this, item.Key));
            }
        }

        ISADelegate GenerateByName(object target, string methodName)
        {
            return (ISADelegate)Delegate.CreateDelegate(typeof(ISADelegate), target, methodName);
        }

        private void ExecuteInstruction(Context context, Instruction instruction)
        {
            executeTable[instruction.GetOpcode()].Invoke(context, instruction);
        }

        public void UpdateTickSize(decimal timeUpdate)
        {
            dMFPlatform.SetTickSize(timeUpdate);
        }

    }

    public class BioVirtualMachineException : Exception
    {
        public BioVirtualMachineException(string message) : base(message)
        {
        }
    }


}
