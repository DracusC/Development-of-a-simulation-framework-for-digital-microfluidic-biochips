using MicrofluidSimulator.SimulatorCode.Simulator;
namespace MicrofluidSimulator.Models
{
    public class CounterHelper
    {
        public int incrementer(int num)
        {
            Simulator simulator = new Simulator();
            simulator.simulatorRun(null);
            // potentially use namespace to get rid of the complete path to information and other classes from different folders?
            //MicrofluidSimulator.SimulatorCode.Information information = new MicrofluidSimulator.SimulatorCode.Information();
            //information.Platform_name = "platformNameTest";
            //Console.WriteLine(information.Platform_name);

            // attempt of reading json files
            //MicrofluidSimulator.SimulatorCode.Initialize.JSONReader jSONReader = new MicrofluidSimulator.SimulatorCode.Initialize.JSONReader("JSONFiles/platform640.json") ;
            //jSONReader.LoadJson();
            //Console.WriteLine(jSONReader.Json);
            //Console.WriteLine(Environment.CurrentDirectory);
            return num + 22;
        }
    }
}
