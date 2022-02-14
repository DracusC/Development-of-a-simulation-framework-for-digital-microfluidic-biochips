using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{
    public class Simulator
    {
        public void simulatorRun(DataTypes.ActionQueueItem[] actionQueue)
        {
            Initialize.Initialize init = new Initialize.Initialize();
            object[] initValues = init.initialize();
            Droplets[] droplets = (Droplets[]) initValues[1];
            Electrodes[,] electrodeBoard = (Electrodes[,])initValues[0];

            DataTypes.ActionQueueItem action = actionQueue[0];
            executeAction(action, initValues);
        }

        private void executeAction(ActionQueueItem action, object[] values)
        {
            String actionName = action.Action.ActionName;
            switch (actionName)
            {
                case "electrode":
                    executeElectrodeAction(action, values);
                    break;
            }
        }

        private void executeElectrodeAction(ActionQueueItem action, object[] values)
        {
            Electrodes[,] electrodeBoard = (Electrodes[,])initValues[0];
            ArrayList electrodeOnOff = electrodeOnOff(values, );

        }
    }
}
