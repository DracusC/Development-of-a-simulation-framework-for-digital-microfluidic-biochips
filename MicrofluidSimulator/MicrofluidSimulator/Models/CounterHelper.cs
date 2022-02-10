namespace MicrofluidSimulator.Models
{
    public class CounterHelper
    {
        public int incrementer(int num)
        {
            MicrofluidSimulator.SimulatorCode.Information information = new MicrofluidSimulator.SimulatorCode.Information();
            information.Platform_name = "platformNameTest";
            Console.WriteLine(information.Platform_name);

            return num + 22;
        }
    }
}
