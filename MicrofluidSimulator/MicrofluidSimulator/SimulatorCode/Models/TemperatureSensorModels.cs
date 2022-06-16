using MicrofluidSimulator.SimulatorCode.DataTypes; 
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class TemperatureSensorModels
    {

        /// <summary>
        /// Model of the temperature sensor detecting the temperature
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static float temperatureSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if(sensor.type == "temperature")
            {
                if (droplet == null)
                {
                    return -1;
                }
                else
                {
                    return droplet.temperature;
                }
            }
            return -1;
            
        }

    }
}
