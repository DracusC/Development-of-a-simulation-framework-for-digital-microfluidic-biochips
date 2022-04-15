using MicrofluidSimulator.SimulatorCode.DataTypes; 
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class TemperatureSensorModels
    {


        public static float temperatureSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if (droplet == null)
            {
                return -1;
            }
            else
            {
                return droplet.temperature;
            }
        }

    }
}
