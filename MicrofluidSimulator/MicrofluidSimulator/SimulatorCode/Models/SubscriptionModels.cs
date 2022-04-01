using MicrofluidSimulator.SimulatorCode.DataTypes;
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
            foreach(int n in dropletSubscritions)
            {
                electrodeBoard[n].subscriptions.Remove(caller.ID);
                //actuators[n].Subscriptions.Remove(caller.ID1); // Carl this is not correct
            }
            caller.subscriptions = new ArrayList();

            //Add the new subscriptions

            //find the index of the electrode we are on
            
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID,container);
            
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            // add the droplet to the sublist of the electrode it is on then run for all neighbours
            dropletElectrode.subscriptions.Add(caller.ID);
            caller.subscriptions.Add(dropletElectrodeIndex);

            foreach (int neighbour in dropletElectrode.neighbours)
            {
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                electrodeBoard[index].subscriptions.Add(caller.ID);
                caller.subscriptions.Add(index);
            }
        }
    }
}
// LOL