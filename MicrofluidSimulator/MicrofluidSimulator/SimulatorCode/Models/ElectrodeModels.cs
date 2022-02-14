using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models

{
    public class ElectrodeModels
    {
        public static ArrayList electrodeOnOff(object[] values, Electrodes electrode, DataTypes.Action action)
        {
            electrode.Status = action.ActionChange;
            return electrode.Subscriptions;
        }
    }
}
