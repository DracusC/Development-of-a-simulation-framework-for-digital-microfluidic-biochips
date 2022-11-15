using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class Instruction
    {
        private int opcode; 
        private List<int> operands = new List<int>();


        public Instruction(string textInstruction)
        {
            FillFromString(textInstruction);
        }


        public void FillFromString(string textInstruction)
        {
            string[] instructionElements = textInstruction.Split(' ');
            opcode = ISA.GetOpcode(instructionElements[0]);
            for (int i = 1; i < instructionElements.Length; i++)
            {
                if (Regex.IsMatch(instructionElements[i], BioAssemblyDefinition.numericFloatRegex))
                {
                    // It is a float
                    operands.Add(BitConverter.ToInt32(BitConverter.GetBytes(Convert.ToSingle(instructionElements[i])), 0));
                }
                else
                {
                    // It is an integer
                    operands.Add(Convert.ToInt32(instructionElements[i]));
                }
            }
        }

        public int GetOpcode()
        {
            return opcode;
        }

        public int GetOperandsSize()
        {
            return operands.Count;
        }

        public int GetOperand(int index)
        {
            return operands[index];
        }

        public float GetOperandFP(int index)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(operands[index]),0);
        }

        public override string ToString()
        {
            string answer = ISA.GetInstructionName(opcode);
            foreach (int operand in operands)
            {
                answer = answer + " " + operand;
            }
            return answer;
        }

    }
}
