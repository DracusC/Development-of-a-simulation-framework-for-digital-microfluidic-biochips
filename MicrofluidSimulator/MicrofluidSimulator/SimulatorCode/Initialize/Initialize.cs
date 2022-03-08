using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Linq;
using System.Text.Json;
using System.Numerics;

namespace MicrofluidSimulator.SimulatorCode.Initialize
{

    public class Initialize
    {

        public Initialize()
        {

        }

        public Container initialize(JsonContainer jsonContainer)
        {

            //Electrodes[] electrodes = new Electrodes[jsonContainer.electrodes.Count];
            Droplets[] droplets = new Droplets[2];
            Electrodes[] electrodeBoard = initializeBoard(jsonContainer.electrodes);



            initializeDroplets(droplets);

            Container container = new Container(electrodeBoard, droplets);
            
            findNeighbours(container);
            initializeSubscriptions(container);
            return container;
        }

        public class Corner
        {
            List<int> Coords { get; set; }
        }

        private Electrodes[] initializeBoard(List<Electrode> electrodes)
        {


            Electrodes[] electrodeBoard = new Electrodes[electrodes.Count];
            for (int i = 0; i < electrodes.Count; i++)
            {
                //electrodeBoard[i] = new Electrodes("arrel", i, i, i, 0, (i % 32) * 20, (i / 32) * 20, 20, 20, 0, null);






                int[,] cornersGetter = new int[electrodes[i].corners.Count, 2];
                for (int j = 0; j < electrodes[i].corners.Count; j++)
                {

                    var res = JsonSerializer.Deserialize<List<int>>(electrodes[i].corners[j].ToString());
                    for (int k = 0; k < 2; k++)
                    {
                        cornersGetter[j, k] = res[k];
                    }
                }





                electrodeBoard[i] = new Electrodes(electrodes[i].name, electrodes[i].ID, electrodes[i].electrodeID, electrodes[i].driverID, electrodes[i].shape,
                electrodes[i].positionX, electrodes[i].positionY, electrodes[i].sizeX, electrodes[i].sizeY, electrodes[i].status, cornersGetter);


                

            }

            return electrodeBoard;


        }

        private Droplets[] initializeDroplets(Droplets[] droplets)
        {
            droplets[0] = new Droplets("test droplet", 0, "h20", 120, 10, 15, 15, "blue", 20);
            droplets[0].ElectrodeID = 1;
            droplets[1] = new Droplets("test droplet2", 1, "h20", 140, 50, 15, 15, "yellow", 20);
            droplets[1].ElectrodeID = 3;
            return droplets;
        }

        private void initializeSubscriptions(Container container)
        {
            Droplets[] droplets = container.Droplets;

            foreach (Droplets droplet in droplets)
            {
                Models.SubscriptionModels.dropletSubscriptions(container, droplet);
                foreach(int sub in droplet.Subscriptions)
                {
                    Console.WriteLine("droplet subs id: " + droplet.ID1 + " subs: " + sub);
                }
                
            }


        }

