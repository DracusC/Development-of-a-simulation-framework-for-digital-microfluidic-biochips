using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        public static void dropletSubscriptions(Container container, Droplets caller)
        {
            // Gets the needed data out of the container
            ArrayList droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
            Actuators[] actuators = container.Actuators;

            // Clear all previous subscriptions before adding new to avoid duplicates
            // and deprecated subscriptions
            ArrayList dropletSubscritions = caller.Subscriptions;
            foreach(int n in dropletSubscritions)
            {
                electrodeBoard[n].Subscriptions.Remove(caller.ID1);
                actuators[n].Subscriptions.Remove(caller.ID1);
            }
            caller.Subscriptions = new ArrayList();

            //Add the new subscriptions

            //find the index of the electrode we are on
            
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.ElectrodeID,container);
            
            Electrodes dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            // add the droplet to the sublist of the electrode it is on then run for all neighbours
            dropletElectrode.Subscriptions.Add(caller.ID1);
            caller.Subscriptions.Add(dropletElectrodeIndex);

            foreach (int neighbour in dropletElectrode.Neighbours)
            {
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                electrodeBoard[index].Subscriptions.Add(caller.ID1);
                caller.Subscriptions.Add(index);
            }
        }
    }
}
// LOL