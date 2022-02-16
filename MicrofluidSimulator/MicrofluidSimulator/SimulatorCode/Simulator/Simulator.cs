using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{
    public class Simulator
    {
        public void simulatorRun(DataTypes.ActionQueueItem[] actionQueue)
        {

            Initialize.Initialize init = new Initialize.Initialize();
            Container initValues = init.initialize();
            Droplets[] droplets = initValues.Droplets;
            Electrodes[] electrodeBoard = initValues.Electrodes;

            //DataTypes.ActionQueueItem action = actionQueue[0];
            //executeAction(action, initValues);
        }

        private void executeAction(ActionQueueItem action, Container container)
        {
            String actionName = action.Action.ActionName;
            switch (actionName)
            {
                case "electrode":
                    executeElectrodeAction(action, container);
                    break;
            }
        }

        private void executeElectrodeAction(ActionQueueItem actionQueueItem, Container container)
        {
            Electrodes[] electrodeBoard = container.Electrodes;
            DataTypes.Action action = actionQueueItem.Action;
            int electrodeId = action.ActionOnID;
            ArrayList subscribers = Models.ElectrodeModels.electrodeOnOff(container, electrodeBoard[electrodeId],action);


        }
    }
}
