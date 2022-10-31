using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BioVirtualMachine
{
    class Program
    {

        static void Main(string[] args)
        {
            //NewExpressionSolver.NewExpressionSolver.Test();

            //Console.WriteLine((long)TypeDescriptor.GetConverter(typeof(long)).ConvertFromString("0xFFFFFFFFFFFFFFFF"));

            
            // Assembling
            //FileStream programFileUser = new FileStream("./../../BVM_test_00.basm", FileMode.Open, FileAccess.Read);
            FileStream programFileUser = new FileStream("./../../DATE2020.basm", FileMode.Open, FileAccess.Read);
            FileStream programFile = new FileStream("./../../test_sys.basm", FileMode.Create, FileAccess.Write);
            Assembler assembler = new Assembler();
            assembler.Translate(ref programFileUser, ref programFile);
            programFileUser.Close();
            programFile.Close();

            // Setting up the logger
            FileStream loggerConfigurationFile = new FileStream("./../../Logger/VMlogger_configuration.xml", FileMode.Open, FileAccess.Read);
            ILogger VMLogger = new Logger(ref loggerConfigurationFile);
            loggerConfigurationFile.Close();

            VMLogger.Info("Bio Virtual Machine says: Hello!");

            VMLogger.Info("Loading VM configuration from file.");
            programFile = new FileStream("./../../test_sys.basm", FileMode.Open, FileAccess.Read);
            VMConfiguration vMConfiguration = new VMConfiguration(ref programFile, VMLogger);
            VMLogger.Info(vMConfiguration.ToString());
            programFile.Close();

            VMLogger.Info("Loading VM data memory from file.");
            programFile = new FileStream("./../../test_sys.basm", FileMode.Open, FileAccess.Read);
            DataMemory dataMemory = new DataMemory(vMConfiguration, ref programFile);//, VMLogger);
            VMLogger.Info(dataMemory.ToString());
            programFile.Close();

            VMLogger.Info("Loading VM instruction memory from file.");
            programFile = new FileStream("./../../test_sys.basm", FileMode.Open, FileAccess.Read);
            InstructionMemory instructionMemory = new InstructionMemory(vMConfiguration, ref programFile);
            VMLogger.Info(instructionMemory.ToString());
            programFile.Close();

            VMLogger.Info("Starting VM engine.");
            IDMFPlatform dMFPlatform = new DMFPlatformDebug(VMLogger);

            Engine engine = new Engine(vMConfiguration, instructionMemory, dataMemory, dMFPlatform, VMLogger);
            VMLogger.Info("VM ready for execution.");
            VMLogger.Info("Starting execution.");

            while (engine.Execute()) {; }

            VMLogger.Info("No more active contexts. End of execution.");
            VMLogger.Info(dataMemory.ToString());
            
            VMLogger.Info("Bio Virtual Machine says: Goodbye!");
            

            Console.ReadLine();
            
        }

    }
}
