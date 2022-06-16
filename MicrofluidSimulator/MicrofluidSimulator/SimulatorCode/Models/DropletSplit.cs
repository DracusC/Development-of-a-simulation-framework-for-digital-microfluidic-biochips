using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletSplit
    {
        /// <summary>
        /// Model that decides when to split and splits the droplets
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static ArrayList dropletSplit(Container container, Droplets caller)
        {
            //split function for the droplet
            ArrayList subscribers = new ArrayList
            {
                caller.ID
            };

            int minimalSplitVolume = 0;
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;

            //get index of electrode we are on and get the electrode
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            ArrayList toSplitToo = new ArrayList();
            foreach (int n in dropletElectrode.neighbours)
            {
                int indexForElectrode  = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(n, container);
                Electrode tempElectrode = electrodeBoard[indexForElectrode];
                

                
                if (tempElectrode.status > 0 && (ElectrodeModels.electrodeHasDroplet(tempElectrode,container) == null) && DropletUtillityFunctions.dropletOverlapElectrode(caller,tempElectrode))
                {
                    toSplitToo.Add(indexForElectrode);
                }
     
            }
            if(toSplitToo.Count > 0)
            {
                ArrayList newSubscribers = splitDroplet(container, toSplitToo, caller);

                foreach (int subscriber in newSubscribers)
                {
                    subscribers.Add(subscriber);
                }
                return subscribers;
            }
            //Part of the code that allows big droplets to split off smaller droplets, if a droplet is subscribed to more than 5 electrodes.
            else if(caller.subscriptions.Count > 5)
            {
                foreach(int sub in caller.subscriptions)
                {
                    Electrode tempElectrode = electrodeBoard[sub];
                    if(tempElectrode.status == 1 && (ElectrodeModels.electrodeHasDroplet(tempElectrode, container) == null) && caller.volume > 1080)
                    {
                        bool allowBorderSplit  = false;
                        //loop that ensures the electrode want to split to is on the border of the big droplet,
                        // by checking all neigbouring electrodes and see if the droplet also overlapps them
                        foreach(int neighbour in tempElectrode.neighbours)
                        {
                            int indexForElectrode = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                            Electrode neigbourElectrode = electrodeBoard[indexForElectrode];
                            if (!DropletUtillityFunctions.dropletOverlapElectrode(caller, neigbourElectrode))
                            {
                                allowBorderSplit = true;
                                break;
                            }
                        }
                        if (allowBorderSplit)
                        {
                            int[] centerOfElectrode = ElectrodeModels.getCenterOfElectrode(tempElectrode);
                            int electrodeCenterX = centerOfElectrode[0];
                            int electrodeCenterY = centerOfElectrode[1];

                            Random rnd = new Random();
                            int id = rnd.Next(10000000);
                            string color = caller.color;
                            int diam = DropletUtillityFunctions.getDiameterOfDroplet(360);
                            Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, diam, diam, color, caller.temperature, (int) (ElectrodeModels.getAreaOfElectrode(tempElectrode)*0.9), tempElectrode.ID, id, caller.accumulatingBubbleEscapeVolume);
                            droplets.Add(newDroplet);
                            subscribers.Add(newDroplet.ID);
                            container.subscribedDroplets.Add(newDroplet.ID);
                            int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(id, container);

                            DropletUtillityFunctions.updateGroupNumber(container, newDroplet, newDroplet.group);

                            DropletTemperatureModels.updateGroupTemperature(container, caller.group, caller);
                            SubscriptionModels.dropletSubscriptions(container, newDroplet);

                            DropletUtillityFunctions.updateGroupVolume(container, caller.group, -360);
                        }
                        
                    }
                }
            }
            return subscribers;

        }
        /// <summary>
        /// function that creates the new droplets based on the logic from dropletSplit
        /// </summary>
        /// <param name="container"></param>
        /// <param name="toSplitToo"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static ArrayList splitDroplet(Container container, ArrayList toSplitToo, Droplets origin)
        {
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;

            ArrayList subscribers = new ArrayList();

            int indexForElectrode = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(origin.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[indexForElectrode];

            //Check that ensures that a sufficent amount of electrodes are on, in case of a big droplet
            //Fx a big droplet will need multiple electrodes to move it
            if (DropletUtillityFunctions.getAreaOfDroplet(origin)/3 > DropletUtillityFunctions.findAreaOfAllConnectedElectrodes(container, dropletElectrode, new ArrayList()))
            {
                return subscribers;
            }

            {
                //Spawns the new droplets
                foreach (int i in toSplitToo)
                {

                    Electrode tempElectrode = electrodeBoard[i];

                    int[] centerOfElectrode = ElectrodeModels.getCenterOfElectrode(tempElectrode);
                    int electrodeCenterX = centerOfElectrode[0];
                    int electrodeCenterY = centerOfElectrode[1];

                    //give a random id to the new droplet
                    Random rnd = new Random();
                    int id = rnd.Next(10000000);
                    string color = origin.color;

                    //creates the droplet and adds it to the data in container
                    Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, 0, 0, color, origin.temperature, 0, tempElectrode.ID, origin.group, origin.accumulatingBubbleEscapeVolume);
                    droplets.Add(newDroplet);
                    subscribers.Add(newDroplet.ID);
                    container.subscribedDroplets.Add(newDroplet.ID);
                    int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(id, container);

                    //Initialize subscriptions of the new droplet
                    SubscriptionModels.dropletSubscriptions(container, newDroplet);
                }
                //Update group number origin droplet and all connected droplet, this is in case that the split have connected two groups together
                DropletUtillityFunctions.updateGroupNumber(container, origin, origin.group);
                
                
                Color originColor = ColorTranslator.FromHtml(origin.color);
                //New droplets are spawned with volume 0, this call ensueres that all droplet in a group get their respective share of the volume
                DropletUtillityFunctions.updateGroupColor(container, origin.group, originColor, 0);
                DropletUtillityFunctions.updateGroupVolume(container, origin.group, 0);
            }

            
            return subscribers;
        }




    }
}
