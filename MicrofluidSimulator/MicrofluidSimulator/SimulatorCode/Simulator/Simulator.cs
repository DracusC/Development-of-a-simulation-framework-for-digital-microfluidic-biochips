using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        Container container;
        Droplets[] droplets;
        Queue<ActionQueueItem> actionQueue;
        public Simulator(Queue<ActionQueueItem> actionQueue, JsonContainer jsonContainer)
        {
            this.actionQueue = generateTestQueue();

            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(jsonContainer);
            droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
        }

        public Container Container { get => container; set => container = value; }
        public Droplets[] Droplets { get => droplets; set => droplets = value; }
        public Queue<ActionQueueItem> ActionQueue { get => actionQueue; set => actionQueue = value; }

        //public void simulatorRun(Queue<ActionQueueItem> actionQueue)
        //{
        //    //test queue
        //    actionQueue = generateTestQueue();

        //    //Initialize all data, board of electrodes, droplets etc.
        //    Initialize.Initialize init = new Initialize.Initialize();
        //    Container container = init.initialize();
        //    Droplets[] droplets = container.Droplets;
        //    Electrodes[] electrodeBoard = container.Electrodes;   

        //    //Loop that runs for all actions in the actionQueue
        //    foreach(ActionQueueItem action in actionQueue)
        //    {
        //        Console.WriteLine("action number " + action.Time);

        //        //Get the first action execute and get back the list of subscribers to the specific action
        //        ArrayList subscribers = executeAction(action, container);
        //        ArrayList subscribersCopy = new ArrayList();
        //        foreach (int subscriber in subscribers)
        //        {
        //            subscribersCopy.Add(subscriber);
        //        }

        //        foreach (int subscriber in subscribersCopy)
        //        {
        //            Droplets droplet = droplets[subscriber];
        //            MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMovement(container, droplet);
        //            Models.SubscriptionModels.dropletSubscriptions(container, droplet);
        //            Console.WriteLine(droplet.ToString());
        //            ArrayList dropletSubscritions = droplet.Subscriptions;
        //        }
        //    }
        //    //DataTypes.ActionQueueItem action = actionQueue[0];
        //    //executeAction(action, initValues);
        //}

        public void simulatorStep()
        {
            if(actionQueue.Count != 0)
            {
                ActionQueueItem action = actionQueue.Dequeue();

                Console.WriteLine("action number " + action.Time);

                //Get the first action execute and get back the list of subscribers to the specific action
                ArrayList subscribers = executeAction(action, container);
                ArrayList subscribersCopy = new ArrayList();
                foreach (int subscriber in subscribers)
                {
                    subscribersCopy.Add(subscriber);
                }

                foreach (int subscriber in subscribersCopy)
                {
                    Droplets droplet = droplets[subscriber];
                    MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMovement(container, droplet);
                    Models.SubscriptionModels.dropletSubscriptions(container, droplet);
                    //Console.WriteLine(droplet.ToString());
                    ArrayList dropletSubscritions = droplet.Subscriptions;
                }
            }    
            
            
        }

        /* Switch that reads the action and determines what needs to be calles*/
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

            SimulatorAction action5 = new SimulatorAction("electrode", 98, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 1, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 3, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 2, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 130, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 98, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10);
            actionQueueInstructions.Enqueue(item10);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 130, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 98, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);






            return actionQueueInstructions;
        }
    }
}
