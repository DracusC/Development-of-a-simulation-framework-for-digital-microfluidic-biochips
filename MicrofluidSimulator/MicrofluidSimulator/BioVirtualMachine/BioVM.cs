using BioVirtualMachine;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace MicrofluidSimulator.BioVirtualMachine
{
    public class BioVM
    {
        Simulator simulator;
        string recompiled_file_path;
        string uploaded_file_path;
        Engine bioVMEngine;
        FileStream programFile;

        public BioVM (string recompiled_file_path, string uploaded_file_path, Simulator simulator)
        {
            this.simulator = simulator;
            this.recompiled_file_path = recompiled_file_path;
            this.uploaded_file_path = uploaded_file_path;

            FileStream programFileUser = new FileStream(uploaded_file_path, FileMode.Open, FileAccess.Read); // Uploaded
            programFile = new FileStream(recompiled_file_path, FileMode.Create, FileAccess.Write); // Recompiled
            Assembler assembler = new Assembler();
            Console.WriteLine("before");
            assembler.Translate(ref programFileUser, ref programFile);
            Console.WriteLine("after");
            programFileUser.Close();
            programFile.Close();

            init();
        }

        public void init()
        {
            programFile = new FileStream(recompiled_file_path, FileMode.Open, FileAccess.Read);
            VMConfiguration vMConfiguration = new VMConfiguration(ref programFile);
            programFile.Close();

            programFile = new FileStream(recompiled_file_path, FileMode.Open, FileAccess.Read);
            DataMemory dataMemory = new DataMemory(vMConfiguration, ref programFile);
            programFile.Close();

            programFile = new FileStream(recompiled_file_path, FileMode.Open, FileAccess.Read);
            InstructionMemory instructionMemory = new InstructionMemory(vMConfiguration, ref programFile);
            programFile.Close();

            IDMFPlatform dMFPlatform = new DMFPlatform(simulator);

            bioVMEngine = new Engine(vMConfiguration, instructionMemory, dataMemory, dMFPlatform);

            //while (bioVMEngine.Execute()) {; }
            Console.WriteLine("BioVM init done");
        }

        public bool step()
        {
            return bioVMEngine.Execute();
        }
    }
}
