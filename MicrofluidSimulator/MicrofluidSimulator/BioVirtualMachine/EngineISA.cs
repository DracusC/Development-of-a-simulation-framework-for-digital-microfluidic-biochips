using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BioVirtualMachine
{
    partial class Engine
    {
        //-----------------------------------------------------------------------------------
        // Implementation of the single instructions
        //-----------------------------------------------------------------------------------

       void SETELI(Context context, Instruction instruction)
        {
            int electrode = instruction.GetOperand(ISA.SETELI.ELECTRODE_IMMEDIATE);
            if (!context.electrodesToSet.Contains(electrode))
            {
                context.electrodesToSet.Add(electrode);
            }
            if (context.electrodesToClear.Contains(electrode))
            {
                context.electrodesToClear.Remove(electrode);
            }
        }


        void CLRELI(Context context, Instruction instruction)
        {
            int electrode = instruction.GetOperand(ISA.CLRELI.ELECTRODE_IMMEDIATE);
            if (!context.electrodesToClear.Contains(electrode))
            {
                context.electrodesToClear.Add(electrode);
            }
            if (context.electrodesToSet.Contains(electrode))
            {
                context.electrodesToSet.Remove(electrode);
            }
        }


        void SETEL(Context context, Instruction instruction)
        {
            int electrode = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.CLREL.ELECTRODE_POINTER), context.getContextID());
            if (!context.electrodesToSet.Contains(electrode))
            {
                context.electrodesToSet.Add(electrode);
            }
            if (context.electrodesToClear.Contains(electrode))
            {
                context.electrodesToClear.Remove(electrode);
            }
        }


        void CLREL(Context context, Instruction instruction)
        {
            int electrode = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.CLREL.ELECTRODE_POINTER), context.getContextID());
            if (!context.electrodesToClear.Contains(electrode))
            {
                context.electrodesToClear.Add(electrode);
            }
            if (context.electrodesToSet.Contains(electrode))
            {
                context.electrodesToSet.Remove(electrode);
            }
        }


        void CLRALL(Context context, Instruction instruction)
        {
            clearAllElectrodes = true;
        }


        void ADEVRD(Context context, Instruction instruction) // Asyncronous device read
        {
            int deviceAddress = instruction.GetOperand(ISA.ADEVRD.DEVICE_ADDRESS_IMMEDIATE);
            int targetPointer = instruction.GetOperand(ISA.ADEVRD.TARGET_POINTER);
            if (devicesToAsyncRead.ContainsKey(deviceAddress))
            {
                devicesToAsyncRead.Remove(deviceAddress);
                devicesToAsyncReadTargetPointers.Remove(deviceAddress);
            }
            devicesToAsyncRead.Add(deviceAddress, 0);
            devicesToAsyncReadTargetPointers.Add(deviceAddress, targetPointer);
        }


        void ADEVWR(Context context, Instruction instruction) // Asyncronous device write
        {
            int deviceAddress = instruction.GetOperand(ISA.ADEVWR.DEVICE_ADDRESS_IMMEDIATE);
            int writeData = instruction.GetOperand(ISA.ADEVWR.SOURCE_POINTER);
            if (devicesToAsyncWrite.ContainsKey(deviceAddress))
            {
                devicesToAsyncWrite.Remove(deviceAddress);
            }
            devicesToAsyncWrite.Add(deviceAddress, writeData);
        }

        void ADEVEX(Context context, Instruction instruction) // Asyncronous device write
        {
            context.dMFPlatform.AsynchronousReadWriteDevices(ref devicesToAsyncRead, devicesToAsyncWrite);
            devicesToAsyncWrite.Clear();
            //Update memory
            foreach (KeyValuePair<int,int> device in devicesToAsyncRead)
            {
                int deviceAddress = device.Key;
                int readValue = device.Value;
                int targetPointer = devicesToAsyncReadTargetPointers[deviceAddress];
                context.dataMemory.WriteAddress(targetPointer, readValue, context.getContextID());
            }
            devicesToAsyncRead.Clear();
            devicesToAsyncReadTargetPointers.Clear();
        }

        void ADEVCL(Context context, Instruction instruction)
        {
            devicesToAsyncWrite.Clear();
            devicesToAsyncRead.Clear();
            devicesToAsyncReadTargetPointers.Clear();
        }

        void DEVWR(Context context, Instruction instruction)
        {
            int deviceAddress = instruction.GetOperand(ISA.DEVWR.DEVICE_ADDRESS_IMMEDIATE);
            int writeData = instruction.GetOperand(ISA.DEVWR.SOURCE_POINTER);
            if (context.devicesToWrite.ContainsKey(deviceAddress))
            {
                context.devicesToWrite.Remove(deviceAddress);
            }
            context.devicesToWrite.Add(deviceAddress, writeData);
        }


        void TSTART(Context context, Instruction instruction)
        {
            contextsToStart.Add(new Context(instructionMemory, dataMemory, dMFPlatform, instruction.GetOperand(ISA.TSTART.TARGET_PROGRAM_COUNTER)));
        }


        void TSTOP(Context context, Instruction instruction)
        {
            contextsToStop.Add(context);
        }


        void TICK(Context context, Instruction instruction)
        {
            lastInstructionInTick = true;
        }


        void BARR(Context context, Instruction instruction)
        {
            int barrierID = instruction.GetOperand(ISA.BARR.BARRIER_ID);
            // If barried does not exist, create barrier
            if (!barriers.ContainsKey(barrierID))
            {
                int barrierCount = instruction.GetOperand(ISA.BARR.BARRIER_COUNT);
                barriers.Add(barrierID, new Barrier(barrierCount));
            }
            if (!barriers[barrierID].IsReleased())
            {
                // Barrier not released
                barriers[barrierID].RegisterProgramCounter(context.programCounter.getProgramCounter());
                int targetProgramCounter = context.programCounter.getProgramCounter() - 1;
                context.programCounter.setProgramCounter(targetProgramCounter);
                lastInstructionInTick = true;
            }
            else
            {
                // Barrier released, traversing
                barriers[barrierID].MarkTraversed(context.programCounter.getProgramCounter());
            }
         }
        

        void ADD(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.ADD.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.ADD.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 + operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.ADD.RESULT_POINTER), result, context.getContextID());
        }


        void SUB(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SUB.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SUB.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 - operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.SUB.RESULT_POINTER), result, context.getContextID());
        }


        void LI(Context context, Instruction instruction)
        {
            int result = instruction.GetOperand(ISA.LI.OPERAND_1_IMMEDIATE);
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.LI.TARGET_POINTER), result, context.getContextID());
        }

        void MOVE(Context context, Instruction instruction)
        {
            int source_pointer = dataMemory.ReadAddress(instruction.GetOperand(ISA.MOVE.SOURCE_POINTER_POINTER), context.getContextID());
            int result = dataMemory.ReadAddress(source_pointer, context.getContextID());
            int target_pointer = dataMemory.ReadAddress(instruction.GetOperand(ISA.MOVE.TARGET_POINTER_POINTER), context.getContextID());
            context.dataMemory.WriteAddress(target_pointer, result, context.getContextID());
        }

        void AND(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.AND.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.AND.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 & operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.ADD.RESULT_POINTER), result, context.getContextID());
        }


        void OR(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.OR.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.OR.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 | operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.OR.RESULT_POINTER), result, context.getContextID());
        }


        void XOR(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.XOR.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.XOR.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 ^ operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.XOR.RESULT_POINTER), result, context.getContextID());
        }


        void NOT(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.NOT.OPERAND_1_POINTER), context.getContextID());
            int result = ~ operand_1;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.NOT.RESULT_POINTER), result, context.getContextID());
        }


        void ADDI(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.ADDI.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = instruction.GetOperand(ISA.ADDI.OPERAND_2_IMMEDIATE);
            int result = operand_1 + operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.ADDI.RESULT_POINTER), result, context.getContextID());
        }


        void SUBI(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SUBI.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = instruction.GetOperand(ISA.SUBI.OPERAND_2_IMMEDIATE);
            int result = operand_1 - operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.SUBI.RESULT_POINTER), result, context.getContextID());
        }


        void ANDI(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.ANDI.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = instruction.GetOperand(ISA.ANDI.OPERAND_2_IMMEDIATE);
            int result = operand_1 & operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.ANDI.RESULT_POINTER), result, context.getContextID());
        }


        void ORI(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.ORI.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = instruction.GetOperand(ISA.ORI.OPERAND_2_IMMEDIATE);
            int result = operand_1 | operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.ORI.RESULT_POINTER), result, context.getContextID());
        }


        void XORI(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.XORI.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = instruction.GetOperand(ISA.XORI.OPERAND_2_IMMEDIATE);
            int result = operand_1 ^ operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.XORI.RESULT_POINTER), result, context.getContextID());
        }


        void SLL(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SLL.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SLL.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 << operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.SLL.RESULT_POINTER), result, context.getContextID());
        }


        void SRL(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SRL.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SRL.OPERAND_2_POINTER), context.getContextID()); 
            uint mask = ~(0xFFFFFFFF << (32 - operand_2)); // Transforming an aritmetic shift into a logical shift
            int result = (operand_1 >> operand_2) & (int)mask;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.SRL.RESULT_POINTER), result, context.getContextID());
        }


        void SRA(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SRA.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.SRA.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 >> operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.SRA.RESULT_POINTER), result, context.getContextID());
        }


        void MULT(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.MULT.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.MULT.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 * operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.MULT.RESULT_POINTER), result, context.getContextID());
        }


        void DIV(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.DIV.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.DIV.OPERAND_2_POINTER), context.getContextID());
            int result = operand_1 / operand_2;
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.DIV.RESULT_POINTER), result, context.getContextID());
        }


        void JI(Context context, Instruction instruction)
        {
            int targetProgramCounter = instruction.GetOperand(ISA.JI.TARGET_PROGRAM_COUNTER_IMMEDIATE) - 1;
            context.programCounter.setProgramCounter(targetProgramCounter);
        }


        void J(Context context, Instruction instruction)
        {
            int targetProgramCounter = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.J.TARGET_PROGRAM_COUNTER_POINTER), context.getContextID()) - 1;
            context.programCounter.setProgramCounter(targetProgramCounter);
        }


        void JIAL(Context context, Instruction instruction)
        {
            int linkLocationAddress = instruction.GetOperand(ISA.JIAL.LINK_LOCATION_POINTER);
            int resumeProgramCounter = context.programCounter.getProgramCounter() + 1;
            context.dataMemory.WriteAddress(linkLocationAddress, resumeProgramCounter, context.getContextID());
            int targetProgramCounter = instruction.GetOperand(ISA.JI.TARGET_PROGRAM_COUNTER_IMMEDIATE) - 1;
            context.programCounter.setProgramCounter(targetProgramCounter);
        }


        void BEQ(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_2_POINTER), context.getContextID());
            if (operand_1 == operand_2)
            {
                int targetProgramCounter = instruction.GetOperand(ISA.BEQ.TARGET_PROGRAM_COUNTER_IMMEDIATE) - 1;
                context.programCounter.setProgramCounter(targetProgramCounter);
            }
        }


        void BGE(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_2_POINTER), context.getContextID());
            if (operand_1 >= operand_2)
            {
                int targetProgramCounter = instruction.GetOperand(ISA.BEQ.TARGET_PROGRAM_COUNTER_IMMEDIATE) - 1;
                context.programCounter.setProgramCounter(targetProgramCounter);
            }
        }


        void BLE(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_1_POINTER), context.getContextID());
            int operand_2 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.BEQ.OPERAND_2_POINTER), context.getContextID());
            if (operand_1 <= operand_2)
            {
                int targetProgramCounter = instruction.GetOperand(ISA.BEQ.TARGET_PROGRAM_COUNTER_IMMEDIATE) - 1;
                context.programCounter.setProgramCounter(targetProgramCounter);
            }
        }


        void F_LI(Context context, Instruction instruction)
        {
            float result = instruction.GetOperandFP(ISA.F_LI.OPERAND_1_IMMEDIATE);
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_LI.TARGET_POINTER), result, context.getContextID());
        }


        void F_ADD(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_ADD.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_ADD.OPERAND_2_POINTER), context.getContextID());
            float result = operand_1 + operand_2;
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_ADD.RESULT_POINTER), result, context.getContextID());
        }


        void F_SUB(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_SUB.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_SUB.OPERAND_2_POINTER), context.getContextID());
            float result = operand_1 - operand_2;
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_SUB.RESULT_POINTER), result, context.getContextID());
        }


        void F_MULT(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_MULT.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_MULT.OPERAND_2_POINTER), context.getContextID());
            float result = operand_1 * operand_2;
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_MULT.RESULT_POINTER), result, context.getContextID());
        }


        void F_DIV(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_DIV.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_DIV.OPERAND_2_POINTER), context.getContextID());
            float result = operand_1 / operand_2;
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_DIV.RESULT_POINTER), result, context.getContextID());
        }


        void F_NEG(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_NEG.OPERAND_1_POINTER), context.getContextID());
            float result = -operand_1;
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_NEG.RESULT_POINTER), result, context.getContextID());
        }


        void F_ABS(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_ABS.OPERAND_1_POINTER), context.getContextID());
            float result;
            if (operand_1 >= 0)
            {
                result = operand_1;
            }
            else
            {
                result = -operand_1;
            }
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_ABS.RESULT_POINTER), result, context.getContextID());
        }

        void F_CEQ(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CEQ.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CEQ.OPERAND_2_POINTER), context.getContextID());
            int result = 0; // false
            if (operand_1 == operand_2)
            {
                result = 1; //true
            }
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.F_CEQ.RESULT_POINTER), result, context.getContextID());
        }

        void F_CGE(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CGE.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CGE.OPERAND_2_POINTER), context.getContextID());
            int result = 0; // false
            if (operand_1 >= operand_2)
            {
                result = 1; //true
            }
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.F_CGE.RESULT_POINTER), result, context.getContextID());
        }

        void F_CLE(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CLE.OPERAND_1_POINTER), context.getContextID());
            float operand_2 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CLE.OPERAND_2_POINTER), context.getContextID());
            int result = 0; // false
            if (operand_1 <= operand_2)
            {
                result = 1; //true
            }
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.F_CLE.RESULT_POINTER), result, context.getContextID());
        }

        void F_CVTI2F(Context context, Instruction instruction)
        {
            int operand_1 = context.dataMemory.ReadAddress(instruction.GetOperand(ISA.F_CVTI2F.OPERAND_1_POINTER), context.getContextID());
            float result = Convert.ToSingle(operand_1);
            context.dataMemory.WriteAddressFP(instruction.GetOperand(ISA.F_CVTI2F.RESULT_POINTER), result, context.getContextID());
        }


        void F_CVTF2I(Context context, Instruction instruction)
        {
            float operand_1 = context.dataMemory.ReadAddressFP(instruction.GetOperand(ISA.F_CVTF2I.OPERAND_1_POINTER), context.getContextID());
            int result = Convert.ToInt32(operand_1);
            context.dataMemory.WriteAddress(instruction.GetOperand(ISA.F_CVTF2I.RESULT_POINTER), result, context.getContextID());
        }
    }
}

/*
private static readonly Dictionary<int, Action<Engine, Context, Instruction>> executeTable = new Dictionary<int, Action<Engine, Context, Instruction>>()
{
    {0, SETELI}, //Set electrode immediate   SETELI - #electrode;
    {1, CLRELI}, //Clear electrode immediate CLRELI - #electrode;
    ...
};

*/
