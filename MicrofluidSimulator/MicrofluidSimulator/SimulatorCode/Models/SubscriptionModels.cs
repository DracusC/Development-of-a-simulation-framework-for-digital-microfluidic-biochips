using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        public static void dropletSubscriptions(Container container, Droplets caller)
        {
            // Gets the needed data out of the container
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            Actuators[] actuators = container.actuators;

            // Clear all previous subscriptions before adding new to avoid duplicates
            // and deprecated subscriptions
            ArrayList dropletSubscritions = caller.subscriptions;
            foreach (int n in dropletSubscritions)
            {
                electrodeBoard[n].subscriptions.Remove(caller.ID);
                //actuators[n].Subscriptions.Remove(caller.ID1); // Carl this is not correct
            }
            caller.subscriptions = new ArrayList();

            //Add the new subscriptions

            //find the index of the electrode we are on

            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);

            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            //dropletElectrode.subscriptions.Add(caller.ID);
            //caller.subscriptions.Add(dropletElectrodeIndex);
            ArrayList alreadyChecked = new ArrayList();
            //alreadyChecked.Add(caller.ID);
            makeNewSubscribtions(container, caller, dropletElectrode, alreadyChecked);

            // add the droplet to the sublist of the electrode it is on then run for all neighbours
            //dropletElectrode.subscriptions.Add(caller.ID);
            //caller.subscriptions.Add(dropletElectrodeIndex);

            //foreach (int neighbour in dropletElectrode.neighbours)
            //{
            //    int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
            //    electrodeBoard[index].subscriptions.Add(caller.ID);
            //    caller.subscriptions.Add(index);
            //}
        }

        private static void makeNewSubscribtions(Container container, Droplets droplet, Electrode electrode, ArrayList alreadyChecked)
        {
            
            if (droplet.electrodeID == electrode.ID && !alreadyChecked.Contains(electrode.ID))
            {
                alreadyChecked.Add(electrode.ID);
                electrode.subscriptions.Add(droplet.ID);
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(electrode.ID, container);
                droplet.subscriptions.Add(index);
                foreach (int neighbour in electrode.neighbours)
                {
                    if (!alreadyChecked.Contains(neighbour))
                    {
                        Electrode[] electrodeBoard = container.electrodes;
                        int indexNeigbhour = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                        Electrode electrodeNeigbour = electrodeBoard[indexNeigbhour];
                        makeNewSubscribtions(container, droplet, electrodeNeigbour, alreadyChecked);
                    }

                }
            }
            else
            {
                alreadyChecked.Add(electrode.ID);
                if (DropletUtillityFunctions.dropletOverlapElectrode(container, droplet, electrode))
                {
                    electrode.subscriptions.Add(droplet.ID);
                    int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(electrode.ID, container);
                    droplet.subscriptions.Add(index);
                    foreach (int neighbour in electrode.neighbours)
                    {
                        if (!alreadyChecked.Contains(neighbour))
                        {
                            Electrode[] electrodeBoard = container.electrodes;
                            int indexNeigbhour = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                            Electrode electrodeNeigbour = electrodeBoard[indexNeigbhour];
                            makeNewSubscribtions(container, droplet, electrodeNeigbour, alreadyChecked);
                        }

                    }

                }
            }
            

            


        }


        private static bool pointIsInsideDroplet(Container container, Droplets droplet, int x, int y)
        {
            if ((x - droplet.positionX) * (x - droplet.positionX) +
                    (y - droplet.positionY) * (y - droplet.positionY) <= droplet.sizeX * droplet.sizeX)
                return true;
            else
                return false;
        }


    }

    // Function to return the minimum distance
    // between a line segment AB and a point E
    



}



// LOL