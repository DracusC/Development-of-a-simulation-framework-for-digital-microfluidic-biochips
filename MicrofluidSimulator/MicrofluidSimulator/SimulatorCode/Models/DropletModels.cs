using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletModels
    {
        public static void dropletMovement(Container container, Droplets caller)
        {

            Droplets[] droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
            int posX = caller.PositionX / 20;
            int posY = caller.PositionY / 20;
            double max= 0;
            Electrodes electrode = null;
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x >= 0 && x < 32 && y >= 0 && y < 32)
                    {
                        int electrodeCenterX = electrodeBoard[(posX + x) + (posY + y) * 32].PositionX + 10;
                        int electrodeCenterY = electrodeBoard[(posX + x) + (posY + y) * 32].PositionY + 10;
                        double dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.PositionX, caller.PositionY);
                        double attraction = electrodeBoard[(posX + x) + (posY + y) * 32].Status / (dist + 0.001);
                        if (attraction > max)
                        {
                            max = attraction;
                            electrode = electrodeBoard[(posX + x) + (posY + y) * 32];
                        }
                    }
                }
            }
            if(electrode != null)
            {
                caller.PositionX = electrode.PositionX + 10;
                caller.PositionY = electrode.PositionY + 10;
            }
        }

        private static double electrodeDistance(int electrodeCenterX, int electrodeCenterY,int dropletX, int dropletY)
        {
            double x = Math.Pow(dropletX - electrodeCenterX,2);
            double y = Math.Pow(dropletY - electrodeCenterY,2);

            return Math.Sqrt(x+y);
        }
    }
}
