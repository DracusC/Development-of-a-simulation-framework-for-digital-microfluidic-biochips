using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Initialize
{
    
    public class Initialize
    {

        public Initialize()
        {

        }

        public object[] initialize()
        {
            //JSONReader FILE = blabla;
            // electrodes = file.splitElectrodes;
            // droplets = file.splitDroplets
            Electrodes[] electrodes = new Electrodes[640];
            Droplets[] droplets = new Droplets[1];
            Electrodes[,] electrodeBoard = initializeBoard(electrodes);


            initializeDroplets(droplets);
            initializeSubscriptions(electrodeBoard, droplets);
            object[] ret = new object[2];
            ret[0] = electrodeBoard;
            ret[1] = droplets;
            return ret;
        }

        private Electrodes[,] initializeBoard(Electrodes[] electrodes)
        {
            Electrodes[,] electrodeBoard = new Electrodes[32, 20];
            for (int i = 0; i < electrodeBoard.GetLength(0); i++)
            {
                for (int j = 0; j < electrodeBoard.GetLength(1); j++)
                {
                    electrodeBoard[i, j].Subscriptions = null;
                    electrodeBoard[i, j].PositionX = i * 20;
                    electrodeBoard[i, j].PositionY = j * 20;
                    electrodeBoard[i, j].SizeX = 20;
                    electrodeBoard[i, j].SizeY = 20;
                    electrodeBoard[i, j].Status = 0;
                }
            }
            return electrodeBoard;

            
        }

        private Droplets[] initializeDroplets(Droplets[] droplets)
        {
            droplets[0].PositionX = 10;
            droplets[0].PositionY = 10;
            
            return droplets;
        }

        private void initializeSubscriptions(Electrodes[,] electrodeBoard, Droplets[] droplets)
        {
            foreach(Droplets droplet in droplets)
                {
                Models.SubscriptionModels.dropletSubscriptions(electrodeBoard, droplets, droplet);
                }
        }




    }
}
