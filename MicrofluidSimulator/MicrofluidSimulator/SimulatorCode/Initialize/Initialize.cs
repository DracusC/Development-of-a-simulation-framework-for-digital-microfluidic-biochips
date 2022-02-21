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
            Droplets[] droplets = new Droplets[2];
            Electrodes[] electrodeBoard = initializeBoard(electrodes);


            initializeDroplets(droplets);
            
            Container container = new Container(electrodeBoard, droplets);
            findNeighbours(container);
            initializeSubscriptions(container);
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
            droplets[0].ElectrodeID = 0;
            droplets[1] = new Droplets("test droplet2", 1, "h20", 50, 50, 50, 50, "blue", 20);
            droplets[1].ElectrodeID = 66;
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
                /*if(container.Electrodes[i].ID1 == 34)
                {
                    Console.WriteLine("electrode id is : " + container.Electrodes[i].ID1);
                    foreach(int neighbour in container.Electrodes[i].Neighbours)
                    {
                        Console.WriteLine(neighbour);
                    }
                    
                }*/
            }
            return container;
        }

        private ArrayList findNeighboursByElectrode(Electrodes[] electrodeBoard, Electrodes electrode)
        {
            // used to check wether the electrode in question is along the top border, left border or in the top left corner
            bool topCase = true;
            bool leftCase = true;

            // initialize neighbour array
            ArrayList neighbours = new ArrayList();

            // define pixel coordinate in the top left corner of the given electrode 
            int searchTopPosX = electrode.PositionX - 1;
            int searchTopPosY = electrode.PositionY - 1;

            // define pixel coordinate in the bottom left corner of the given electrode 
            int searchBottomPosX = electrode.PositionX - 1;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;

            // loop through all electrodes to find a matching elecrode to the given coordinates
            for(int i = 0; i < electrodeBoard.Length; i++)
            {
                
                // for each electrode calculate the margin of coordinates that it holds
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;


                // check if the i'th electrode matches the coordinate of the electrode we're finding neighbours of

                // check top left coordinate 
                if (searchTopPosX > minMarginX && searchTopPosX < maxMarginX && searchTopPosY > minMarginY && searchTopPosY < maxMarginY)
                {
                    // if there is a neighbour in the top left corner of the electrode we are no longer in an edge or corner case, therefore we set false
                    topCase = false;
                    leftCase = false;
                    // add the top left neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // define an accumulative size so when the neighbourfinder recursively runs, it won't find any neighbours that are further away than +/- 1 of the 
                    // electrode we are currently looking at
                    int accumulativeSizeX = -electrodeBoard[i].SizeX-1;
                    int accumulativeSizeY = -1;

                    // find all the neighbours along the topside of the search electrode
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    
                    // take the last found neighbour from the previous found neighbours (this would be the one in the top right corner of our search electrode) and find all 
                    // the neighbours along the right side (downwards) of the search electrode
                    neighbours.AddRange(findLeftSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[(int)neighbours[neighbours.Count - 1]], accumulativeSizeY, electrode.SizeY));

                    // start from the top left neighbour and find all the neighbours along the left side (downwards) of the search electrode 
                    neighbours.AddRange(findRightSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                }

                // check bottom left coordinate 
                if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    // define an accumulative size so when the neighbourfinder recursively runs, it won't find any neighbours that are further away than +/- 1 of the 
                    // electrode we are currently looking at
                    
                    int accumulativeSizeX = -electrodeBoard[i].SizeX-1;

                    // add the bottom left neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // start from the bottom left neighbour and find all the neighbours along the bottom side of the search electrode
                    neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                }


            
            }

            // special case for electrodes that lie on the top border of the grid
            if (topCase == true)
            {
                // loop through all electrodes to find a matching elecrode to the given coordinates defined previously searchTopPosX and searchTopPosY
                for (int i = 0; i < electrodeBoard.Length; i++)
                {
                    // searchTopPos is now the coordinate on the left side of the search electrode 
                    searchTopPosX = electrode.PositionX - 1;
                    searchTopPosY = electrode.PositionY + 1;

                    // searchRightTopPos is the coordinate on the right side of the search electrode
                    int searchRightTopPosX = electrode.PositionX + electrode.SizeX + 1;
                    int searchRightTopPosY = electrode.PositionY + 1;

                    // for each electrode calculate the margin of coordinates that it holds
                    int minMarginX = electrodeBoard[i].PositionX;
                    int minMarginY = electrodeBoard[i].PositionY;
                    int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                    int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                    
                    // check left side coordinate for matches
                    if (searchTopPosX > minMarginX && searchTopPosX < maxMarginX && searchTopPosY > minMarginY && searchTopPosY < maxMarginY)
                    {
                        // add the left neighbour first
                        neighbours.Add(electrodeBoard[i].ID1);

                        // since we found a left neighbour, we now know that we are not in a corner case, but just in a top case (along the top border of the grid)
                        leftCase = false;

                        // we're working downwards, therefore we define the accumulator for the y size.
                        int accumulativeSizeY = 0;
                        
                        // find all the neighbours along the left side of our electrode, but stop before finding the neighbour in the bottom left corner 
                        neighbours.AddRange(findRightSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                    }
                    
                    // check right side coordinate for matches
                    if (searchRightTopPosX > minMarginX && searchRightTopPosX < maxMarginX && searchRightTopPosY > minMarginY && searchRightTopPosY < maxMarginY)
                    {
                        // add the right neighbour first
                        neighbours.Add(electrodeBoard[i].ID1);

                        // we're working downwards, therefore we define the accumulator for the y size.
                        int accumulativeSizeY = 0;

                        // find all the neighbours along the right side of our electrode, but stop before finding the neighbour in the bottom right corner 
                        neighbours.AddRange(findLeftSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, electrode.SizeY));
                    }

                }
                
            }

            // special case for electrodes that lie on the left border of the grid
            if (leftCase == true)
            {

                // searchBottomPos is now the coordinates of the neighbour straight under the given electrode 
                searchBottomPosX = electrode.PositionX + 1;
                searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;

                // searchStraightTopPos is the coordinates of the neighbour straight above the given electrode
                int searchStraightTopPosX = electrode.PositionX + 1;
                int searchStraightTopPosY = electrode.PositionY - 1;

                // traverse the array for matching electrodes
                for (int i = 0; i < electrodeBoard.Length; i++)
                {

                    // for each electrode calculate the margin of coordinates that it holds
                    int minMarginX = electrodeBoard[i].PositionX;
                    int minMarginY = electrodeBoard[i].PositionY;
                    int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                    int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                    // check bottom side coordinate for matches
                    if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                    {
                        // we're working to the right, therefore we define an accumulator for the x size
                        int accumulativeSizeX = -electrodeBoard[i].SizeX;

                        // start by adding the neighbour straight below the given electrode
                        neighbours.Add(electrodeBoard[i].ID1);

                        // start from the previously added neighbour and find the neighbours to the right of it
                        neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    }

                    // check top side coordinate for matches
                    if (searchStraightTopPosX > minMarginX && searchStraightTopPosX < maxMarginX && searchStraightTopPosY > minMarginY && searchStraightTopPosY < maxMarginY)
                    {
                        // we're working to the right, therefore we define an accumulator for the x size
                        int accumulativeSizeX = -electrodeBoard[i].SizeX;

                        // start by adding the neighbour straight above the given electrode
                        neighbours.Add(electrodeBoard[i].ID1);

                        // start from the previously added neighbour and find the neighbours to the right of it
                        neighbours.AddRange(findBottomRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, electrode.SizeX));
                    }
                }
            }
            
            // remove the electrode itself, since it will be found when looking for neighbours downwards from the electrode just above it
            neighbours.Remove(electrode.ID1);
          

            return neighbours;
        }

        private ArrayList findTopRightNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeX, int mainSizeX)
        {
            // initialize neighbour array
            ArrayList neighbours = new ArrayList();

            // define the coordinates of the bottom right corner of electrode in question
            int searchRightPosX = electrode.PositionX + electrode.SizeX + 1;
            int searchRightPosY = electrode.PositionY + electrode.SizeY - 1;

            // add the size of previously found neighbour, to check that we haven't run past the border of our original electrode
            accumulativeSizeX += electrode.SizeX;

            // return if we are now past the border of our original electrode
            if (accumulativeSizeX >= mainSizeX)
            {
                
                return neighbours;
            }

            // traverse the electrode array
            for (int i = 0; i < electrodeBoard.Length; i++)
            {

                // for each electrode calculate the margin of coordinates that it holds
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                // check right side coordinate for matches
                if (searchRightPosX > minMarginX && searchRightPosX < maxMarginX && searchRightPosY > minMarginY && searchRightPosY < maxMarginY)
                {
                    // add the neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // recursively check right side neighbours of the newly found neighbour until the accumulative size surpasses the size of the original electrode
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, mainSizeX));
                }
                
            }
            
            return neighbours;
        }

        private ArrayList findBottomRightNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeX, int mainSizeX)
        {

            // initialize neighbour array
            ArrayList neighbours = new ArrayList();

            // define the coordinates of the top right corner of electrode in question
            int searchRightPosX = electrode.PositionX + electrode.SizeX + 1;
            int searchRightPosY = electrode.PositionY + 1;

            // add the size of previously found neighbour, to check that we haven't run past the border of our original electrode
            accumulativeSizeX += electrode.SizeX;

            // return if we are now past the border of our original electrode
            if (accumulativeSizeX >= mainSizeX)
            {
                return neighbours;
            }

            // traverse the electrode array
            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                // for each electrode calculate the margin of coordinates that it holds
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;


                // check right side coordinate for matches
                if (searchRightPosX > minMarginX && searchRightPosX < maxMarginX && searchRightPosY > minMarginY && searchRightPosY < maxMarginY)
                {
                    // add the neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // recursively check right side neighbours of the newly found neighbour until the accumulative size surpasses the size of the original electrode
                    neighbours.AddRange(findTopRightNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeX, mainSizeX));
                }

            }

            return neighbours;
        }

        private ArrayList findRightSideDownwardsNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeY, int mainSizeY)
        {
            // initialize neighbour array
            ArrayList neighbours = new ArrayList();

            // define the coordinates of the bottom right corner of electrode in question
            int searchBottomPosX = electrode.PositionX + electrode.SizeX - 1;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;

            // add the size of previously found neighbour, to check that we haven't run past the border of our original electrode
            accumulativeSizeY += electrode.SizeY;

            // return if we are now past the border of our original electrode
            if (accumulativeSizeY >= mainSizeY)
            {
                return neighbours;
            }

            // traverse the electrode array
            for (int i = 0; i < electrodeBoard.Length; i++)
            {

                // for each electrode calculate the margin of coordinates that it holds
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;



                // check bottom right side coordinate for matches
                if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    // add the neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // recursively check bottom right side neighbours of the newly found neighbour until the accumulative size surpasses the size of the original electrode
                    neighbours.AddRange(findRightSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, mainSizeY));
                }

            }

            return neighbours;
        }

        private ArrayList findLeftSideDownwardsNeighbourByElectrode(Electrodes[] electrodeBoard, Electrodes electrode, int accumulativeSizeY, int mainSizeY)
        {
            // initialize neighbour array
            ArrayList neighbours = new ArrayList();

            // define the coordinates of the bottom left corner of electrode in question
            int searchBottomPosX = electrode.PositionX + electrode.SizeX - 1;
            int searchBottomPosY = electrode.PositionY + electrode.SizeY + 1;

            // add the size of previously found neighbour, to check that we haven't run past the border of our original electrode
            accumulativeSizeY += electrode.SizeY;

            // return if we are now past the border of our original electrode
            if (accumulativeSizeY >= mainSizeY)
            {
                return neighbours;
            }

            // traverse the electrode array
            for (int i = 0; i < electrodeBoard.Length; i++)
            {

                // for each electrode calculate the margin of coordinates that it holds
                int minMarginX = electrodeBoard[i].PositionX;
                int minMarginY = electrodeBoard[i].PositionY;
                int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;



                // check bottom left side coordinate for matches
                if (searchBottomPosX > minMarginX && searchBottomPosX < maxMarginX && searchBottomPosY > minMarginY && searchBottomPosY < maxMarginY)
                {
                    // add the neighbour found
                    neighbours.Add(electrodeBoard[i].ID1);

                    // recursively check bottom left side neighbours of the newly found neighbour until the accumulative size surpasses the size of the original electrode
                    neighbours.AddRange(findLeftSideDownwardsNeighbourByElectrode(electrodeBoard, electrodeBoard[i], accumulativeSizeY, mainSizeY));
                }

            }

            return neighbours;
        }


    }
}
