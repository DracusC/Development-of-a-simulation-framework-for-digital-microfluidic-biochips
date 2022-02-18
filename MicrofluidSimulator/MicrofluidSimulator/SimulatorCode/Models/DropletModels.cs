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


            

            //get index of electrode we are on and get the electrode
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.ElectrodeID, container);
            Electrodes dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            // init values for finding the electrode with highest attraction
            double max = 0;
            Electrodes electrode = null;

            // Find the attraction for the electrode we are on
            // Attraction is definded by status/distance
            int electrodeCenterX = dropletElectrode.PositionX + 10;
            int electrodeCenterY = dropletElectrode.PositionY + 10;
            double dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.PositionX, caller.PositionY);
            double attraction = dropletElectrode.Status / (dist + 0.1);
            //If the attraction is higher than the previous highest, this is now the highest
            if (attraction > max)
            {
                max = attraction;
                electrode = dropletElectrode;
            }

            // Find the attraction for all the neighbouring electrodes
            foreach (int neighbour in dropletElectrode.Neighbours)
            {
                electrodeCenterX = electrodeBoard[neighbour].PositionX + 10;
                electrodeCenterY = electrodeBoard[neighbour].PositionY + 10;
                dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.PositionX, caller.PositionY);
                attraction = electrodeBoard[neighbour].Status / (dist + 0.1);
                if (attraction > max)
                {
                    max = attraction;
                    electrode = electrodeBoard[neighbour];
                }
            }
            // move the droplet to the electrode with the highest attraction
            if(electrode != null)
            {
                caller.PositionX = electrode.PositionX + 10;
                caller.PositionY = electrode.PositionY + 10;
                caller.ElectrodeID = electrode.ID1;
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
