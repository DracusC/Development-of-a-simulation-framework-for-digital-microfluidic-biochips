using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletEvaporationModels
    {
        /// <summary>
        /// Model for evaporation of droplet into bubbles
        /// </summary>
        /// <param name="container"></param>
        /// <param name="droplet"></param>
        /// <returns></returns>
        public static ArrayList makeBubble(Container container, Droplets droplet)
        {
            ArrayList subscribtions = new ArrayList();

            Random rnd = new Random();
            if (droplet.temperature >= 90 && droplet.volume <= 10)
            {

                ArrayList dropletSubscritions = droplet.subscriptions;
                foreach (int n in dropletSubscritions)
                {

                    container.electrodes[n].subscriptions.Remove(droplet.ID);
                }
                container.subscribedDroplets.Remove(droplet.ID);
                //caller.Subscriptions = new ArrayList();
                float volume = droplet.volume;

                int removedDropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(droplet.electrodeID, container);
                Electrode removedDropletElectrode = container.electrodes[removedDropletElectrodeIndex];

                double splitAmount = droplet.volume;
                double splitSize = HelpfullRetreiveFunctions.getDiameterOfBubble(splitAmount);

                int id = rnd.Next(10000000);
                Bubbles bubble = new Bubbles("test bubble", id, droplet.positionX, droplet.positionY, (int)splitSize, (int)splitSize);
                container.bubbles.Add(bubble);
                container.subscribedBubbles.Add(bubble.ID);
                container.droplets.Remove(droplet);
                return subscribtions;
            }
            else if (droplet.temperature >= 90 && (container.timeStep >= 0.5m || droplet.accumulatingBubbleEscapeVolume >= 0.5m))
            {

                droplet.accumulatingBubbleEscapeVolume += container.timeStep;



                decimal splitAmount = 0.1m * droplet.sizeX * droplet.accumulatingBubbleEscapeVolume;
                double splitSize = HelpfullRetreiveFunctions.getDiameterOfBubble((double)splitAmount);
                if (splitSize > droplet.sizeX * 0.8)
                {
                    splitSize = droplet.sizeX * 0.8;
                }


                int signX = rnd.Next(2);
                int signY = rnd.Next(2);

                int toAddX = (-2 * signX) + 1;
                int toAddY = (-2 * signY) + 1;
                int id = rnd.Next(10000000);




                Bubbles bubble = new Bubbles("test bubble", id, droplet.positionX + toAddX, droplet.positionY + toAddY, (int)splitSize, (int)splitSize);
                droplet.volume -= (float)splitAmount;
                droplet.sizeX = DropletUtillityFunctions.getDiameterOfDroplet(droplet.volume);
                droplet.sizeY = DropletUtillityFunctions.getDiameterOfDroplet(droplet.volume);


                // move bubble
                BubbleModels.moveBubbleAccordingToGroup(container, droplet, bubble);
                BubbleModels.moveBubbleFromDroplet(droplet, bubble);
                container.bubbles.Add(bubble);
                container.subscribedBubbles.Add(bubble.ID);


                subscribtions.Add(droplet.ID);
                droplet.accumulatingBubbleEscapeVolume = 0;
                return subscribtions;
            }
            else if (droplet.temperature >= 90)
            {
                droplet.accumulatingBubbleEscapeVolume += container.timeStep;
                subscribtions.Add(droplet.ID);
                return subscribtions;
            }

            subscribtions.Add(droplet.ID);
            return subscribtions;


        }
    }
}
