using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models

{
    public class ElectrodeModels
    {
        public static ArrayList electrodeOnOff(Container values, Electrodes electrode, DataTypes.SimulatorAction action)
        {
            electrode.Status = action.ActionChange;
            //if (action.ActionChange != 0)
            //{
                return electrode.Subscriptions;
            //}
            //ArrayList empty = new ArrayList();
            //return empty;
        }
        public static bool electrodeHasDroplet(Electrodes Caller, Container container)
        {
            ArrayList droplets = container.Droplets;
            foreach (Droplets droplet in droplets)
            {
                if (droplet.ElectrodeID == Caller.ID1)
                {
                    return true;
                }
            }
            return false;
        }


    }

    
}
