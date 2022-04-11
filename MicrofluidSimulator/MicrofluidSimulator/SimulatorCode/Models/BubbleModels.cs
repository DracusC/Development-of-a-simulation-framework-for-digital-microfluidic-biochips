using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class BubbleModels
    {

        public static void bubbleMerge(Container container, Bubbles bubble, List<Bubbles> bubbles)
        {
            for (int i = 0; i < bubbles.Count; i++)
            {
                if (!(bubbles[i].ID == bubble.ID) && (bubbles[i].toRemove == false) && (bubble.sizeX > bubbles[i].sizeX || bubble.sizeX == bubbles[i].sizeX))
                {

                    
                    double radius1 = bubble.sizeX / 2;
                    double radius2 = bubbles[i].sizeX / 2;
                    double vecVX = bubbles[i].positionX - bubble.positionX;
                    double vecVY = bubbles[i].positionY - bubble.positionY;

                    double dist = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));
                    double resize = Math.Sqrt(Math.Pow(bubbles[i].sizeX, 2) + Math.Pow(bubbles[i].sizeY, 2));
                    double vecUX = (resize / dist) * vecVX;
                    double vecUY = (resize / dist) * vecVY;

                    if (dist < (radius1 + radius2))
                    {
                        Console.WriteLine("IN HERE2");
                        bubble.sizeX += bubbles[i].sizeX;
                        bubble.sizeY += bubbles[i].sizeY;

                        bubble.positionX += (int)vecUX;
                        bubble.positionY += (int)vecUY;
                        bubbles[i].toRemove = true;
                    }
                }
                
            }
            
        }

        public static bool bubbleIsOneDroplet(Droplets droplet, Bubbles bubble)
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
        public static void moveBubble(Droplets droplet, Bubbles bubble)
        {
            
            double vecVX = bubble.positionX - droplet.positionX;
            double vecVY = bubble.positionY - droplet.positionY;

            double vecLength = Math.Sqrt(Math.Pow(vecVX, 2) + Math.Pow(vecVY, 2));

            double radius1 = droplet.sizeX / 2;
            double radius2 = bubble.sizeX / 2;
            double resize = radius1 + radius2;
            if ((radius1 + radius2) < GlobalVariables.RECTANGULARELECTRODESIZE/2)
            {
                resize = GlobalVariables.RECTANGULARELECTRODESIZE*0.6;
            }
            if (vecLength == 0)
            {
                double vecUX = (resize / 1) * 1;
                double vecUY = (resize / 1) * 1;


                bubble.positionX = droplet.positionX + (int)vecUX;
                bubble.positionY = droplet.positionY + (int)vecUY;
            }else if (vecLength < (radius1 + radius2) || vecLength <= GlobalVariables.RECTANGULARELECTRODESIZE/2)
            {
                
                double vecUX = (resize / vecLength) * vecVX;
                double vecUY = (resize / vecLength) * vecVY;
                

                bubble.positionX = droplet.positionX + (int) vecUX;
                bubble.positionY = droplet.positionY + (int) vecUY;


                
                

            }
            

        }

        public static ArrayList makeBubble(Container container, Droplets droplet)
        {
            ArrayList subscribtions = new ArrayList();
            
            if(droplet.temperature >= 90)
            {
                Random rnd = new Random();
                
                int splitAmount = (int) (0.1 * droplet.sizeX * container.timeStep);



                int signX = rnd.Next(3);
                int signY = rnd.Next(3);

                int toAddX = (-2 * signX) + 1;
                int toAddY = (-2 * signY) + 1;
                int id = rnd.Next(10000000);
                

                

                Bubbles bubble = new Bubbles("test bubble", id, droplet.positionX + toAddX, droplet.positionY + toAddY, splitAmount*5, splitAmount*5, droplet.color);
                droplet.sizeX -= splitAmount;
                droplet.sizeY -= splitAmount;
                droplet.volume = DropletUtillityFunctions.getVolumeOfDroplet(droplet.sizeX);
                bubble.subscriptions.Add(droplet.ID);
                
                moveBubble(droplet, bubble);
                container.bubbles.Add(bubble);
                container.subscribedBubbles.Add(bubble.ID);


                subscribtions.Add(droplet.ID);
                return subscribtions;
            }
            subscribtions.Add(droplet.ID);
            return subscribtions;
            
            
        }

        internal static List<Bubbles> copyBubbles(List<Bubbles> bubbles)
        {
            List<Bubbles> copyOfBubbles = new List<Bubbles>();

            foreach(Bubbles bubble in bubbles)
            {
                Bubbles newBubble = new Bubbles("test bubble", bubble.ID, bubble.positionX, bubble.positionY, bubble.sizeX, bubble.sizeY, bubble.color);
                newBubble.toRemove = bubble.toRemove;
                copyOfBubbles.Add(newBubble);
            }
            return copyOfBubbles;
           
        }
    }
}
