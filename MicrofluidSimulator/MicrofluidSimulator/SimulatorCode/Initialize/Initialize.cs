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
            
            Container container = new Container(electrodeBoard, droplets);
            initializeSubscriptions(container);
            findNeighbours(container);
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
            droplets[0] = new Droplets("test droplet", 0, "h20", 10, 10, 10, 10, "blue", 20);
            
            return droplets;
        }

        private void initializeSubscriptions(Container container)
        {
            Droplets[] droplets = container.Droplets;
            foreach (Droplets droplet in droplets)
                {
                Models.SubscriptionModels.dropletSubscriptions(container, droplet);
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
            bool topCase = true;
            bool leftCase = true;

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
                    topCase = false;
                    leftCase = false;
                    neighbours.Add(electrodeBoard[i].ID1);
                    int accumulativeSizeX = -electrodeBoard[i].SizeX-1;
                    int accumulativeSizeY = -1;
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    
                    
                    neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[(int)neighbours[neighbours.Count - 1]], accumulativeSizeY, electrode.SizeY));
                    
                    
                    neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                }
                if(searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    
                    int accumulativeSizeX = -electrodeBoard[i].SizeX-1;
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                }


            
            }
            if (topCase == true)
            {
                for (int i = 0; i < electrodeBoard.Length; i++)
                {
                    searchTopPosX = electrode.PositionX - 1;
                    searchTopPosY = electrode.PositionY + 1;

                    int searchRightTopPosX = electrode.PositionX + electrode.SizeX + 1;
                    int searchRightTopPosY = electrode.PositionY + 1;

                    int minMarginX = electrodeBoard[i].PositionX;
                    int minMarginY = electrodeBoard[i].PositionY;
                    int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                    int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                    
                    
                    if (searchTopPosX > minMarginX && searchTopPosX < maxMarginX && searchTopPosY > minMarginY && searchTopPosY < maxMarginY)
                    {

                        neighbours.Add(electrodeBoard[i].ID1);
                        leftCase = false;
                        int accumulativeSizeY = 0;
                        neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                    }
                    
                    if (searchRightTopPosX > minMarginX && searchRightTopPosX < maxMarginX && searchRightTopPosY > minMarginY && searchRightTopPosY < maxMarginY)
                    {

                        neighbours.Add(electrodeBoard[i].ID1);
                        
                        int accumulativeSizeY = 0;
                        neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                    }

                }
                
            }

            if (leftCase == true)
            {
                searchBottomPosX = electrode.PositionX + 1;
                searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;

                int searchStraightTopPosX = electrode.PositionX + 1;
                int searchStraightTopPosY = electrode.PositionY - 1;
                for (int i = 0; i < electrodeBoard.Length; i++)
                {
                    int minMarginX = electrodeBoard[i].PositionX;
                    int minMarginY = electrodeBoard[i].PositionY;
                    int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                    int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                    if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                    {

                        int accumulativeSizeX = -electrodeBoard[i].SizeX;
                        neighbours.Add(electrodeBoard[i].ID1);
                        neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    }

                    if (searchStraightTopPosX > minMarginX && searchStraightTopPosX < maxMarginX && searchStraightTopPosY > minMarginY && searchStraightTopPosY < maxMarginY)
                    {

                        int accumulativeSizeX = -electrodeBoard[i].SizeX;
                        neighbours.Add(electrodeBoard[i].ID1);
                        neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    }
                }
            }
            
            neighbours.Remove(electrode.ID1);
          

            return neighbours;
        }

        private ArrayList findTopRightNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeX, int mainSizeX)
        {
            ArrayList neighbours = new ArrayList();

            // go to bottom right corner of electrode in question
            int searchRightPosX = electrode.PositionX + electrode.SizeX + 1;
            int searchRightPosY = electrode.PositionY + electrode.SizeY - 1;

            accumulativeSizeX += electrode.SizeX;

            if (accumulativeSizeX >= mainSizeX)
            {
                
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                
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
            int searchRightPosX = electrode.PositionX + electrode.SizeX + 1;
            int searchRightPosY = electrode.PositionY + 1;

            accumulativeSizeX += electrode.SizeX;
            if (accumulativeSizeX >= mainSizeX)
            {
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                
                
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
            int searchBottomPosX = electrode.PositionX + electrode.SizeX - 1;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;
           
            accumulativeSizeY += electrode.SizeY;
            

            if (accumulativeSizeY >= mainSizeY)
            {
                return neighbours;
            }

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                
                

                if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    //accumulativeSizeY += electrode.SizeY;
                    neighbours.Add(electrodeBoard[i].ID1);
                    neighbours.AddRange(findDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, mainSizeY));
                }

            }

            return neighbours;
        }


    }
}
