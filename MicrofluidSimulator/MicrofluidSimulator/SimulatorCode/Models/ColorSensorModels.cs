using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorSensorModels
    {
        public int[] colorSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if(droplet == null)
            {
                return null;
            }
            else
            {
                Color color = Color.FromName(droplet.color);
                
                return new int[] { color.R, color.G, color.B };
            }
        }
    }
}
