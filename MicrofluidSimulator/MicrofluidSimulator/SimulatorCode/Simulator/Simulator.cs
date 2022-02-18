﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{
    public class Simulator
    {
        public void simulatorRun(Queue<ActionQueueItem> actionQueue)
        {
            actionQueue = generateTestQueue();

            Initialize.Initialize init = new Initialize.Initialize();
            Container initValues = init.initialize();
            Droplets[] droplets = initValues.Droplets;
            Electrodes[] electrodeBoard = initValues.Electrodes;

            int i = 1;
            foreach(ActionQueueItem action in actionQueue)
            {
                Console.WriteLine("action number " + i);

                ArrayList subscribers = executeAction(action, initValues);
                //foreach(int subscriber in subscribers)
                //{
                    Droplets droplet = droplets[0];
                    MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMovement(initValues, droplet);
                    Models.SubscriptionModels.dropletSubscriptions(initValues, droplet);
                    Console.WriteLine(droplet.ToString());
                ArrayList dropletSubscritions = droplet.Subscriptions;
                Console.WriteLine("subs");
                foreach (int u in dropletSubscritions)
                    {
                    Console.WriteLine(u);
                    }
                Console.WriteLine("neigh");
                ArrayList electroneibou = electrodeBoard[droplet.ElectrodeID].Neighbours;
                foreach (int u in electroneibou)
                {
                    Console.WriteLine(u);
                }
                Console.WriteLine("end");
                Console.WriteLine("Hello" +  action.Time);
                //}
                i++;
            }
            //DataTypes.ActionQueueItem action = actionQueue[0];
            //executeAction(action, initValues);
        }

        private ArrayList executeAction(ActionQueueItem action, Container container)
        {
            String actionName = action.Action.ActionName;
            switch (actionName)
            {
                case "electrode":
                    return executeElectrodeAction(action, container);
                    break;
            }
            return null;
        }

        private ArrayList executeElectrodeAction(ActionQueueItem actionQueueItem, Container container)
        {
            Electrodes[] electrodeBoard = container.Electrodes;
            DataTypes.SimulatorAction action = actionQueueItem.Action;
            int electrodeId = action.ActionOnID;
            ArrayList subscribers = Models.ElectrodeModels.electrodeOnOff(container, electrodeBoard[electrodeId],action);
            return subscribers;

        }

        private Queue<ActionQueueItem> generateTestQueue()
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 0, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 1, 1);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 0, 0);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 2, 1);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 34, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 1, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 5);
            actionQueueInstructions.Enqueue(item6);



            return actionQueueInstructions;
        }
    }
}
