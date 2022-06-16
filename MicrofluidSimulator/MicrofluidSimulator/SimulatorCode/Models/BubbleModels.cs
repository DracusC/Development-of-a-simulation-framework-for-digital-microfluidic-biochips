using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class BubbleModels
    {
        /// <summary>
        /// Model for merging bubbles
        /// </summary>
        /// <param name="container"></param>
        /// <param name="bubble"></param>
        /// <returns></returns>
        public static ArrayList bubbleMerge(Container container, Bubbles bubble)
        {
            ArrayList subscribers = new ArrayList();
            subscribers.Add(bubble.ID);
            List<Bubbles> bubbles = container.bubbles;
            for (int i = 0; i < bubbles.Count; i++)
            {
                // Merge a bubble into the caller if the merging bubble is marked with toRemove and it is smaller than the caller
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
        /// <summary>
        /// Function to check if a bubble is on a droplet
        /// </summary>
        /// <param name="droplet"></param>
        /// <param name="bubble"></param>
        /// <returns></returns>
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

            
        }
        /// <summary>
        /// Model to move a bubble
        /// </summary>
        /// <param name="container"></param>
        /// <param name="bubble"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Move a bubble acording to a group of droplets
        /// </summary>
        /// <param name="container"></param>
        /// <param name="droplet"></param>
        /// <param name="bubble"></param>
        public static void moveBubbleAccordingToGroup(Container container, Droplets droplet, Bubbles bubble)
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
        /// <summary>
        /// Function to move a bubble away from a droplet
        /// </summary>
        /// <param name="droplet"></param>
        /// <param name="bubble"></param>
        public static void moveBubbleFromDroplet(Droplets droplet, Bubbles bubble)
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
        
    }
}
