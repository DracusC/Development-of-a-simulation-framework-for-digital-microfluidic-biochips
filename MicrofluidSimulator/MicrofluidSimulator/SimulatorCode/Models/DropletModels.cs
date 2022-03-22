using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletModels
    {
        public static void dropletSplit(Container container, Droplets caller)
        {
            int minimalSplitVolume = 0;
            ArrayList droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;

            //get index of electrode we are on and get the electrode
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.ElectrodeID, container);
            Electrodes dropletElectrode = electrodeBoard[dropletElectrodeIndex];
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
            foreach (int n in dropletElectrode.Neighbours)
            {
                int indexForElectrode  = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(n, container);
                Electrodes tempElectrode = electrodeBoard[indexForElectrode];
                
                
                if (tempElectrode.Status > 0 && !ElectrodeModels.electrodeHasDroplet(tempElectrode,container))
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

            splitDroplet(container, toSplitToo,caller);


        }

        public static void splitDroplet(Container container, ArrayList toSplitToo, Droplets origin)
        {
            foreach(int i in toSplitToo)
            {
                ArrayList droplets = container.Droplets;
                Electrodes[] electrodeBoard = container.Electrodes;

                Electrodes tempElectrode = electrodeBoard[i];

                int[] centerOfElectrode = ElectrodeModels.getCenterOfElectrode(tempElectrode);
                int electrodeCenterX = centerOfElectrode[0];
                int electrodeCenterY = centerOfElectrode[1];

                Random rnd = new Random();
                int id = rnd.Next(10000000);
                string color = origin.Color;
                //float vol = origin.Volume;
                //origin.Volume = vol/2;
                //int diam = getDiameterOfDroplet(vol / 2, 1);
                //origin.SizeX = diam;
                //origin.SizeY = diam;
                Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, 0, 0, color, 20, 0, tempElectrode.ID1, origin.Group);
                droplets.Add(newDroplet);
                int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(id, container);
                //((Droplets)droplets[index]).ElectrodeID = tempElectrode.ID1;
                SubscriptionModels.dropletSubscriptions(container, newDroplet);
            }
            updateGroupVolume( container, origin.Group, 0);
        }

        public static void updateGroupVolume(Container container, int groupID, float extraVolume)
        {
            ArrayList droplets = container.Droplets;
            ArrayList groupMembers = findGroupMembers(container, groupID);
            float volume = getGroupVolume(container, groupMembers) + extraVolume;
            float newVolume = volume/groupMembers.Count;
            int diam = getDiameterOfDroplet(newVolume, 1);
            foreach (Droplets droplet in groupMembers)
            {
                droplet.Volume = newVolume;
                droplet.SizeX = diam;
                droplet.SizeY = diam;
            }

        }

        static ArrayList findGroupMembers(Container container, int groupID)
        {
            ArrayList groupMembers = new ArrayList();
            ArrayList droplets = container.Droplets;
            foreach (Droplets droplet in droplets)
            {   
                if(droplet.Group == groupID)
                { 
                    groupMembers.Add(droplet);
                }
            }
            return groupMembers;

        }

        public static float getGroupVolume(Container container, ArrayList groupMembers)
        {
            ArrayList droplets = container.Droplets;

            float volume = 0;
            foreach (Droplets droplet in groupMembers)
            {
                volume = volume + droplet.Volume;
            }
            return volume;
        }

        public static void dropletMerge(Container container, Droplets caller)
        {
            ArrayList droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.ElectrodeID, container);
            Electrodes dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            if (dropletElectrode.Status == 0)
            {
                ArrayList onNeighbours = new ArrayList();
                foreach (int neighbour in dropletElectrode.Neighbours)
                {
                    int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                    Electrodes electrode = electrodeBoard[electrodeIndex];
                    if(electrode.Status != 0)
                    {
                        onNeighbours.Add(neighbour);
                    }
                }
                if(onNeighbours.Count > 0)
                {
                    ArrayList dropletSubscritions = caller.Subscriptions;
                    foreach (int n in dropletSubscritions)
                    {
                        //Console.WriteLine("!!sub remove!!"+ n);
                        //foreach(int i in electrodeBoard[n].Subscriptions)
                        //{
                        //    Console.WriteLine("contain " + i);
                        //}
                        //Console.WriteLine("ID"+caller.ID1);
                        electrodeBoard[n].Subscriptions.Remove(caller.ID1);
                        //Console.WriteLine("AFTER");
                        //foreach (int i in electrodeBoard[n].Subscriptions)
                        //{
                        //    Console.WriteLine("contain " + i);
                        //}
                    }
                    //caller.Subscriptions = new ArrayList();
                    float volume = caller.Volume;
                    int groupId = caller.Group;
                    droplets.Remove(caller);
                    updateGroupVolume(container, groupId, volume);

                }
            }
        }

        static int countOnNeigbours(Electrodes[] electrodeBoard, Electrodes electrode)
        {
            int onNeighbours = 0;
            foreach (int neighbour in electrode.Neighbours)
            {
                if (electrodeBoard[neighbour].Status > 0)
                {
                    onNeighbours++;
                }
            }
            return onNeighbours;
        }



        public static string dropletColorChange(Container container, Droplets caller)
        {
            ArrayList groupColors = new ArrayList();
            ArrayList groupMembers = findGroupMembers(container, caller.Group);

            foreach (Droplets droplet in groupMembers)
            {   
                groupColors.Add(ColorTranslator.FromHtml(droplet.Color));
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

            
            return ColorTranslator.ToHtml(Color.FromArgb(r, g, b));
        }







        public static void dropletMovement(Container container, Droplets caller)
        {

            ArrayList droplets = container.Droplets;
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
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                electrodeCenterX = electrodeBoard[index].PositionX + 10;
                electrodeCenterY = electrodeBoard[index].PositionY + 10;
                dist = electrodeDistance(electrodeCenterX, electrodeCenterY, caller.PositionX, caller.PositionY);
                attraction = electrodeBoard[index].Status / (dist + 0.1);
                if (attraction > max)
                {
                    max = attraction;
                    electrode = electrodeBoard[index];
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
