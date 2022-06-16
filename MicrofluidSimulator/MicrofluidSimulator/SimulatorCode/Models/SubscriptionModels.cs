using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        /// <summary>
        /// Model to initialize/update subscribtions of a droplet
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        public static void dropletSubscriptions(Container container, Droplets caller)
        {
            // Gets the needed data out of the container
            
            Electrode[] electrodeBoard = container.electrodes;
            

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

            // Generate the subscriptions
            makeNewSubscribtions(container, caller, dropletElectrode, new ArrayList());


        }
        /// <summary>
        /// Function used to subscribe a droplet to electrodes, this is used in the dropletSubscriptions
        /// </summary>
        /// <param name="container"></param>
        /// <param name="droplet"></param>
        /// <param name="electrode"></param>
        /// <param name="alreadyChecked"></param>
        private static void makeNewSubscribtions(Container container, Droplets droplet, Electrode electrode, ArrayList alreadyChecked)
        {
            //Recursive function that runs through the elctrodes that have the droplet overlappin them until all have been found
            //

            //Check for the electrode the droplet is on
            if (droplet.electrodeID == electrode.ID && !alreadyChecked.Contains(electrode.ID))
            {

                alreadyChecked.Add(electrode.ID);
                //Add the droplet as a subscriber to the electrode
                electrode.subscriptions.Add(droplet.ID);
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(electrode.ID, container);

                //Add the index of the electrode for cleanup
                droplet.subscriptions.Add(index);

                // for each of the neighbours to the electrode do the recursive call
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
                //If the droplet overlaps an electrode
                if (DropletUtillityFunctions.dropletOverlapElectrode(droplet, electrode))
                {
                    //Add the droplet as a subscriber to the electrode
                    electrode.subscriptions.Add(droplet.ID);
                    int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(electrode.ID, container);

                    //Add the index of the electrode for cleanup
                    droplet.subscriptions.Add(index);

                    // for each of the neighbours to the electrode do the recursive call
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


    }
}