        private Container findNeighbours(Container container)
        {
            for (int i = 0; i < container.Electrodes.Length; i++)
            {

                if (container.Electrodes[i].Shape == 0)
                {
                    container.Electrodes[i].Neighbours = findNeighboursByElectrode(container.Electrodes, container.Electrodes[i]);

                }
                else
                {
                    
                    container.Electrodes[i].Neighbours = findNeighboursByElectrodePolygon(container.Electrodes, container.Electrodes[i]);
                }

                






                }
            //for (int i = 0; i < container.Electrodes.Length; i++)
            //{
            //    if(container.Electrodes[i].ID1 == 66)
            //    {
            //        for (int j = 0; j < container.Electrodes[i].Neighbours.Count; j++)
            //        {
            //            Console.WriteLine("neighbours for  " + container.Electrodes[i].ID1 + " : " + container.Electrodes[i].Neighbours[j]);
            //        }
            //    }
                

            //}


            return container;
        }

        
        private ArrayList findNeighboursByElectrodePolygon(Electrodes[] electrodeBoard, Electrodes electrode)
        {
            ArrayList neighbours = electrode.Neighbours;


            int currentPointX = electrode.Corners[0, 0] + electrode.PositionX;
            int currentPointY = electrode.Corners[0, 1] + electrode.PositionY;

            for (int i = 0; i < electrode.Corners.GetLength(0); i++)
            {
                int vecX = electrode.Corners[(i + 1) % electrode.Corners.GetLength(0), 0] - electrode.Corners[i, 0];
                int vecY = electrode.Corners[(i + 1) % electrode.Corners.GetLength(0), 1] - electrode.Corners[i, 1];
                int tempVecX = vecX;
                int tempVecY = vecY;
                int divisor = gcd(Math.Abs(vecX), Math.Abs(vecY));
                vecX /= divisor;
                vecY /= divisor;

                int tempCurrentPointX = currentPointX;
                int tempCurrentPointY = currentPointY;

               
                
                while ((currentPointX != (tempCurrentPointX + tempVecX)) || ((currentPointY != (tempCurrentPointY + tempVecY)))){


                    
                    for (int j = 0; j < electrodeBoard.Length; j++)
                    {

                        if (electrodeBoard[j].Shape == 0)
                        {
                            
                            // for each electrode calculate the margin of coordinates that it holds
                            int minMarginX = electrodeBoard[j].PositionX;
                            int minMarginY = electrodeBoard[j].PositionY;
                            int maxMarginX = electrodeBoard[j].PositionX + electrodeBoard[j].SizeX;
                            int maxMarginY = electrodeBoard[j].PositionY + electrodeBoard[j].SizeY;





                            // check if the i'th electrode matches the coordinate of the electrode we're finding neighbours of

                            // check top left coordinate 
                            if ((currentPointX == minMarginX && currentPointY == minMarginY) || (currentPointX == maxMarginX && currentPointY == maxMarginY
                                || currentPointX == minMarginX && currentPointY == maxMarginY || currentPointX == maxMarginX && currentPointY == minMarginY)
                                && !neighbours.Contains(electrodeBoard[j].ID1) && electrodeBoard[j] != electrode)
                            {


                                // add neighbour found if it isnt in the array already
                                neighbours.Add(electrodeBoard[j].ID1);
                                if (!electrodeBoard[j].Neighbours.Contains(electrode.ID1))
                                {
                                    electrodeBoard[j].Neighbours.Add(electrode.ID1);
                                }
                                





                            }
                        }
                        else if (electrodeBoard[j].Shape == 1 && electrodeBoard[j] != electrode)
                        {
                            int checkPointX = electrodeBoard[j].Corners[0, 0] + electrodeBoard[j].PositionX;
                            int checkPointY = electrodeBoard[j].Corners[0, 1] + electrodeBoard[j].PositionY;

                            for (int k = 0; k < electrodeBoard[j].Corners.GetLength(0); k++)
                            {

                                int vecCheckX = electrodeBoard[j].Corners[(k + 1) % electrodeBoard[j].Corners.GetLength(0), 0] - electrodeBoard[j].Corners[k, 0];
                                int vecCheckY = electrodeBoard[j].Corners[(k + 1) % electrodeBoard[j].Corners.GetLength(0), 1] - electrodeBoard[j].Corners[k, 1];
                                int tempVecCheckX = vecCheckX;
                                int tempVecCheckY = vecCheckY;

                                int checkDivisor = gcd(Math.Abs(vecCheckX), Math.Abs(vecCheckY));
                                vecCheckX /= checkDivisor;
                                vecCheckY /= checkDivisor;
                                
                                int tempCheckPointX = checkPointX;
                                int tempCheckPointY = checkPointY;

                                
                                while ((checkPointX != (tempCheckPointX+ tempVecCheckX)) || (checkPointY != (tempCheckPointY+ tempVecCheckY)))
                                {
                                    //Console.WriteLine("Infinity");
                                    if ((currentPointX == checkPointX && currentPointY == checkPointY)
                                    && !neighbours.Contains(electrodeBoard[j].ID1) && electrodeBoard[j] != electrode)
                                    {


                                        // add neighbour found if it isnt in the array already
                                        neighbours.Add(electrodeBoard[j].ID1);





                                    }
                                    checkPointX += vecCheckX;
                                    checkPointY += vecCheckY;
                                }
                            }
                        }
                        

                    }
                    currentPointX += vecX;
                    currentPointY += vecY;
                   
                }

            }

            return neighbours;
        }

