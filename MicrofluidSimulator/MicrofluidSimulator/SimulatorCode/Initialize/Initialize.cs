using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Linq;
using System.Text.Json;
using System.Numerics;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes;
using MicrofluidSimulator.SimulatorCode.DataTypes;

namespace MicrofluidSimulator.SimulatorCode.Initialize
{

    public class Initialize
    {

        public Initialize()
        {

        }

        public Container initialize(JsonContainer jsonContainer, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {

            //Electrodes[] electrodes = new Electrodes[jsonContainer.electrodes.Count];
           
            Electrodes[] electrodeBoard = initializeBoard(jsonContainer.electrodes, electrodesWithNeighbours);



            ArrayList droplets = initializeDroplets(jsonContainer.droplets);
            Console.WriteLine("actuatorname: " + jsonContainer.actuators[0].name);
            List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators> actuatorsInitial = initializeActuators(jsonContainer.actuators);

            Container container = new Container(electrodeBoard, droplets);
            if(electrodesWithNeighbours == null)
            {
                findNeighbours(container);
            }

            initializeSubscriptions(container);


            return container;
        }

        private List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators> initializeActuators(List<MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes.Actuators> actuators)
        {
            List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators> actuatorsInitial = new List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators>();
            int heaterCount = 0;
            for(int i = 0; i < actuators.Count; i++)
            {
                switch (actuators[i].type)
                {
                    case "heater":
                        
                        int[,] cornersGetter = null;
                        
                        if (actuators[i].corners != null)
                        {
                            cornersGetter = new int[actuators[i].corners.Count, 2];
                            for (int j = 0; j < actuators[i].corners.Count; j++)
                            {

                                var res = System.Text.Json.JsonSerializer.Deserialize<List<int>>(actuators[i].corners[j].ToString());
                                for (int k = 0; k < 2; k++)
                                {
                                    cornersGetter[j, k] = res[k];
                                }
                            }

                        }
                        
                        actuatorsInitial.Add(new Heater(actuators[i].name, actuators[i].ID, actuators[i].actuatorID, actuators[i].type, actuators[i].positionX,
                            actuators[i].positionY, actuators[i].sizeX, actuators[i].sizeY, actuators[i].actualTemperature, actuators[i].desiredTemperature,
                            actuators[i].status, actuators[i].nextDesiredTemperature, actuators[i].nextStatus, cornersGetter));



                        break;
                    
                }
                
            }

            return actuatorsInitial;

        }

        public class Corner
        {
            List<int> Coords { get; set; }
        }

        private Electrodes[] initializeBoard(List<Electrode> electrodes, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {


            Electrodes[] electrodeBoard = new Electrodes[electrodes.Count];
            for (int i = 0; i < electrodes.Count; i++)
            {
                //electrodeBoard[i] = new Electrodes("arrel", i, i, i, 0, (i % 32) * 20, (i / 32) * 20, 20, 20, 0, null);






                int[,] cornersGetter = new int[electrodes[i].corners.Count, 2];
                for (int j = 0; j < electrodes[i].corners.Count; j++)
                {

                    var res = System.Text.Json.JsonSerializer.Deserialize<List<int>>(electrodes[i].corners[j].ToString());
                    for (int k = 0; k < 2; k++)
                    {
                        cornersGetter[j, k] = res[k];
                    }
                }





                electrodeBoard[i] = new Electrodes(electrodes[i].name, electrodes[i].ID, electrodes[i].electrodeID, electrodes[i].driverID, electrodes[i].shape,
                electrodes[i].positionX, electrodes[i].positionY, electrodes[i].sizeX, electrodes[i].sizeY, electrodes[i].status, cornersGetter);

                if(electrodesWithNeighbours != null)
                {
                    
                    for(int j = 0; j < electrodesWithNeighbours[i].Neighbours.Count; j++)
                    {
                        electrodeBoard[i].Neighbours.Add(electrodesWithNeighbours[i].Neighbours[j]);
                    }
                    
                    //int[] neighboursGetter = new int[electrodesWithNeighbours[i].Neighbours.Count];
                    //for (int j = 0; j < electrodesWithNeighbours[i].Neighbours.Count; j++)
                    //{

                    //    var res = System.Text.Json.JsonSerializer.Deserialize<List<int>>(electrodesWithNeighbours[i].Neighbours[j].ToString());

                    //    for(int k = 0; k < res.Count; k++)
                    //    {
                    //        neighboursGetter[k] = res[k];
                    //    }

                    //    electrodeBoard[i].Neighbours.Add(neighboursGetter[j]);

                    //}

                }
                





            }

            return electrodeBoard;


        }

        private ArrayList initializeDroplets(List<MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes.Droplets> droplets)
        {
            ArrayList dropletsArray = new ArrayList();
            int i = 0;
            foreach (MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes.Droplets droplet in droplets)
            {
                dropletsArray.Add(new Droplets(droplet.name, droplet.ID, droplet.substance_name, droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY, droplet.color, droplet.temperature, DropletModels.getVolumeOfDroplet(droplet.sizeX, 1), droplet.electrodeID, i));
                i++;
            }
            //    droplets.Add(new Droplets("test droplet", 0, "h20", 120, 10, 15, 15, "blue", 20, DropletModels.getVolumeOfDroplet(15, 1), 0));
            //((Droplets)droplets[0]).ElectrodeID = 1;

            //droplets.Add(new Droplets("test droplet", 1, "h20", 30, 10, 15, 15, "blue", 20));
            //((Droplets)droplets[0]).ElectrodeID = 1;

            //droplets.Add(new Droplets("test droplet", 2, "h20", 50, 10, 15, 15, "blue", 20));
            //((Droplets)droplets[0]).ElectrodeID = 2;

            //droplets.Add(new Droplets("test droplet2", 3, "h20", 160, 70, 15, 15, "yellow", 20, DropletModels.getVolumeOfDroplet(15, 1), 1));
            //((Droplets) droplets[1]).ElectrodeID = 99;
            return dropletsArray;
        }

        

        private void initializeSubscriptions(Container container)
        {
            ArrayList droplets = container.Droplets;
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
            string electrodesWithNeighbours = JsonConvert.SerializeObject(container.Electrodes);
            Console.WriteLine(electrodesWithNeighbours);
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
                            if ((currentPointX == minMarginX + 1 && currentPointY == minMarginY || currentPointX == maxMarginX - 1 && currentPointY == maxMarginY
                            || currentPointX == minMarginX && currentPointY == maxMarginY - 1 || currentPointX == maxMarginX && currentPointY == minMarginY + 1
                            || currentPointX == minMarginX && currentPointY == minMarginY + 1 || currentPointX == maxMarginX && currentPointY == maxMarginY - 1
                            || currentPointX == minMarginX + 1 && currentPointY == maxMarginY || currentPointX == maxMarginX - 1 && currentPointY == minMarginY)
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
                                    if (((currentPointX == checkPointX && currentPointY == checkPointY)
                                    && !neighbours.Contains(electrodeBoard[j].ID1) && electrodeBoard[j] != electrode) && !isCorner(electrode, currentPointX, currentPointY))
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
                    while (currentTopLeftPosX <= electrode.PositionX + electrode.SizeX - 1)
                    {
                        


                        if ((currentTopLeftPosX == minMarginX +1 && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY - 1|| currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY + 1
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY + 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY - 1
                            || currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == minMarginY)
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
                    while (currentTopLeftPosY <= electrode.PositionY + electrode.SizeY - 1)
                    {

                        
                        if ((currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == minMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == maxMarginY
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == maxMarginY - 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == minMarginY + 1
                            || currentTopLeftPosX == minMarginX && currentTopLeftPosY == minMarginY + 1 || currentTopLeftPosX == maxMarginX && currentTopLeftPosY == maxMarginY - 1
                            || currentTopLeftPosX == minMarginX + 1 && currentTopLeftPosY == maxMarginY || currentTopLeftPosX == maxMarginX - 1 && currentTopLeftPosY == minMarginY)
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
                    while (currentBottomRightPosX >= electrode.PositionX + 1)
                    {
                        
                        
                        
                        if ((currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY - 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY + 1
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY + 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY - 1
                            || currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == minMarginY)
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
                    while (currentBottomRightPosY >= electrode.PositionY + 1)
                    {
                        
                        if ((currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == minMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == maxMarginY
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == maxMarginY - 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == minMarginY + 1
                            || currentBottomRightPosX == minMarginX && currentBottomRightPosY == minMarginY + 1 || currentBottomRightPosX == maxMarginX && currentBottomRightPosY == maxMarginY - 1
                            || currentBottomRightPosX == minMarginX + 1 && currentBottomRightPosY == maxMarginY || currentBottomRightPosX == maxMarginX - 1 && currentBottomRightPosY == minMarginY)
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
                    currentBottomRightPosY = electrode.PositionY + electrode.SizeY;

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

        private bool isCorner(Electrodes electrode, int currentPointX, int currentPointY)
        {
            for(int i = 0; i < electrode.Corners.GetLength(0); i++)
            {
                if(electrode.Corners[i, 0] + electrode.PositionX == currentPointX && electrode.Corners[i, 1] + electrode.PositionY == currentPointY)
                {
                    
                    return true;
                }
            }
            return false;
        }


    }
}
