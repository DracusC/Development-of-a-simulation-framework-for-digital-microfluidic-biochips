using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        Container container;
        ArrayList droplets;
        Queue<ActionQueueItem> actionQueue;
        public Simulator(Queue<ActionQueueItem> actionQueue, JsonContainer jsonContainer, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {
            this.actionQueue = generateTestQueue();

            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(jsonContainer, electrodesWithNeighbours);
            droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
        }

        public Container Container { get => container; set => container = value; }
        public ArrayList Droplets { get => droplets; set => droplets = value; }
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

        // called from joelspage
        public void simulatorStep()
        {
            // only execute if action exists in queue
            if(actionQueue.Count != 0)
            {
                // store the first action in the queue and dequeue it
                ActionQueueItem action = actionQueue.Dequeue();

                // print the timestamp of the action we're about to execute
                Console.WriteLine("action number " + action.Time);

                //Get the first action execute and get back the list of subscribers to the specific action
                ArrayList subscribers = executeAction(action, container);

                // create a copy of the subscribers array
                ArrayList subscribersCopy = new ArrayList();
                subscribersCopy = (ArrayList)subscribers.Clone();
                
                // split all the droplets that needs to be split
                foreach (int subscriber in subscribersCopy)
                {
                    int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                    if(index != -1)
                    {
                        Droplets droplet = (Droplets)droplets[index];

                        //int actionChange = action.Action.ActionChange;
                        //if (actionChange != 0)
                        //{
                            MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletSplit(container, droplet);

                        //}
                        //else
                        //{
                           // MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMerge(container, droplet);

                        //}
                        Models.SubscriptionModels.dropletSubscriptions(container, droplet);
                        //Console.WriteLine(droplet.ToString());
                        ArrayList dropletSubscritions = droplet.Subscriptions;
                    }


                    
                }

                // merge all the droplets that needs to be merged
                foreach (int subscriber in subscribersCopy)
                {
                    int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                    if (index != -1)
                    {
                        Droplets droplet = (Droplets)droplets[index];

                        //int actionChange = action.Action.ActionChange;
                        //if (actionChange != 0)
                        //{
                        //MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMovement2(container, droplet);

                        //}
                        //else
                        //{
                        MicrofluidSimulator.SimulatorCode.Models.DropletModels.dropletMerge(container, droplet);
                        
                        Electrodes[] electrodes = container.Electrodes;
                        Console.WriteLine("!!sub remove!!" + "33" + "for sub " + subscriber);
                        foreach (int i in electrodes[33].Subscriptions)
                        {
                            Console.WriteLine("contain " + i);
                        }
                        //}
                        //.SubscriptionModels.dropletSubscriptions(container, droplet);
                        //Console.WriteLine(droplet.ToString());
                        //ArrayList dropletSubscritions = droplet.Subscriptions;
                    }



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
                case "heater":
                    return executeHeaterAction(action, container);
                    break;
            }
            return null;
        }

        private ArrayList executeHeaterAction(ActionQueueItem actionQueueItem, Container container)
        {
            // get the electrodes
            MicrofluidSimulator.SimulatorCode.DataTypes.Actuators[] actuators = container.Actuators;
            // initialize action
            DataTypes.SimulatorAction action = actionQueueItem.Action;
            int actuatorId = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfActuatorByID(action.ActionOnID, container);
            // get the subscribers for the electrode flip
            ArrayList subscribers = Models.ActuatorModels.heaterTemperatureChange(container, (Heater) actuators[actuatorId], action);
            return subscribers;
        }

        private ArrayList executeElectrodeAction(ActionQueueItem actionQueueItem, Container container)
        {
            // get the electrodes
            Electrodes[] electrodeBoard = container.Electrodes;
            // initialize action
            DataTypes.SimulatorAction action = actionQueueItem.Action;
            // get the id of the electro of which the action is executed on
            int electrodeId = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfElectrodeByID(action.ActionOnID, container);
            // get the subscribers for the electrode flip
            ArrayList subscribers = Models.ElectrodeModels.electrodeOnOff(container, electrodeBoard[electrodeId], action);
            return subscribers;

        }

        private Queue<ActionQueueItem> generateTestQueue()
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 1, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 2, 1);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 33, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 34, 1);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 35, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 36, 1);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 37, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 3, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 131, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 99, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10);
            actionQueueInstructions.Enqueue(item10);

            action9 = new SimulatorAction("electrode", 132, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 1);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);






            return actionQueueInstructions;
        }
    }
}
