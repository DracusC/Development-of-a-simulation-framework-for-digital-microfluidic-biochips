using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BioVirtualMachine
{
    class InstructionMemory
    {
        private Dictionary<int, Instruction> program = new Dictionary<int, Instruction>();

        VMConfiguration vMConfiguration;

        /* Class constuctor */
        public InstructionMemory(VMConfiguration vMConfiguration, ref FileStream inFile)
        {
            this.vMConfiguration = vMConfiguration;
            List<string> instructionList = GetInstructionListFromFile(ref inFile);
            InitializeMemory(instructionList);
        }

        /*
        * This metod loads the content of the inFile into the list outBuffer and: 
        * - Removes the comments
        * - Makes all lowercase
        * - Remove line start and line end whitespaces
        * - Removes empity lines
        * - Reducing multiple whitespaces (including tabs) to a single one
        */
        internal List<string> GetInstructionListFromFile(ref FileStream inFile)
        {
            List<string> instructionList = new List<string>();
            bool isText = true; // By default, what is read is .text

            StreamReader inFileStream = new StreamReader(inFile);

            string line;

            while ((line = inFileStream.ReadLine()) != null)
            {
                line = line.Trim();
                line = Regex.Replace(line, @"\s+", " "); // Reducing multiple whitespaces to a single one
                if (!line.Equals(""))
                {
                    if (Regex.IsMatch(line, BioAssemblyDefinition.textSectionRegex))
                    {
                        isText = true;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.dataSectionRegex))
                    {
                        isText = false;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.configurationSectionRegex))
                    {
                        isText = false;
                        continue;
                    }
                    if (isText)
                    {
                        instructionList.Add(line.ToUpper());
                    }
                }
            }
            inFileStream.Close();
            return instructionList;
        }


        internal void InitializeMemory(List<string> initializationList)
        {
            foreach (string item in initializationList)
            {
                string line = item.Substring(0, item.IndexOf(BioAssemblyDefinition.instructionTerminator));
                string[] lineElements = line.Split(BioAssemblyDefinition.instructionAddressSeparator);
                int address = Convert.ToInt32(lineElements[0]);
                lineElements[1] = lineElements[1].Trim();
                program.Add(address, new Instruction(lineElements[1]));
            }
        }

        public bool IsAddressUsed(int address)
        {
            return program.ContainsKey(address);
        }

        public Instruction ReadAddress(int address)
        {
            if (IsAddressUsed(address))
            {
                return program[address];
            }
            else
            {
                throw new System.ArgumentException("Reading an empity instruction memory location.");
            }
        }


        public override string ToString()
        {
            string answer = "";
            answer += "Instruction memory content: \n";
            foreach(KeyValuePair<int, Instruction> memEntry in program)
            {
                answer += String.Format("{0} : {1}", memEntry.Key, memEntry.Value.ToString()) + '\n';
            }
            answer = answer.Remove(answer.Length - 1); // Removing last newline
            return answer;
        }
    }

}