        private ArrayList findNeighboursByElectrode(Electrodes[] electrodeBoard, Electrodes electrode)
        {

            // initialize neighbour array
            ArrayList neighbours = electrode.Neighbours;

            

            // loop through all electrodes to find a matching elecrode to the given coordinates

            for (int i = 0; i < electrodeBoard.Length; i++)
            {

                // define pixel coordinate in the top left corner of the given electrode 
                int currentTopLeftPosX = electrode.PositionX;
                int currentTopLeftPosY = electrode.PositionY;

                // define pixel coordinate in the bottom left corner of the given electrode 
                int currentBottomRightPosX = electrode.PositionX + electrode.SizeX;
                int currentBottomRightPosY = electrode.PositionY + electrode.SizeY;
                if (electrodeBoard[i].Shape == 0)
                {


                    // for each electrode calculate the margin of coordinates that it holds
                    int minMarginX = electrodeBoard[i].PositionX;
                    int minMarginY = electrodeBoard[i].PositionY;
                    int maxMarginX = electrodeBoard[i].PositionX + electrodeBoard[i].SizeX;
                    int maxMarginY = electrodeBoard[i].PositionY + electrodeBoard[i].SizeY;

                    // check if the i'th electrode matches the coordinate of the electrode we're finding neighbours of
                    // traverse topside of electrode
                    while (currentTopLeftPosX <= electrode.PositionX + electrode.SizeX)
                    {
                        
                        

                        if ((currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID1) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID1);
                            currentTopLeftPosX += (maxMarginX - currentTopLeftPosX);
                        }
                        else
                        {
                            currentTopLeftPosX++;
                        }

                    }

                    //reset
                    currentTopLeftPosX = electrode.PositionX;

                    //traverse leftside of electrode
                    while (currentTopLeftPosY <= electrode.PositionY + electrode.SizeY)
                    {
                        if ((currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID1) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID1);
                            currentTopLeftPosY += (maxMarginY - currentTopLeftPosY);
                        }
                        else
                        {
                            currentTopLeftPosY++;
                        }
                    }
                    //reset
                    currentTopLeftPosY = electrode.PositionY;
                    //traverse bottomside of electrode
                    while (currentBottomRightPosX >= electrode.PositionX)
                    {
                        //if(electrode.ID1 == 642 && electrodeBoard[i].ID1 == 643)
                        //{
                        //    Console.WriteLine("search pos (x,y) : " + currentBottomRightPosX + ", " + currentBottomRightPosX);
                        //    Console.WriteLine("x to x: " + minMarginX + ", " + maxMarginX);
                        //    Console.WriteLine("y to y: " + minMarginY + ", " + maxMarginY);
                        //}
                        if ((currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID1) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID1);
                            currentBottomRightPosX += (minMarginX - currentBottomRightPosX);
                        }
                        else
                        {
                            currentBottomRightPosX--;
                        }
                    }
                    //reset
                    currentBottomRightPosX = electrode.PositionX + electrode.SizeX;
                    //traverse rightside of electrode
                    while (currentBottomRightPosY >= electrode.PositionY)
                    {
                        if ((currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID1) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID1);
                            currentBottomRightPosY += (minMarginY - currentBottomRightPosY);
                        }
                        else
                        {
                            currentBottomRightPosY--;
                        }
                    }
                    //reset
                    currentBottomRightPosY = electrode.PositionX + electrode.SizeX;

                }

                

            }
            return neighbours;
        }

        private int gcd(int x, int y)
        {
            int gcd = 1;
            for (int i = 1; i <= x && i <= y; i++)
            {
                if (x % i == 0 && y % i == 0)
                    gcd = i;
            }
            if(x==0)
            {
                gcd = y;
            }else if (y == 0)
            {
                gcd = x;
            }else if(x == y)
            {
                gcd = x;
            }

            return gcd;
        }


    }
}
// Carl