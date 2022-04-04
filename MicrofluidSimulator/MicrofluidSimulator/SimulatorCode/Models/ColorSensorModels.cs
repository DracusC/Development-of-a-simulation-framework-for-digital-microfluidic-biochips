using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorSensorModels
    {
        public string colorSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if(droplet == null)
            {
                return null;
            }
            else
            {
                return droplet.color;
            }
        }
    }
}
