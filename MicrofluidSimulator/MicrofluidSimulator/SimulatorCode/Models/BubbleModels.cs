using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class BubbleModels
    {
        public static void moveBubble(Droplets droplet, Bubbles bubble)
        {
            double radius = droplet.sizeX/2;
            if((droplet.positionX + radius >= bubble.positionX && droplet.positionX <= bubble.positionX
                && droplet.positionY + radius >= bubble.positionY && droplet.positionY <= bubble.positionY)
                || (droplet.positionX - radius <= bubble.positionX && droplet.positionX >= bubble.positionX
                && droplet.positionY - radius <= bubble.positionY && droplet.positionY >= bubble.positionY)
                || (droplet.positionX + radius >= bubble.positionX && droplet.positionX <= bubble.positionX
                && droplet.positionY - radius <= bubble.positionY && droplet.positionY >= bubble.positionY)
                || (droplet.positionX - radius <= bubble.positionX && droplet.positionX >= bubble.positionX
                && droplet.positionY + radius >= bubble.positionY && droplet.positionY <= bubble.positionY))
            {
                
                double vecVX = bubble.positionX - droplet.positionX;
                double vecVY = bubble.positionY - droplet.positionY;

                double vecLength = Math.Sqrt(Math.Pow(vecVX,2) + Math.Pow(vecVY,2));

                double vecUX = (radius / vecLength) * vecVX;
                double vecUY = (radius / vecLength) * vecVY;
                

                bubble.positionX = droplet.positionX + (int) vecUX;
                bubble.positionY = droplet.positionY + (int) vecUY;

                
                

            }
            

        }

        public static ArrayList makeBubble(Container container, Droplets droplet)
        {
            ArrayList subscribtions = new ArrayList();
            
            if(droplet.temperature >= 100)
            {
                Random rnd = new Random();
                
                int splitAmount = (int) (0.1 * droplet.sizeX * container.timeStep);



                int signX = rnd.Next(3);
                int signY = rnd.Next(3);

                int toAddX = (-2 * signX) + 1;
                int toAddY = (-2 * signY) + 1;
                int id = rnd.Next(10000000);
                

                Console.WriteLine("TOADD IS X, Y IS " + toAddX + ", " + toAddY);

                Bubbles bubble = new Bubbles("test bubble", id, droplet.positionX + toAddX, droplet.positionY + toAddY, splitAmount, splitAmount, droplet.color);
                droplet.sizeX -= splitAmount;
                droplet.sizeY -= splitAmount;
                droplet.volume = DropletUtillityFunctions.getVolumeOfDroplet(droplet.sizeX);
                bubble.subscriptions.Add(droplet.ID);
                
                moveBubble(droplet, bubble);
                container.bubbles.Add(bubble);
                
                subscribtions.Add(droplet.ID);
                return subscribtions;
            }
            subscribtions.Add(droplet.ID);
            return subscribtions;
            
            
        }
    }
}
