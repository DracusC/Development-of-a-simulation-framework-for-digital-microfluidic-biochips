using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        public static void dropletSubscriptions(Container container, Droplets caller)
        {   

            Droplets[] droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
            // Clear all previous subscriptions
            ArrayList dropletSubscritions = caller.Subscriptions;
            foreach(int n in dropletSubscritions)
            {
                electrodeBoard[n].Subscriptions.Remove(caller.ID1);
            }
            caller.Subscriptions = new ArrayList();
            //Add the new subscriptions
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.ElectrodeID,container);
            Electrodes dropletElectrode = electrodeBoard[dropletElectrodeIndex];
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
