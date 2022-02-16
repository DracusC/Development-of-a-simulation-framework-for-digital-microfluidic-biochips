using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Linq;
namespace MicrofluidSimulator.SimulatorCode.Initialize
{
    
    public class Initialize
    {

        public Initialize()
        {

        }

        public Container initialize()
        {
            
            Electrodes[] electrodes = new Electrodes[640];
            Droplets[] droplets = new Droplets[1];
            Electrodes[] electrodeBoard = initializeBoard(electrodes);


            initializeDroplets(droplets);
            initializeSubscriptions(electrodeBoard, droplets);
            Container container = new Container(electrodeBoard, droplets);
            return container;
        }

        private Electrodes[] initializeBoard(Electrodes[] electrodes)
        {
            Electrodes[] electrodeBoard = new Electrodes[640];
            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                electrodeBoard[i] = new Electrodes("arrel", i, i, i, 0, (i % 32) * 20, (i / 32) * 20, 20, 20, 0, null);
                /*electrodeBoard[i].ID1 = i;
                //electrodeBoard[i].Subscriptions = null;
                electrodeBoard[i].PositionX = (i%32) * 20;
                electrodeBoard[i].PositionY = (i/32) * 20;
                electrodeBoard[i].SizeX = 20;
                electrodeBoard[i].SizeY = 20;
                electrodeBoard[i].Status = 0;*/
                
            }
            return electrodeBoard;

            
        }

        private Droplets[] initializeDroplets(Droplets[] droplets)
        {
            droplets[0] = new Droplets();
            droplets[0].PositionX = 10;
            droplets[0].PositionY = 10;
            
            return droplets;
        }

        private void initializeSubscriptions(Electrodes[] electrodeBoard, Droplets[] droplets)
        {
            foreach(Droplets droplet in droplets)
                {
                Models.SubscriptionModels.dropletSubscriptions(electrodeBoard, droplets, droplet);
                }
        }

        private Container findNeighbours(Container container)
        {
            for(int i = 0; i < container.Electrodes.Length; i++)
            {
                
                container.Electrodes[i].Neighbours = findNeighboursByElectrode(container.Electrodes, container.Electrodes[i]);
            }
            return container;
        }

        private ArrayList findNeighboursByElectrode(Electrodes[] electrodeBoard, Electrodes electrode)
        {
            ArrayList neighbours = new ArrayList();
            int searchTopPosX = electrode.PositionX - 1;
            int searchTopPosY = electrode.PositionY - 1;

            int searchBottomPosX = electrode.PositionX - 1;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;
            for(int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                
                if (searchTopPosX > minMarginX && searchTopPosX < maxMarginX && searchTopPosY > minMarginY && searchTopPosY < maxMarginY)
                {
                    neighbours.Add(electrodeBoard[i].ID1);
                    int accumulativeSizeX = -electrodeBoard[i].SizeX - 1;
                    int accumulativeSizeY = - 1;
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX+1));
                    neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[(int) neighbours[neighbours.Count]], accumulativeSizeY, electrode.SizeY + 1));
                    neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY+1));
                }
                if(searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    int accumulativeSizeX = -electrodeBoard[i].SizeX - 1;
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX+1));
                }
            }
            return neighbours;
        }

        private ArrayList findTopRightNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeX, int mainSizeX)
        {
            ArrayList neighbours = new ArrayList();

            // go to bottom right corner of electrode in question
            int searchRightPosX = electrode.PositionX + electrode.SizeX;
            int searchRightPosY = electrode.PositionY + electrode.SizeY - 1;

            if(accumulativeSizeX > mainSizeX)
            {
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                accumulativeSizeX += electrode.SizeX;
                if (searchRightPosX > minMarginX && searchRightPosX < maxMarginX && searchRightPosY > minMarginY && searchRightPosY < maxMarginY)
                {
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, mainSizeX));
                }
                
            }

            return neighbours;
        }

        private ArrayList findBottomRightNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeX, int mainSizeX)
        {
            ArrayList neighbours = new ArrayList();

            // go to bottom right corner of electrode in question
            int searchRightPosX = electrode.PositionX + electrode.SizeX;
            int searchRightPosY = electrode.PositionY;

            if (accumulativeSizeX > mainSizeX)
            {
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                accumulativeSizeX += electrode.SizeX;
                if (searchRightPosX > minMarginX && searchRightPosX < maxMarginX && searchRightPosY > minMarginY && searchRightPosY < maxMarginY)
                {
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, mainSizeX));
                }

            }

            return neighbours;
        }

        private ArrayList findDownwardsNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeY, int mainSizeY)
        {

            ArrayList neighbours = new ArrayList();
            int searchBottomPosX = electrode.PositionX + electrode.SizeX;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY;

            if (accumulativeSizeY > mainSizeY)
            {
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                accumulativeSizeY += electrode.SizeY;
                if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, mainSizeY));
                }

            }

            return neighbours;
        }


    }
}
