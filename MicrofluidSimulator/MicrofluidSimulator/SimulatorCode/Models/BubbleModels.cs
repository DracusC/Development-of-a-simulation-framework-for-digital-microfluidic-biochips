namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class BubbleModels
    {
        public static void moveBubble(Droplets droplet, Bubbles bubble)
        {
            double radius = droplet.sizeX;
            if(droplet.positionX + radius >= bubble.positionX && droplet.positionX <= bubble.positionX
                && droplet.positionY + radius >= bubble.positionY && droplet.positionY <= bubble.positionY)
            {
                double vecVX = bubble.positionX - droplet.positionX;
                double vecVY = bubble.positionY - droplet.positionY;

                double vecLength = Math.Sqrt(Math.Pow(vecVX,2) * Math.Pow(vecVY,2));

                double vecUX = (radius / vecLength) * vecVX;
                double vecUY = (radius / vecLength) * vecVY;

                bubble.positionX = droplet.positionX + (int) vecUX;
                bubble.positionY = droplet.positionY + (int) vecUY;

            }
        }

        public static Bubbles makeBubble(Droplets droplet, int size)
        {

            Random rnd = new Random();
            

            int randX = rnd.Next(-1, 1);
            int randY = rnd.Next(-1, 1);
            while(randY == 0)
            {
                randY = rnd.Next(-1, 1);
            }
            while(randX == 0)
            {
                randX = rnd.Next(-1, 1);
            }
            int id = rnd.Next(10000000);

            Bubbles bubble = new Bubbles("test bubble", id, droplet.positionX + randX, droplet.positionY + randY, size, size, droplet.color);
            droplet.sizeX -= size;
            droplet.sizeY -= size;
            bubble.subscriptions.Add(droplet.ID);

            moveBubble(droplet, bubble);

            return bubble;
        }
    }
}
