using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorSensorModels
    {
        public static int[] colorSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if(droplet == null)
            {
                Console.WriteLine("DIDNT FIND DROPLET");
                return null;
            }
            else
            {
                Console.WriteLine("FOUND DROPLET WITH ID" + droplet.ID);
                Color color = Color.FromName(droplet.color);
                Console.WriteLine("FOUND DROPLET WITH COLOR 1" + color.R);
                return new int[] { color.R, color.G, color.B };
            }
        }
    }
}
