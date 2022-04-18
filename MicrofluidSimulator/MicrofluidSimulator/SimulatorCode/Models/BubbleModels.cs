using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class BubbleModels
    {
        
        public static ArrayList bubbleMerge(Container container, Bubbles bubble)
        {
            ArrayList subscribers = new ArrayList();
            subscribers.Add(bubble.ID);
            List<Bubbles> bubbles = container.bubbles;
            for (int i = 0; i < bubbles.Count; i++)
            {
                if (!(bubbles[i].ID == bubble.ID) && (bubbles[i].toRemove == false) && (bubble.sizeX > bubbles[i].sizeX || bubble.sizeX == bubbles[i].sizeX))
                {
                    

                    double radius1 = bubble.sizeX / 2;
                    double radius2 = bubbles[i].sizeX / 2;
                    double vecVX = bubbles[i].positionX - bubble.positionX;
                    double vecVY = bubbles[i].positionY - bubble.positionY;

                    double dist = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));
                    double resize = Math.Sqrt(Math.Pow(bubbles[i].sizeX/2, 2) + Math.Pow(bubbles[i].sizeY/2, 2));
                    double vecUX = (resize / dist) * vecVX;
                    double vecUY = (resize / dist) * vecVY;

                    if (dist < (radius1 + radius2))
                    {
                        
                        bubble.sizeX += bubbles[i].sizeX;
                        bubble.sizeY += bubbles[i].sizeY;

                        bubble.positionX += (int)vecUX;
                        bubble.positionY += (int)vecUY;
                        bubbles[i].toRemove = true;
                    }
                }
                
            }
            return subscribers;
            
        }

        public static bool bubbleIsOnDroplet(Droplets droplet, Bubbles bubble)
        {

            double vecVX = bubble.positionX - droplet.positionX;
            double vecVY = bubble.positionY - droplet.positionY;

            double vecLength = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));

            double radius1 = droplet.sizeX / 2;
            double radius2 = bubble.sizeX / 2;

            if (vecLength < (radius1 + radius2) || vecLength <= GlobalVariables.RECTANGULARELECTRODESIZE / 2 || vecLength == 0)
            {

                return true;





            }
            return false;

            //double vecVX = bubble.positionX - droplet.positionX;
            //double vecVY = bubble.positionY - droplet.positionY;

            //double dist = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));

            //double radius1 = droplet.sizeX / 2;
            //double radius2 = bubble.sizeX / 2;

            //if (dist < (radius1 + radius2))
            //{
            //    return true;

            //}
            //return false;
        }
        public static ArrayList moveBubble(Container container, Bubbles bubble)
        {
            ArrayList subscribers = new ArrayList();
            subscribers.Add(bubble.ID);
            foreach (Droplets droplet in container.droplets)
            {
                if (bubbleIsOnDroplet(droplet, bubble))
                {
                    moveBubbleAccordingToGroup(container, droplet, bubble);
                    moveBubbleFromDroplet(droplet, bubble);
                }
            }
            return subscribers;
            

        }

        private static void moveBubbleAccordingToGroup(Container container, Droplets droplet, Bubbles bubble)
        {
            
            int rightPosX = droplet.positionX + GlobalVariables.RECTANGULARELECTRODESIZE;
            int rightPosY = droplet.positionY;
            if(HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, rightPosX, rightPosY))
            {
                bubble.positionX = droplet.positionX - 1;
            }
                

            int leftPosX = droplet.positionX - GlobalVariables.RECTANGULARELECTRODESIZE;
            int leftPosY = droplet.positionY;
            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, leftPosX, leftPosY))
            {
                bubble.positionX = droplet.positionX + 1;
            }
            int topPosX = droplet.positionX;
            int topPosY = droplet.positionY - GlobalVariables.RECTANGULARELECTRODESIZE;
            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, topPosX, topPosY))
            {
                bubble.positionY = droplet.positionY + 1;
            }
            int bottomPosX = droplet.positionX;
            int bottomPosY = droplet.positionY + GlobalVariables.RECTANGULARELECTRODESIZE;
            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, bottomPosX, bottomPosY))
            {
                
                bubble.positionY = droplet.positionY - 1;
            }
            
        }

        private static void moveBubbleFromDroplet(Droplets droplet, Bubbles bubble)
        {
            double vecVX = bubble.positionX - droplet.positionX;
            double vecVY = bubble.positionY - droplet.positionY;

            double vecLength = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));

            double radius1 = droplet.sizeX / 2;
            double radius2 = bubble.sizeX / 2;
            double resize = radius1 + radius2;
            if ((radius1 + radius2) < GlobalVariables.RECTANGULARELECTRODESIZE / 2)
            {
                resize = GlobalVariables.RECTANGULARELECTRODESIZE * 0.6;
            }
            if (vecLength == 0)
            {
                double vecUX = (resize / 1) * 1;
                double vecUY = (resize / 1) * 1;


                bubble.positionX = droplet.positionX + (int)vecUX;
                bubble.positionY = droplet.positionY + (int)vecUY;
            }
            else if (vecLength < (radius1 + radius2) || vecLength <= GlobalVariables.RECTANGULARELECTRODESIZE / 2)
            {

                double vecUX = (resize / vecLength) * vecVX;
                double vecUY = (resize / vecLength) * vecVY;


                bubble.positionX = droplet.positionX + (int)vecUX;
                bubble.positionY = droplet.positionY + (int)vecUY;





            }
        }

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
            else if (droplet.temperature >= 90 && (container.timeStep >= 0.5 || droplet.accumulatingBubbleEscapeVolume >= 0.5) )
            {
                
                droplet.accumulatingBubbleEscapeVolume += container.timeStep;
                
                
                    
                double splitAmount = 0.1 * droplet.sizeX * droplet.accumulatingBubbleEscapeVolume;
                double splitSize = HelpfullRetreiveFunctions.getDiameterOfBubble(splitAmount);
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
                droplet.volume -= (float) splitAmount;
                droplet.sizeX = DropletUtillityFunctions.getDiameterOfDroplet(droplet.volume);
                droplet.sizeY = DropletUtillityFunctions.getDiameterOfDroplet(droplet.volume);
                
                //bubble.subscriptions.Add(droplet.ID);

                // move bubble
                moveBubbleAccordingToGroup(container, droplet, bubble);
                moveBubbleFromDroplet(droplet, bubble);
                container.bubbles.Add(bubble);
                container.subscribedBubbles.Add(bubble.ID);


                subscribtions.Add(droplet.ID);
                droplet.accumulatingBubbleEscapeVolume = 0;
                return subscribtions;
            }else if(droplet.temperature >= 90)
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
