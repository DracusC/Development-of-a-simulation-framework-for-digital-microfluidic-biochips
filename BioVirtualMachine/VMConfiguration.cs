using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class VMConfiguration
    {
        private int parametersCount = 4;
        internal int DATA_MEMORY_SIZE { get; set; }
        internal int PRIVATE_DATA_MEMORY_START_ADDRESS { get; set; }
        internal bool DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING { get; set; }
        internal bool DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES { get; set; }
        //internal bool LOGGER_LOG_DEBUG_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_INFO_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_WARNING_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_ERROR_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_FATAL_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_TRACE_TO_CONSOLE { get; set; }
        //internal bool LOGGER_LOG_DEBUG_TO_FILE { get; set; }
        //internal bool LOGGER_LOG_INFO_TO_FILE { get; set; }
        //internal bool LOGGER_LOG_WARNING_TO_FILE { get; set; }
        //internal bool LOGGER_LOG_ERROR_TO_FILE { get; set; }
        //internal bool LOGGER_LOG_FATAL_TO_FILE { get; set; }
        //internal bool LOGGER_LOG_TRACE_TO_FILE { get; set; }

        ILogger logger;

        internal VMConfiguration()
        {
            this.logger = new InactiveLogger();
            InitializeParametersToDefault();
            logger.Warning("Default configuration applied for all the parameters.");
        }

        internal VMConfiguration(ILogger logger)
        {
            this.logger = logger;
            InitializeParametersToDefault();
            logger.Warning("Default configuration applied for all the parameters.");
        }

        internal VMConfiguration(ref FileStream inFile)
        {
            this.logger = new InactiveLogger();
            InitializeParametersToDefault();
            List<string> configurationList = GetConfigurationListFromFile(ref inFile);
            int initalizedParametersCount = InitializeParameters(configurationList);
            if (initalizedParametersCount< parametersCount)
            {
                logger.Warning("Initialized only {0} of the {1} parameters. \nDefault configuration is applied for the rest.", initalizedParametersCount, parametersCount);
            }
        }

        internal VMConfiguration(ref FileStream inFile, ILogger logger)
        {
            this.logger = logger;
            InitializeParametersToDefault();
            List<string> configurationList = GetConfigurationListFromFile(ref inFile);
            int initalizedParametersCount = InitializeParameters(configurationList);
            if (initalizedParametersCount < parametersCount)
            {
                logger.Warning("Initialized only {0} of the {1} parameters. \nDefault configuration is applied for the rest.", initalizedParametersCount, parametersCount);
            }
        }


        internal void InitializeParametersToDefault()
        {
            DATA_MEMORY_SIZE = -1;
            PRIVATE_DATA_MEMORY_START_ADDRESS = 0x800000;
            DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING = false;
            DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES = false;
            //LOGGER_LOG_DEBUG_TO_CONSOLE = false;
            //LOGGER_LOG_INFO_TO_CONSOLE = false;
            //LOGGER_LOG_WARNING_TO_CONSOLE = false;
            //LOGGER_LOG_ERROR_TO_CONSOLE = false;
            //LOGGER_LOG_FATAL_TO_CONSOLE = false;
            //LOGGER_LOG_TRACE_TO_CONSOLE = false;
            //LOGGER_LOG_DEBUG_TO_FILE = false;
            //LOGGER_LOG_INFO_TO_FILE = false;
            //LOGGER_LOG_WARNING_TO_FILE = false;
            //LOGGER_LOG_ERROR_TO_FILE = false;
            //LOGGER_LOG_FATAL_TO_FILE = false;
            //LOGGER_LOG_TRACE_TO_FILE = false;
        }


        internal int InitializeParameters(List<string> configurationList)
        {
            int initalizedParametersCount = 0;
            foreach (string item in configurationList)
            {
                string line = item.Substring(0, item.IndexOf(BioAssemblyDefinition.configurationTerminator));
                if (line.Contains("DATA_MEMORY_SIZE"))
                {
                    string[] lineElements = line.Split(' ');
                    DATA_MEMORY_SIZE = Convert.ToInt32(lineElements[1]);
                    initalizedParametersCount++;
                }
                if (line.ToUpper().Contains("PRIVATE_DATA_MEMORY_START_ADDRESS"))
                {
                    string[] lineElements = line.Split(' ');
                    PRIVATE_DATA_MEMORY_START_ADDRESS = Convert.ToInt32(lineElements[1]);
                    initalizedParametersCount++;
                }
                if (line.ToUpper().Contains("DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING"))
                {
                    string[] lineElements = line.Split(' ');
                    DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING = lineElements[1].Equals("TRUE");
                    initalizedParametersCount++;
                }
                if (line.ToUpper().Contains("DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES"))
                {
                    string[] lineElements = line.Split(' ');
                    DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES = lineElements[1].Equals("TRUE");
                    initalizedParametersCount++;
                }
                //if (line.ToUpper().Contains("LOGGER_LOG_DEBUG_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_DEBUG_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_INFO_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_INFO_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_WARNING_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_WARNING_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_ERROR_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_ERROR_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_FATAL_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_FATAL_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_TRACE_TO_CONSOLE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_TRACE_TO_CONSOLE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_DEBUG_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_DEBUG_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_INFO_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_INFO_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_WARNING_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_WARNING_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_ERROR_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_ERROR_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_FATAL_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_FATAL_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
                //if (line.ToUpper().Contains("LOGGER_LOG_TRACE_TO_FILE"))
                //{
                //    string[] lineElements = line.Split(' ');
                //    LOGGER_LOG_TRACE_TO_FILE = lineElements[1].Equals("TRUE");
                //    initalizedParametersCount++;
                //}
            }
            return initalizedParametersCount;
        }

        public override string ToString()
        {
            string answer = "";
            answer += "Bio Virtual Machine configuration:" + '\n';
            answer += "DATA_MEMORY_SIZE = " + DATA_MEMORY_SIZE.ToString() + '\n';
            answer += "PRIVATE_DATA_MEMORY_START_ADDRESS = " + PRIVATE_DATA_MEMORY_START_ADDRESS.ToString() + '\n';
            answer += "DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING = " + DEBUG_LOG_DATA_TYPES_FOR_PRINT_FORMATTING.ToString() + '\n';
            answer += "DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES = " + DEBUG_KEEP_ALL_PRIVATE_DATA_MEMORIES.ToString();// +'\n';
            //answer += "LOGGER_LOG_DEBUG_TO_CONSOLE = " + LOGGER_LOG_DEBUG_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_INFO_TO_CONSOLE = " + LOGGER_LOG_INFO_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_WARNING_TO_CONSOLE = " + LOGGER_LOG_WARNING_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_ERROR_TO_CONSOLE = " + LOGGER_LOG_ERROR_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_FATAL_TO_CONSOLE = " + LOGGER_LOG_FATAL_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_TRACE_TO_CONSOLE = " + LOGGER_LOG_TRACE_TO_CONSOLE.ToString() + '\n';
            //answer += "LOGGER_LOG_DEBUG_TO_FILE  = " + LOGGER_LOG_DEBUG_TO_FILE.ToString() + '\n';
            //answer += "LOGGER_LOG_INFO_TO_FILE  = " + LOGGER_LOG_INFO_TO_FILE.ToString() + '\n';
            //answer += "LOGGER_LOG_WARNING_TO_FILE  = " + LOGGER_LOG_WARNING_TO_FILE.ToString() + '\n';
            //answer += "LOGGER_LOG_ERROR_TO_FILE  = " + LOGGER_LOG_ERROR_TO_FILE.ToString() + '\n';
            //answer += "LOGGER_LOG_FATAL_TO_FILE  = " + LOGGER_LOG_FATAL_TO_FILE.ToString() + '\n';
            //answer += "LOGGER_LOG_TRACE_TO_FILE  = " + LOGGER_LOG_TRACE_TO_FILE.ToString(); // + '\n';
            return answer;
        }


        /*
        * This metod loads the content of the inFile into the list outBuffer and: 
        * - Removes the comments
        * - Makes all lowercase
        * - Remove line start and line end whitespaces
        * - Removes empity lines
        * - Reducing multiple whitespaces (including tabs) to a single one
        */
        internal List<string> GetConfigurationListFromFile(ref FileStream inFile)
        {
            List<string> configurationList = new List<string>();
            bool isConfiguration = true; // By default, what is read is .configuration

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
                        isConfiguration = false;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.dataSectionRegex))
                    {
                        isConfiguration = false;
                        continue;
                    }
                    else if (Regex.IsMatch(line, BioAssemblyDefinition.configurationSectionRegex))
                    {
                        isConfiguration = true;
                        continue;
                    }
                    if (isConfiguration)
                    {
                        configurationList.Add(line.ToUpper());
                    }
                }
            }
            inFileStream.Close();
            return configurationList;
        }


    }
}
