using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    internal static class ISA
    {

        private static readonly Dictionary<string, int> opcodeTable = new Dictionary<string, int>()
        {
            //DMF specific
            {"SETELI", 0}, //Set electrode immediate   SETELI - #electrode;
            {"CLRELI", 1}, //Clear electrode immediate CLRELI - #electrode;
            
            {"SETEL", 2}, //Set electrode             SETEL  - *electrode;
            {"CLREL", 3}, //Clear electrode           CLREL  - *electrode;

            {"CLRALL", 49}, //Clear all electrodes

            // Accessing devices
            {"DEVWR", 6}, // devID *source
            {"ADEVRD", 4}, // devID *dest
            {"ADEVWR", 5}, // devID *source
            {"ADEVEX", 47}, // 
            {"ADEVCL", 48}, // 
            
            //Dispatching
            {"TSTART", 7}, //Start task TSTART - #target;
            {"TSTOP", 8}, //Stop task                 TSTOP  - ;

            //Synchronization
            {"TICK", 9}, //Release barrier           RBARR  - #barrier;
            {"BARR", 10}, //Wait barrier              WBARR  - #barrier_id #barrier_count;
            
            //ALU
            {"LI", 34}, //Load immediate                      LOAD  - *res, #imm
            {"MOVE", 11}, // Implments memory indirection     MOVE  - *target_pointer_pointer, *source_pointer_pointer
            {"ADD", 12}, //Add                       ADD  - *res, *o1, *o2
            {"SUB", 13}, //Subtract                  SUB  - *res, *o1, *o2
            {"AND", 14}, //And                       AND  - *res, *o1, *o2
            {"OR", 15}, //Or                        OR   - *res, *o1, *o2
            {"XOR", 16}, //Exclusive Or XOR  - *res, *o1, *o2
            {"NOT", 17}, //Not                       NOT  - *res, *o1
            {"ADDI", 18}, //Add Immediate ADDI - *res, *o1, #imm
            {"SUBI", 19}, //Subtract Immediate        SUBI - *res, *o1, #imm
            {"ANDI", 20}, //And Immediate             ANDI - *res, *o1, #imm
            {"ORI", 21}, //Or Immediate              ORI  - *res, *o1, #imm
            {"XORI", 22}, //Exclusive Or Immediate XORI - *res, *o1, #imm
            //{"", }, //(To Be Decided) Load Upper Immediate LUI

            //Shifts
            {"SLL", 23}, //Shift Left Logical        SLL
            {"SRL", 24}, //Shift Right Logical       SRL
            {"SRA", 25}, //Shift Right Arithmetic    SRA

            //Multiplication and division
            {"MULT", 26}, //Multiply MULT - *res, *o1, *o2
            {"DIV", 27}, //Divide                    DIV  - *res, *o1, *o2

            //Jump and branch
            {"JI", 28}, //Jump Immediate                                   JI     - #target
            {"J", 29}, //Jump J      - *target
            {"JIAL", 30}, //Jump Immediate and Link JIAL   - #target, *link
            {"BEQ", 31}, //Branch if Equal BEQ    - #target, *o1, *o1
            {"BGE", 32}, //Branch if Greater Than or Equal                        BGE    - #target, *o1, *o1
            {"BLE", 33}, //Branch if Less Than or Equal             BLE   - #target, *o1, *o1

            // Float arithmentics
            {"F_LI", 35}, // LOAD IMMEDIATE FLOAT
            {"F_ADD", 36}, // ADDITION FLOAT
            {"F_SUB", 37}, // SUBTRACTION FLOAT
            {"F_MULT", 38}, // MULTIPLICATION FLOAT
            {"F_DIV", 39}, // DIVISION FLOAT
            {"F_NEG", 40}, // CHANGE SIGN FLOAT
            {"F_ABS", 41}, // ABSOLUTE FLOAT
            {"F_CEQ", 42}, // COMPARE EQUAL -> TARGET ZERO IF FALSE
            {"F_CGE", 43}, // COMPARE GREATER EQUAL -> TARGET ZERO IF FALSE
            {"F_CLE", 44}, // COMPARE LESS EQUAL -> TARGET ZERO IF FALSE
            {"F_CVTI2F", 45}, // INTEGER TO FLOAT
            {"F_CVTF2I", 46} // FLOAT TO INTEGER

            //NEXT OPCODE: 50
        };

        internal static Dictionary<string, int> GetOpcodeTable()
        {
            return opcodeTable;
        }

        internal static int GetOpcode(string instructionName)
        {
            return opcodeTable[instructionName];
        }

        internal static string GetInstructionName(int opcode)
        {
            /*
            foreach (KeyValuePair<string,int> kvp in opcodeTable){
                if (kvp.Value == opcode){
                    return kvp.Key;
                }
            }
            */
            return opcodeTable.First(x => x.Value == opcode).Key;
        }

        
        internal static string GetConcatenatedOpcodes(string separator)
        {
            List<string> opcodes = opcodeTable.Keys.ToList();
            string concatenatedOpcodes = "";
            for (int i = 0; i < opcodes.Count-1; i++)
            {
                concatenatedOpcodes = concatenatedOpcodes + opcodes[i] + separator;
            }
            concatenatedOpcodes = concatenatedOpcodes + opcodes[opcodes.Count - 1];
            return concatenatedOpcodes;
        }

        internal static class SETELI
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int ELECTRODE_IMMEDIATE = 0;
        }

        internal static class CLRELI
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int ELECTRODE_IMMEDIATE = 0;
        }

        internal static class SETEL
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int ELECTRODE_POINTER = 0;
        }

        internal static class CLREL
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int ELECTRODE_POINTER = 0;
        }

        internal static class CLRALL
        {
            internal static readonly int OPERANDS_COUNT = 0;
        }

        internal static class ADEVRD
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int DEVICE_ADDRESS_IMMEDIATE = 0;
            internal static readonly int TARGET_POINTER = 1;
        }

        internal static class ADEVWR
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int DEVICE_ADDRESS_IMMEDIATE = 0;
            internal static readonly int SOURCE_POINTER = 1;
        }

        internal static class ADEVEX
        {
            internal static readonly int OPERANDS_COUNT = 0;
        }

        internal static class ADEVCL
        {
            internal static readonly int OPERANDS_COUNT = 0;
        }

        internal static class DEVWR
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int DEVICE_ADDRESS_IMMEDIATE = 0;
            internal static readonly int SOURCE_POINTER = 1;
        }

        internal static class TSTART
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int TARGET_PROGRAM_COUNTER = 0;
        }

        internal static class TSTOP
        {
            internal static readonly int OPERANDS_COUNT = 0;
        }

        internal static class TICK
        {
            internal static readonly int OPERANDS_COUNT = 0;
        }

        internal static class BARR
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int BARRIER_ID = 0;
            internal static readonly int BARRIER_COUNT = 1;
        }

        internal static class LI
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int TARGET_POINTER = 0;
            internal static readonly int OPERAND_1_IMMEDIATE = 1;
        }

        internal static class MOVE
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int TARGET_POINTER_POINTER = 0;
            internal static readonly int SOURCE_POINTER_POINTER = 1;
        }

        internal static class ADD
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class SUB
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class AND
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class OR
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class XOR
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class NOT
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
        }

        internal static class ADDI
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_IMMEDIATE = 2;
        }

        internal static class SUBI
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_IMMEDIATE = 2;
        }

        internal static class ANDI
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_IMMEDIATE = 2;
        }

        internal static class ORI
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_IMMEDIATE = 2;
        }

        internal static class XORI
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_IMMEDIATE = 2;
        }

        internal static class SLL
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class SRL
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class SRA
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class MULT
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class DIV
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }
        
        internal static class JI
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int TARGET_PROGRAM_COUNTER_IMMEDIATE = 0;
        }

        internal static class J
        {
            internal static readonly int OPERANDS_COUNT = 1;
            internal static readonly int TARGET_PROGRAM_COUNTER_POINTER = 0;
        }

        internal static class JIAL
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int TARGET_PROGRAM_COUNTER_IMMEDIATE = 0;
            internal static readonly int LINK_LOCATION_POINTER = 1; // Where the to come back is saved.
        }

        internal static class BEQ
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int TARGET_PROGRAM_COUNTER_IMMEDIATE = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class BGE
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int TARGET_PROGRAM_COUNTER_IMMEDIATE = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class BLE
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int TARGET_PROGRAM_COUNTER_IMMEDIATE = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_LI
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int TARGET_POINTER = 0;
            internal static readonly int OPERAND_1_IMMEDIATE = 1;
        }

        internal static class F_ADD
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_SUB
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_MULT
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_DIV
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_NEG
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
        }

        internal static class F_ABS
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
        }

        internal static class F_CEQ
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_CGE
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_CLE
        {
            internal static readonly int OPERANDS_COUNT = 3;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
            internal static readonly int OPERAND_2_POINTER = 2;
        }

        internal static class F_CVTI2F
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
        }

        internal static class F_CVTF2I
        {
            internal static readonly int OPERANDS_COUNT = 2;
            internal static readonly int RESULT_POINTER = 0;
            internal static readonly int OPERAND_1_POINTER = 1;
        }

    }
}
