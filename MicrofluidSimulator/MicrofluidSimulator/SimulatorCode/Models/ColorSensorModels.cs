using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ColorSensorModels
    {
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
