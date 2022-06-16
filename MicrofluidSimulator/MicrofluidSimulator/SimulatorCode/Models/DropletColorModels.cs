using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletColorModels
    {
        /// <summary>
        /// Model for chaning color of a droplet
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static ArrayList dropletColorChange(Container container, Droplets caller)
        {
            Color color = ColorTranslator.FromHtml(caller.color);
            DropletUtillityFunctions.updateGroupColor(container, caller.group, color, 0);

            ArrayList subscribers = new ArrayList();
            subscribers.Add(caller.ID);
            return subscribers;
        }
    }
}
