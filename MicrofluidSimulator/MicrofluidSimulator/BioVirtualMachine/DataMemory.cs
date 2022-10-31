using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class DataMemory
    {
        private Dictionary<int, int> sharedDataMemory = new Dictionary<int, int>();
        private Dictionary<int, Dictionary<int, int>> privateDataMemory = new Dictionary<int, Dictionary<int, int>>(); //<contextID, <address, data>>

        private List<long> fPAddresses = new List<long>(); //<context/address hash>
        //private Dictionary<int, List<int>> fPPrivateAddresses = new Dictionary<int, List<int>>(); //<contextID, <addresses>>

        VMConfiguration vMConfiguration;
        ILogger logger;
        

        /* Class constuctors */
        public DataMemory(VMConfiguration vMConfiguration)
        {
            this.logger = new InactiveLogger();
            this.vMConfiguration = vMConfiguration;
        }


        public DataMemory(VMConfiguration vMConfiguration, ILogger logger)
        {
            this.logger = logger;
            this.vMConfiguration = vMConfiguration;
        }


        public DataMemory(VMConfiguration vMConfiguration, ref FileStream inFile)
        {
            this.logger = new InactiveLogger();
            this.vMConfiguration = vMConfiguration;
            List<string> initializationList = GetInitializationListFromFile(ref inFile);
            InitializeMemory(initializationList);
        }


        public DataMemory(VMConfiguration vMConfiguration, ref FileStream inFile, ILogger logger)
        {
            this.logger = logger;
            this.vMConfiguration = vMConfiguration;
            try
            {
                List<string> initializationList = GetInitializationListFromFile(ref inFile);
                InitializeMemory(initializationList);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Initialization of the data memory from file failed.");
                throw;
            }
        }


        private void InitializeMemory(List<string> initializationList)
        {
            logger.Info("Initialization of the data memory from file.");
            foreach (string item in initializationList)
            {
                string line = item.Substring(0, item.IndexOf(BioAssemblyDefinition.dataTerminator));
                string[] lineElements = line.Split(BioAssemblyDefinition.dataAddressSeparator);
                int address = Convert.ToInt32(lineElements[0]);
                lineElements[1] = lineElements[1].Trim();
                if(Regex.IsMatch(lineElements[1], BioAssemblyDefinition.numericFloatRegex))
                {
                    float data = Convert.ToSingle(lineElements[1]);
                    WriteAddressFP(address, data, 0);
                }
                else
                {
                    int data = Convert.ToInt32(lineElements[1]);
                    WriteAddress(address, data, 0);
                }
            }
        }

        /*
        * This metod loads the content of the inFile into the list outBuffer and: 
        * - Removes the comments
        * - Makes all lowercase
        * - Remove line start and line end whitespaces
        * - Removes empity lines
        * - Reducing multiple whitespaces (including tabs) to a single one
        */
        internal List<string> GetInitializationListFromFile(ref FileStream inFile)
        {
            List<string> initializationList = new List<string>();
            bool isData = true; // By default, what is read is .data

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
                        isData = false;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.dataSectionRegex))
                    {
                        isData = true;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.configurationSectionRegex))
                    {
                        isData = false;
                        continue;
                    }
                    if (isData)
                    {
                        initializationList.Add(line);
                    }
                }
            }
            inFileStream.Close();
            return initializationList;
        }

        private bool PrivateDataMemoryExists(int contextID)
        {
            return privateDataMemory.ContainsKey(contextID);
        }

        private bool IsAddressUsed(int address, int contextID)
        {
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                // Shared memory access
                return sharedDataMemory.ContainsKey(address);
            }
            else
            {
                // Private memory access
                if (PrivateDataMemoryExists(contextID))
                {
                    return privateDataMemory[contextID].ContainsKey(address);
                }
                else
                {
                    return false;
                }
            }

        }

        public void WriteAddress(int address, int value, int contextID)
        {
            if (vMConfiguration.DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING && fPAddresses.Contains(HashContexIDAndAddress(contextID, address)))
            {
                fPAddresses.Remove(HashContexIDAndAddress(contextID, address));
            }
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                // Shared memory access
                logger.Info("Shared data memory write: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, value);
                if (IsAddressUsed(address, contextID))
                {
                    sharedDataMemory[address] = value;
                }
                else
                {
                    sharedDataMemory.Add(address, value); 
                }
            }
            else
            {
                logger.Info("Private data memory write: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, value);
                if (PrivateDataMemoryExists(contextID))
                {
                    if (IsAddressUsed(address, contextID))
                    {
                        privateDataMemory[contextID][address] = value;
                    }
                    else
                    {
                        privateDataMemory[contextID].Add(address, value);
                    }
                }
                else
                {
                    privateDataMemory.Add(contextID, new Dictionary<int, int>() { { address, value } });
                }

            }
        }

        public void WriteAddressFP(int address, float value, int contextID)
        {
            if (vMConfiguration.DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING && !fPAddresses.Contains(HashContexIDAndAddress(contextID, address)))
            {
                fPAddresses.Add(HashContexIDAndAddress(contextID, address));
            }
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                // Shared memory access
                logger.Info("Shared data memory write: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, value);
                if (IsAddressUsed(address, contextID))
                {
                    sharedDataMemory[address] = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
                }
                else
                {
                    sharedDataMemory.Add(address, BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
                }
            }
            else
            {
                logger.Info("Private data memory write: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, value);
                if (PrivateDataMemoryExists(contextID))
                {
                    if (IsAddressUsed(address, contextID))
                    {
                        privateDataMemory[contextID][address] = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
                    }
                    else
                    {
                        privateDataMemory[contextID].Add(address, BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
                    }
                }
                else
                {
                    privateDataMemory.Add(contextID, new Dictionary<int, int>() { { address, BitConverter.ToInt32(BitConverter.GetBytes(value), 0) } });
                }

            }
        }

        public int ReadAddress(int address, int contextID)
        {
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                // Shared memory access
                if (IsAddressUsed(address, contextID))
                {
                    logger.Info("Shared data memory read: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, sharedDataMemory[address]);
                    return sharedDataMemory[address];
                }
                else
                {
                    logger.Warning("Shared data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, 0);
                    return 0; 
                }
            }
            else
            {
                // Private memory access
                if (PrivateDataMemoryExists(contextID))
                {
                    if (IsAddressUsed(address, contextID))
                    {
                        logger.Info("Private data memory read: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, privateDataMemory[contextID][address]);
                        return privateDataMemory[contextID][address];
                    }
                    else
                    {
                        logger.Warning("Private data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, 0);
                        return 0;
                    }
                }
                else
                {
                    logger.Warning("Private data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (integer)", contextID, address, 0);
                    return 0;
                }

            }
        }


        public float ReadAddressFP(int address, int contextID)
        {
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                // Shared memory access
                if (IsAddressUsed(address, contextID))
                {
                    logger.Info("Shared data memory read: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, BitConverter.ToSingle(BitConverter.GetBytes(sharedDataMemory[address]), 0));
                    return BitConverter.ToSingle(BitConverter.GetBytes(sharedDataMemory[address]), 0);
                }
                else
                {
                    logger.Warning("Shared data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, 0);
                    return 0;
                }
            }
            else
            {
                // Private memory access
                if (PrivateDataMemoryExists(contextID))
                {
                    if (IsAddressUsed(address, contextID))
                    {
                        logger.Info("Private data memory read: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, BitConverter.ToSingle(BitConverter.GetBytes(privateDataMemory[contextID][address]), 0));
                        return BitConverter.ToSingle(BitConverter.GetBytes(privateDataMemory[contextID][address]), 0);
                    }
                    else
                    {
                        logger.Warning("Shared data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, 0);
                        return 0;
                    }
                }
                else
                {
                    logger.Warning("Shared data memory read from unitialized location: \ncontextID: {0} \naddress: {1} \nvalue: {2} (real)", contextID, address, 0);
                    return 0;
                }

            }
        }

        public void RemovePrivateDataMemory(int contextID)
        {
            if (privateDataMemory.ContainsKey(contextID))
            {
                privateDataMemory.Remove(contextID);
                logger.Info("Deleting private data memory for contextID: {0}.", contextID);
            }
            if (vMConfiguration.DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING){
                for (int i = fPAddresses.Count-1; i >= 0 ; i--)
                {
                    if (getContextIDFromHash(fPAddresses[i]) == contextID)
                    {
                        if (getAddressFromHash(fPAddresses[i]) >= vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
                        {
                            fPAddresses.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string answer = "";
            List<int> list = sharedDataMemory.Keys.ToList();
            list.Sort();
            answer += "Shared memory content: \n" ;
            foreach (int key in list)
            {
                string dataString = "";
                if (vMConfiguration.DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING && fPAddresses.Contains(HashContexIDAndAddress(0, key)))
                {
                    dataString = BitConverter.ToSingle(BitConverter.GetBytes(sharedDataMemory[key]), 0).ToString(BioAssemblyDefinition.floatNumberFormat, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    dataString = sharedDataMemory[key].ToString();
                }
                answer += String.Format("{0} : {1}", key, dataString) + '\n';
            }

            list = privateDataMemory.Keys.ToList();
            list.Sort();
            foreach (var contextID in list)
            {
                var sublist = privateDataMemory[contextID].Keys.ToList();
                sublist.Sort();
                answer += String.Format("Private memory context {0}:", contextID) + '\n';
                foreach (var key in sublist)
                {
                    string dataString = "";
                    if (vMConfiguration.DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING && fPAddresses.Contains(HashContexIDAndAddress(contextID, key)))
                    {
                        dataString = BitConverter.ToSingle(BitConverter.GetBytes(privateDataMemory[contextID][key]), 0).ToString(BioAssemblyDefinition.floatNumberFormat, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dataString = privateDataMemory[contextID][key].ToString();
                    }
                    answer += String.Format("{0} : {1}", key, dataString) + '\n';
                }
            }
            answer = answer.Remove(answer.Length - 1); // Removing last newline
            return answer;
        }

        private long HashContexIDAndAddress(int contextID, int address)
        {
            if (address < vMConfiguration.PRIVATE_DATA_MEMORY_START_ADDRESS)
            {
                return (long)(ulong)(uint)address;
            }
            else
            {
                return (long)((((ulong)(uint)contextID) << 32) | (ulong)(uint)address);
            }
            
        }

        private int getContextIDFromHash(long hash)
        {
            return (int)(uint)(((ulong)hash & 0xFFFFFFFF00000000)>>32);
        }

        private int getAddressFromHash(long hash)
        {
            return (int)(uint)((ulong)hash & 0x00000000FFFFFFFF);
        }


    }
}
