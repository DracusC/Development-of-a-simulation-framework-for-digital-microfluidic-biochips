using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Text;

using System.Security.AccessControl;
namespace MicrofluidSimulator.SimulatorCode.Initialize
{
    public class NeighbourFinder
    {
        /// <summary>
        /// Function for finding neigbors of electrodes
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Container findNeighbours(Container container)
        {
            for (int i = 0; i < container.electrodes.Length; i++)
            {

                if (container.electrodes[i].shape == 0)
                {
                    container.electrodes[i].neighbours = findNeighboursByElectrode(container.electrodes, container.electrodes[i]);

                }
                else
                {

                    container.electrodes[i].neighbours = findNeighboursByElectrodePolygon(container.electrodes, container.electrodes[i]);
                }

            }
            string electrodesWithNeighbours = JsonConvert.SerializeObject(container.electrodes);
            Console.WriteLine(electrodesWithNeighbours);


            return container;
        }

        /// <summary>
        /// Function for finding neigbors of a electrodes of a non rectagular shape
        /// </summary>
        /// <param name="electrodeBoard"></param>
        /// <param name="electrode"></param>
        /// <returns></returns>
        private static ArrayList findNeighboursByElectrodePolygon(Electrode[] electrodeBoard, Electrode electrode)
        {
            ArrayList neighbours = electrode.neighbours;


            int currentPointX = electrode.corners[0][0] + electrode.positionX;
            int currentPointY = electrode.corners[0][1] + electrode.positionY;



            for (int i = 0; i < electrode.corners.Count; i++)
            {
                int vecX = electrode.corners[(i + 1) % electrode.corners.Count][0] - electrode.corners[i][0];
                int vecY = electrode.corners[(i + 1) % electrode.corners.Count][1] - electrode.corners[i][1];
                int tempVecX = vecX;
                int tempVecY = vecY;
                int divisor = gcd(Math.Abs(vecX), Math.Abs(vecY));
                vecX /= divisor;
                vecY /= divisor;

                int tempCurrentPointX = currentPointX;
                int tempCurrentPointY = currentPointY;



                while ((currentPointX != (tempCurrentPointX + tempVecX)) || ((currentPointY != (tempCurrentPointY + tempVecY))))
                {



                    for (int j = 0; j < electrodeBoard.Length; j++)
                    {

                        if (electrodeBoard[j].shape == 0)
                        {

                            // for each electrode calculate the margin of coordinates that it holds
                            int minMarginX = electrodeBoard[j].positionX;
                            int minMarginY = electrodeBoard[j].positionY;
                            int maxMarginX = electrodeBoard[j].positionX + electrodeBoard[j].sizeX;
                            int maxMarginY = electrodeBoard[j].positionY + electrodeBoard[j].sizeY;





                            // check if the i'th electrode matches the coordinate of the electrode we're finding neighbours of

                            // check top left coordinate 
                            if ((currentPointX == minMarginX + 1 && currentPointY == minMarginY || currentPointX == maxMarginX - 1 && currentPointY == maxMarginY
                            || currentPointX == minMarginX && currentPointY == maxMarginY - 1 || currentPointX == maxMarginX && currentPointY == minMarginY + 1
                            || currentPointX == minMarginX && currentPointY == minMarginY + 1 || currentPointX == maxMarginX && currentPointY == maxMarginY - 1
                            || currentPointX == minMarginX + 1 && currentPointY == maxMarginY || currentPointX == maxMarginX - 1 && currentPointY == minMarginY)
                                && !neighbours.Contains(electrodeBoard[j].ID) && electrodeBoard[j] != electrode)
                            {


                                // add neighbour found if it isnt in the array already
                                neighbours.Add(electrodeBoard[j].ID);
                                if (!electrodeBoard[j].neighbours.Contains(electrode.ID))
                                {
                                    electrodeBoard[j].neighbours.Add(electrode.ID);
                                }






                            }
                        }
                        else if (electrodeBoard[j].shape == 1 && electrodeBoard[j] != electrode)
                        {
                            int checkPointX = electrodeBoard[j].corners[0][0] + electrodeBoard[j].positionX;
                            int checkPointY = electrodeBoard[j].corners[0][1] + electrodeBoard[j].positionY;

                            for (int k = 0; k < electrodeBoard[j].corners.Count; k++)
                            {

                                int vecCheckX = electrodeBoard[j].corners[(k + 1) % electrodeBoard[j].corners.Count][0] - electrodeBoard[j].corners[k][0];
                                int vecCheckY = electrodeBoard[j].corners[(k + 1) % electrodeBoard[j].corners.Count][1] - electrodeBoard[j].corners[k][1];
                                int tempVecCheckX = vecCheckX;
                                int tempVecCheckY = vecCheckY;

                                int checkDivisor = gcd(Math.Abs(vecCheckX), Math.Abs(vecCheckY));
                                vecCheckX /= checkDivisor;
                                vecCheckY /= checkDivisor;

                                int tempCheckPointX = checkPointX;
                                int tempCheckPointY = checkPointY;


                                while ((checkPointX != (tempCheckPointX + tempVecCheckX)) || (checkPointY != (tempCheckPointY + tempVecCheckY)))
                                {
                                    
                                    if (((currentPointX == checkPointX && currentPointY == checkPointY)
                                    && !neighbours.Contains(electrodeBoard[j].ID) && electrodeBoard[j] != electrode) && !isCorner(electrode, currentPointX, currentPointY))
                                    {


                                        // add neighbour found if it isnt in the array already
                                        neighbours.Add(electrodeBoard[j].ID);





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


        /// <summary>
        /// Function to find neigbors of electrodes of a rectangular shape
        /// </summary>
        /// <param name="electrodeBoard"></param>
        /// <param name="electrode"></param>
        /// <returns></returns>
        private static ArrayList findNeighboursByElectrode(Electrode[] electrodeBoard, Electrode electrode)
        {

            // initialize neighbour array
            ArrayList neighbours = electrode.neighbours;



            // loop through all electrodes to find a matching elecrode to the given coordinates

            for (int i = 0; i < electrodeBoard.Length; i++)
            {

                // define pixel coordinate in the top left corner of the given electrode 
                int currentTopLeftPosX = electrode.positionX;
                int currentTopLeftPosY = electrode.positionY;

                // define pixel coordinate in the bottom left corner of the given electrode 
                int currentBottomRightPosX = electrode.positionX + electrode.sizeX;
                int currentBottomRightPosY = electrode.positionY + electrode.sizeY;
                if (electrodeBoard[i].shape == 0)
                {


                    // for each electrode calculate the margin of coordinates that it holds
                    int minMarginX = electrodeBoard[i].positionX;
                    int minMarginY = electrodeBoard[i].positionY;
                    int maxMarginX = electrodeBoard[i].positionX + electrodeBoard[i].sizeX;
                    int maxMarginY = electrodeBoard[i].positionY + electrodeBoard[i].sizeY;

                    // check if the i'th electrode matches the coordinate of the electrode we're finding neighbours of
                    // traverse topside of electrode
                    while (currentTopLeftPosX <= electrode.positionX + electrode.sizeX - 1)
                    {



                        if ((currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY - 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY + 1
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY + 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY - 1
                            || currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID);
                            currentTopLeftPosX += (maxMarginX - currentTopLeftPosX);
                        }
                        else
                        {
                            currentTopLeftPosX++;
                        }

                    }

                    //reset
                    currentTopLeftPosX = electrode.positionX;

                    //traverse leftside of electrode
                    while (currentTopLeftPosY <= electrode.positionY + electrode.sizeY - 1)
                    {


                        if ((currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY - 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY + 1
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY + 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY - 1
                            || currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID);
                            currentTopLeftPosY += (maxMarginY - currentTopLeftPosY);
                        }
                        else
                        {
                            currentTopLeftPosY++;
                        }
                    }
                    //reset
                    currentTopLeftPosY = electrode.positionY;
                    //traverse bottomside of electrode
                    while (currentBottomRightPosX >= electrode.positionX + 1)
                    {



                        if ((currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY - 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY + 1
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY + 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY - 1
                            || currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID);
                            currentBottomRightPosX += (minMarginX - currentBottomRightPosX);
                        }
                        else
                        {
                            currentBottomRightPosX--;
                        }
                    }
                    //reset
                    currentBottomRightPosX = electrode.positionX + electrode.sizeX;
                    //traverse rightside of electrode
                    while (currentBottomRightPosY >= electrode.positionY + 1)
                    {

                        if ((currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY - 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY + 1
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY + 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY - 1
                            || currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == minMarginY)
                            && !neighbours.Contains(electrodeBoard[i].ID) && electrodeBoard[i] != electrode)
                        {
                            neighbours.Add(electrodeBoard[i].ID);
                            currentBottomRightPosY += (minMarginY - currentBottomRightPosY);
                        }
                        else
                        {
                            currentBottomRightPosY--;
                        }
                    }
                    //reset
                    currentBottomRightPosY = electrode.positionY + electrode.sizeY;

                }



            }
            return neighbours;
        }

        /// <summary>
        /// Greatest common divisor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int gcd(int x, int y)
        {
            int gcd = 1;
            for (int i = 1; i <= x && i <= y; i++)
            {
                if (x % i == 0 && y % i == 0)
                    gcd = i;
            }
            if (x == 0)
            {
                gcd = y;
            }
            else if (y == 0)
            {
                gcd = x;
            }
            else if (x == y)
            {
                gcd = x;
            }

            return gcd;
        }

        /// <summary>
        /// Checks if a point is a corner of an electrode
        /// </summary>
        /// <param name="electrode"></param>
        /// <param name="currentPointX"></param>
        /// <param name="currentPointY"></param>
        /// <returns></returns>
        private static bool isCorner(Electrode electrode, int currentPointX, int currentPointY)
        {
            for (int i = 0; i < electrode.corners.Count; i++)
            {
                if (electrode.corners[i][0] + electrode.positionX == currentPointX && electrode.corners[i][1] + electrode.positionY == currentPointY)
                {

                    return true;
                }
            }
            return false;
        }

    }
}
