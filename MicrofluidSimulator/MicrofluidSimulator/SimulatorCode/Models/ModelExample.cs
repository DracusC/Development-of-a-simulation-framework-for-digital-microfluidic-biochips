using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
using System.Drawing;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    /// <summary>
    /// This class is a basic example of the model classes
    /// The modelExample function creates a new droplet (this could normally be the result of a split) and adds it to the container and the list of subscribed droplets
    /// </summary>
    public class ModelExample
    {
        public static ArrayList modelExample(Container container, Droplets caller)
        {
            // Retrieve needed information from the container, e.g.
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            // Implement model behaviour calculations
            // Grab the first electrode and get its coordinates
            Electrode tempElectrode = electrodeBoard[0];
            int[] centerOfElectrode = ElectrodeModels.getCenterOfElectrode(tempElectrode);
            int electrodeCenterX = centerOfElectrode[0];
            int electrodeCenterY = centerOfElectrode[1];
            // Construct a random ID
            Random rnd = new Random();
            int id = rnd.Next(10000000);
            // Use the same color as the caller
            string color = caller.color;
            // Get the diameter of a droplet of default volume
            int diam = DropletUtillityFunctions.getDiameterOfDroplet(360);
            // Create the new droplet
            Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, diam, diam, color, caller.temperature, (int)(ElectrodeModels.getAreaOfElectrode(tempElectrode) * 0.9), tempElectrode.ID, id, caller.accumulatingBubbleEscapeVolume);
            // Add the new droplet to the droplets in the container
            droplets.Add(newDroplet);
            
            // Always add the caller ID to the list of subscribers
            ArrayList subscribers = new ArrayList();
            subscribers.Add(caller.ID);
            // Also add any additional new droplets in case they are created in the model
            subscribers.Add(newDroplet.ID);

            // Return the list of subsribers
            return subscribers;

        }
    }
}
