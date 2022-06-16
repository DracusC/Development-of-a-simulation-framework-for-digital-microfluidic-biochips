using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorSensorModels
    {
        /// <summary>
        /// Model for the color sensor detecting the color 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static int[] colorSensor(Container container, Sensors sensor)
        {
            Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, sensor);
            if (sensor.type == "RGB_color")
            {
                if (droplet == null)
                {

                    return new int[] { -1, -1, -1 };
                }
                else
                {
                    Color color = ColorTranslator.FromHtml(droplet.color);
                    return new int[] { color.R, color.G, color.B };
                }
            }
            return new int[] { -1, -1, -1 };
        }
    }
}
