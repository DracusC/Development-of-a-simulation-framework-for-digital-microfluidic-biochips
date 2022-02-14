namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletModels
    {
        public static void dropletMovement(Electrodes[,] electrodeBoard, Droplets[] droplets, Droplets caller)
        {
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
                        int electrodeCenterX = electrodeBoard[posX + x, posY + y].PositionX + 10;
                        int electrodeCenterY = electrodeBoard[posX + x, posY + y].PositionY + 10;
                        double dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.PositionX, caller.PositionY);
                        if (dist > max)
                        {
                            max = dist;
                            electrode = electrodeBoard[posX + x, posY + y];
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
