using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorModels
    {
        public string colorSensor(Container container, Droplets caller)
        {
            return caller.Color;
            
        }
    }
}
