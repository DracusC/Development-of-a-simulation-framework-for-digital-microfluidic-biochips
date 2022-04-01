using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletModels
    {
        public static ArrayList dropletSplit(Container container, Droplets caller)
        {   
            ArrayList subscribers = new ArrayList();   
            subscribers.Add(caller.ID);

            int minimalSplitVolume = 0;
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;

            //get index of electrode we are on and get the electrode
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            //Console.WriteLine("CHeck split for " + caller.ID1);
            //int onNeighbours = countOnNeigbours(electrodeBoard, dropletElectrode);
            //if(onNeighbours  != 0)
            //{
            //    if (dropletElectrode.Status > 0)
            //    {
            //        onNeighbours++;
            //    }

            if (minimalSplitVolume != 0)
            {

            }
            int cur = 0;

            ArrayList toSplitToo = new ArrayList();
            foreach (int n in dropletElectrode.neighbours)
            {
                int indexForElectrode  = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(n, container);
                Electrode tempElectrode = electrodeBoard[indexForElectrode];
                
                
                if (tempElectrode.status > 0 && (ElectrodeModels.electrodeHasDroplet(tempElectrode,container) == null))
                {
                    toSplitToo.Add(indexForElectrode);

                    //int electrodeCenterX = tempElectrode.PositionX + 10;
                    //int electrodeCenterY = tempElectrode.PositionY + 10;
                    //Random rnd = new Random();
                    //int id = rnd.Next(10000000);
                    //string color = caller.Color;
                    //Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, 15, 15, color, 20, getVolumeOfDroplet(15,1), caller.Group);
                    //droplets.Add(newDroplet);
                    //int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(id,container);
                    //((Droplets)droplets[index]).ElectrodeID = tempElectrode.ID1;
                    //SubscriptionModels.dropletSubscriptions(container, newDroplet);
                    //Console.WriteLine(newDroplet.ID1);
                    //Console.WriteLine("electrode  " + newDroplet.ElectrodeID + " spawn  " + electrodeCenterX + " , " + electrodeCenterY);

                }


                    
            }

            ArrayList newSubscribers = splitDroplet(container, toSplitToo, caller);
            foreach(int subscriber in newSubscribers)
            {
                subscribers.Add(subscriber);
            }
            return subscribers;

        }

        public static ArrayList splitDroplet(Container container, ArrayList toSplitToo, Droplets origin)
        {
            ArrayList subscribers = new ArrayList();

            foreach(int i in toSplitToo)
            {
                List<Droplets> droplets = container.droplets;
                Electrode[] electrodeBoard = container.electrodes;

                Electrode tempElectrode = electrodeBoard[i];

                int[] centerOfElectrode = ElectrodeModels.getCenterOfElectrode(tempElectrode);
                int electrodeCenterX = centerOfElectrode[0];
                int electrodeCenterY = centerOfElectrode[1];

                Random rnd = new Random();
                int id = rnd.Next(10000000);
                string color = origin.color;
                //float vol = origin.Volume;
                //origin.Volume = vol/2;
                //int diam = getDiameterOfDroplet(vol / 2, 1);
                //origin.SizeX = diam;
                //origin.SizeY = diam;
                Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, 0, 0, color, 20, 0, tempElectrode.ID, origin.group);
                droplets.Add(newDroplet);
                subscribers.Add(newDroplet.ID);
                container.subscribedDroplets.Add(newDroplet.ID);
                int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(id, container);
                //((Droplets)droplets[index]).ElectrodeID = tempElectrode.ID1;
                SubscriptionModels.dropletSubscriptions(container, newDroplet);
            }
            updateGroupNumber(container, origin, origin.group);
            dropletColorChange(container, origin);
            updateGroupVolume( container, origin.group, 0);
            return subscribers;
        }

        public static void updateGroupVolume(Container container, int groupID, float extraVolume)
        {
            List<Droplets> droplets = container.droplets;
            ArrayList groupMembers = findGroupMembers(container, groupID);
            float volume = getGroupVolume(container, groupMembers) + extraVolume;
            float newVolume = volume/groupMembers.Count;
            int diam = getDiameterOfDroplet(newVolume, 1);
            foreach (Droplets droplet in groupMembers)
            {
                droplet.volume = newVolume;
                droplet.sizeX = diam;
                droplet.sizeY = diam;
            }

        }

        static ArrayList findGroupMembers(Container container, int groupID)
        {
            ArrayList groupMembers = new ArrayList();
            List<Droplets> droplets = container.droplets;
            foreach (Droplets droplet in droplets)
            {   
                if(droplet.group == groupID)
                { 
                    groupMembers.Add(droplet);
                }
            }
            return groupMembers;

        }

        public static float getGroupVolume(Container container, ArrayList groupMembers)
        {
            List<Droplets> droplets = container.droplets;

            float volume = 0;
            foreach (Droplets droplet in groupMembers)
            {
                volume = volume + droplet.volume;
            }
            return volume;
        }

        public static ArrayList dropletMerge(Container container, Droplets caller)
        {
            ArrayList subscribers = new ArrayList();
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            if (dropletElectrode.status == 0)
            {
                ArrayList onNeighbours = new ArrayList();
                foreach (int neighbour in dropletElectrode.neighbours)
                {
                    int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                    Electrode electrode = electrodeBoard[electrodeIndex];
                    if(electrode.status != 0)
                    {
                        onNeighbours.Add(neighbour);
                    }
                }
                if(onNeighbours.Count > 0)
                {
                    ArrayList dropletSubscritions = caller.subscriptions;
                    foreach (int n in dropletSubscritions)
                    {

                        electrodeBoard[n].subscriptions.Remove(caller.ID);
                    }
                    container.subscribedDroplets.Remove(caller.ID);
                    //caller.Subscriptions = new ArrayList();
                    float volume = caller.volume;
                    int groupId = caller.group;
                    int removedDropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
                    Electrode removedDropletElectrode = electrodeBoard[removedDropletElectrodeIndex];


                    droplets.Remove(caller);



                    ArrayList handeledDroplet = new ArrayList();
                    ArrayList neighbouringDroplets = new ArrayList();


                    foreach (int neigbour in removedDropletElectrode.neighbours)
                    {
                        int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neigbour, container);
                        Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];

                        Droplets tempDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                        if (tempDroplet != null)
                        {
                            neighbouringDroplets.Add(tempDroplet);
                        }
                    }

                    Droplets firstDroplet = (Droplets) neighbouringDroplets[0];

                    updateGroupNumber(container, firstDroplet, firstDroplet.group);
                    handeledDroplet.Add(firstDroplet);

                    ArrayList connectedDroplets = new ArrayList();
                    findAllConnectedDroplets(container, firstDroplet, connectedDroplets);
                    foreach (Droplets d in connectedDroplets)
                    {
                        if(!handeledDroplet.Contains(d) && neighbouringDroplets.Contains(d))
                        {
                            handeledDroplet.Add(d);
                        }
                    }

                    foreach(Droplets droplet in neighbouringDroplets)
                    {
                        if (!handeledDroplet.Contains(droplet))
                        {
                            Random rnd = new Random();
                            int id = rnd.Next(10000000);
                            updateGroupNumber(container, droplet, id);
                            handeledDroplet.Add(droplet);

                            ArrayList connectedDroplets2 = new ArrayList();
                            findAllConnectedDroplets(container, droplet, connectedDroplets2);
                            foreach (Droplets d in connectedDroplets2)
                            {
                                if (!handeledDroplet.Contains(d) && neighbouringDroplets.Contains(d))
                                {
                                    handeledDroplet.Add(d);
                                }
                            }


                        }
                    }

                    foreach (Droplets droplet in neighbouringDroplets)
                    {
                        updateGroupVolume(container, droplet.group, volume/neighbouringDroplets.Count);
                    }




                   


                }
            }
            subscribers.Add(caller.ID);
            return subscribers;
        }

        static int countOnNeigbours(Electrode[] electrodeBoard, Electrode electrode)
        {
            int onNeighbours = 0;
            foreach (int neighbour in electrode.neighbours)
            {
                if (electrodeBoard[neighbour].status > 0)
                {
                    onNeighbours++;
                }
            }
            return onNeighbours;
        }



        public static void dropletColorChange(Container container, Droplets caller)
        {
            ArrayList groupColors = new ArrayList();
            ArrayList groupMembers = findGroupMembers(container, caller.group);

            foreach (Droplets droplet in groupMembers)
            {   
                groupColors.Add(ColorTranslator.FromHtml(droplet.color));
            }

            int r = 0;
            int g = 0;
            int b = 0;
            foreach (Color c in groupColors)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }
            r /= groupColors.Count;
            g /= groupColors.Count;
            b /= groupColors.Count;

            foreach(Droplets droplet in groupMembers)
            {
                droplet.color = $"#{r:X2}{g:X2}{b:X2}";
            }


            //return $"#{r:X2}{g:X2}{b:X2}";
        }



        static void updateGroupNumber(Container container, Droplets caller, int newGroupID)
        {
            ArrayList connectedDroplets = new ArrayList();
            findAllConnectedDroplets( container,caller, connectedDroplets);
            foreach(Droplets droplet in connectedDroplets)
            {
                droplet.group = newGroupID;
            }
        }




        static void findAllConnectedDroplets(Container container, Droplets caller, ArrayList members)
        {
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            members.Add(caller);
            foreach(int neigbour in dropletElectrode.neighbours)
            {   

                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neigbour, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];

                Droplets tempDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                if(tempDroplet != null && !members.Contains(tempDroplet))
                {
                    findAllConnectedDroplets(container, tempDroplet, members);         
                }
            }
        }




        public static void dropletMovement(Container container, Droplets caller)
        {

            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int posX = caller.positionX / 20;
            int posY = caller.positionY / 20;


            

            //get index of electrode we are on and get the electrode
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            // init values for finding the electrode with highest attraction
            double max = 0;
            Electrode electrode = null;

            // Find the attraction for the electrode we are on
            // Attraction is definded by status/distance
            int electrodeCenterX = dropletElectrode.positionX + 10;
            int electrodeCenterY = dropletElectrode.positionY + 10;
            double dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.positionX, caller.positionY);
            double attraction = dropletElectrode.status / (dist + 0.1);
            //If the attraction is higher than the previous highest, this is now the highest
            if (attraction > max)
            {
                max = attraction;
                electrode = dropletElectrode;
            }

            // Find the attraction for all the neighbouring electrodes
            foreach (int neighbour in dropletElectrode.neighbours)
            {
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                electrodeCenterX = electrodeBoard[index].positionX + 10;
                electrodeCenterY = electrodeBoard[index].positionY + 10;
                dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.positionX, caller.positionY);
                attraction = electrodeBoard[index].status / (dist + 0.1);
                if (attraction > max)
                {
                    max = attraction;
                    electrode = electrodeBoard[index];
                }
            }
            // move the droplet to the electrode with the highest attraction
            if(electrode != null)
            {
                caller.positionX = electrode.positionX + 10;
                caller.positionY = electrode.positionY + 10;
                caller.electrodeID = electrode.ID;
            }
        }

        private static double electrodeDistance(int electrodeCenterX, int electrodeCenterY,int dropletX, int dropletY)
        {
            double x = Math.Pow(dropletX - electrodeCenterX,2);
            double y = Math.Pow(dropletY - electrodeCenterY,2);

            return Math.Sqrt(x+y);
        }

        public static float getVolumeOfDroplet(float diameter, float height)
        {
            float pi = (float)Math.PI;
            return ((float)pi) * ((float)Math.Pow((diameter / 2), 2)) * height;
        }

        public static int getDiameterOfDroplet(float volume, float height)
        {
            float pi = (float)Math.PI;
            return (int)(2 * (float)(Math.Sqrt(volume / (pi * height))));
        }
    }
}
