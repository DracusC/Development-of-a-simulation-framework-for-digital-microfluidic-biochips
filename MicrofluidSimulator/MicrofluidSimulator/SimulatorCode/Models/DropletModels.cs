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

            ArrayList toSplitToo = new ArrayList();
            foreach (int n in dropletElectrode.neighbours)
            {
                int indexForElectrode  = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(n, container);
                Electrode tempElectrode = electrodeBoard[indexForElectrode];
                

                
                if (tempElectrode.status > 0 && (ElectrodeModels.electrodeHasDroplet(tempElectrode,container) == null) && allowSplit(container,caller))
                {
                    toSplitToo.Add(indexForElectrode);
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

                Droplets newDroplet = new Droplets("test droplet", id, "h20", electrodeCenterX, electrodeCenterY, 0, 0, color, origin.temperature, 0, tempElectrode.ID, origin.group);
                droplets.Add(newDroplet);
                subscribers.Add(newDroplet.ID);
                container.subscribedDroplets.Add(newDroplet.ID);
                int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(id, container);

                SubscriptionModels.dropletSubscriptions(container, newDroplet);
            }
            DropletUtillityFunctions.updateGroupNumber(container, origin, origin.group);
            DropletUtillityFunctions.updateGroupVolume( container, origin.group, 0);
            return subscribers;
        }

        

        

        

        private static bool allowSplit(Container container, Droplets caller)
        {
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            float areaOfElectrode = Models.ElectrodeModels.getAreaOfElectrode(dropletElectrode);
            if (areaOfElectrode * 0.7 < caller.volume)
            {
                return true;
            }

            return false;
        }




       



        

       
    }
}
