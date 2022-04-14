using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletColorModels
    {
        public static ArrayList deprecatedDropletColorChange(Container container, Droplets caller)
        {

            ArrayList groupColors = new ArrayList();
            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, caller.group);

            foreach (Droplets droplet in groupMembers)
            {
                groupColors.Add(ColorTranslator.FromHtml(droplet.color));
            }

            int r = 0;
            int g = 0;
            int b = 0;
            foreach (Color c in groupColors)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }
            r /= groupColors.Count;
            g /= groupColors.Count;
            b /= groupColors.Count;

            foreach (Droplets droplet in groupMembers)
            {
                droplet.color = $"#{r:X2}{g:X2}{b:X2}";
            
            }


            //return $"#{r:X2}{g:X2}{b:X2}";
            
            ArrayList subscribers = new ArrayList();
            subscribers.Add(caller.ID);
            return subscribers;
        }

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
