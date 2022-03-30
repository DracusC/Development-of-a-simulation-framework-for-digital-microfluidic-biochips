﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes;
using System.Collections;
using System.Reflection;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        Container container;
        ArrayList droplets;
        Queue<ActionQueueItem> actionQueue;
        //ArrayList containerConfigurations;
        //Queue<ActionQueueItem> oldActionQueue;
        public Simulator(Queue<ActionQueueItem> actionQueue, JsonContainer jsonContainer, ElectrodesWithNeighbours[] electrodesWithNeighbours, string generatedActionQueue)
        {
            

            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(jsonContainer, electrodesWithNeighbours);
            this.actionQueue = generateTestQueueFromReader(generatedActionQueue, container);
            droplets = container.Droplets;
            Electrodes[] electrodeBoard = container.Electrodes;
            //ArrayList containerConfigurations = new ArrayList();
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
        public void simulatorStep(float timeStepLength)
        {
            // only execute if action exists in queue
            float targetTime = container.CurrentTime + timeStepLength;
            bool executeAStep = false;


            if (timeStepLength == -1)
            {
                if (actionQueue.Count > 1)
                {
                    ActionQueueItem actionPeek = actionQueue.Peek();
                    targetTime = actionPeek.Time;
                    executeAStep = true;
                }
                else
                {
                    targetTime = container.CurrentTime;
                    executeAStep = false;
                }
            }



            while (targetTime > container.CurrentTime || executeAStep)
            {
                ArrayList subscribers = new ArrayList();

                if (actionQueue.Count > 1)
                {
                    ActionQueueItem actionPeekForTime = actionQueue.Peek();
                    if (actionPeekForTime.Time == container.CurrentTime)
                    {
                        // store the first action in the queue and dequeue it
                        bool noMoreActions = false;
                        ActionQueueItem action = actionQueue.Dequeue();

                        // print the timestamp of the action we're about to execute
                        Console.WriteLine("action number " + action.Time);

                        //containerConfigurations.Add(container);
                        //Get the first action execute and get back the list of subscribers to the specific action
                        subscribers = executeAction(action, container);
                        executeAStep = false;
                        while (!noMoreActions)
                        {
                            if (actionQueue.Count > 0)
                            {
                                ActionQueueItem actionPeek = actionQueue.Peek();
                                if (action.Time == actionPeek.Time)
                                {
                                    ActionQueueItem nextAction = actionQueue.Dequeue();
                                    Console.WriteLine("they are at the same time");
                                    ArrayList extraSubscribers = executeAction(nextAction, container);
                                    foreach (int subscriber in extraSubscribers)
                                    {
                                        if (!subscribers.Contains(subscriber))
                                        {
                                            subscribers.Add(subscriber);
                                        }
                                    }
                                }
                                else
                                {
                                    noMoreActions = true;
                                }
                            }
                            else
                            {
                                noMoreActions = true;
                            }
                        }
                        
                    }
                    else
                    {
                        //get subscribers to delta time
                        //subscribers = new ArrayList();
                        subscribers = container.SubscribedDroplets;
                        if(actionPeekForTime.Time > targetTime)
                        {
                            executeAStep = false;
                        }else 
                        {
                            container.TimeStep = actionPeekForTime.Time - container.CurrentTime;
                            executeAStep = true;
                        }
                    }
                }
                else
                {
                    //get subscribers to delta time
                    //subscribers = new ArrayList();
                    container.TimeStep = targetTime - container.CurrentTime;
                    subscribers = container.SubscribedDroplets;
                    executeAStep = false;
                }



 
                


                Queue<int> subscriberQueue = new Queue<int>();
                foreach (int subscriber in subscribers)
                {
                    subscriberQueue.Enqueue(subscriber);
                }


                Console.WriteLine("subscriber queue " + subscriberQueue.Count());
                while (subscriberQueue.Count() > 0)
                {
                    int subscriber = subscriberQueue.Dequeue();
                    int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                    if (index != -1)
                    {
                        Droplets droplet = (Droplets)droplets[index];
                        handelSubscriber(container, droplet, subscriberQueue);
                    }
                }

                container.CurrentTime = container.CurrentTime + container.TimeStep;

            }


            
            
            
            
        }


        private void handelSubscriber(Container container, Droplets caller, Queue<int> subscriber)
        {
            if(caller.NextModel < caller.ModelOrder.Count())
            {
                String nextModel = caller.ModelOrder[caller.NextModel];
                caller.NextModel++;
                ArrayList newSubscribers = executeModel(container,caller,nextModel);
                if(newSubscribers != null)
                {
                    if (newSubscribers.Count > 0)
                    {
                        foreach (int i in newSubscribers)
                        {
                            subscriber.Enqueue(i);
                        }
                    }
                    else
                    {
                        caller.NextModel = 0;
                    }

                }
                else
                {
                    caller.NextModel = 0;
                }
                
            }
            else
            {
                caller.NextModel = 0;
            }
            
        }

        private ArrayList executeModel(Container container, Droplets caller, String model)
        {
            switch (model)
            {
                case "split":
                    return Models.DropletModels.dropletSplit(container, caller);
                case "merge":
                    return Models.DropletModels.dropletMerge(container, caller);
                case "temperature":
                    return Models.DropletTemperatureModels.dropletTemperatureChange(container, caller);
                case "color":
                    return Models.DropletColorModels.dropletColorChange(container, caller);
                    ;
            }
            return null;
        }

        /* Switch that reads the action and determines what needs to be calles*/
        private ArrayList executeAction(ActionQueueItem action, Container container)
        {
            String actionName = action.Action.ActionName;
            float lastHeaterCallTime = 0;
            switch (actionName)
            {
                case "electrode":
                    return executeElectrodeAction(action, container);
                    break;
                case "heater":

                    return executeHeaterAction(action, container, lastHeaterCallTime);
                    break;

            }
            return null;
        }

        private ArrayList executeHeaterAction(ActionQueueItem actionQueueItem, Container container, float lastHeaterCallTime)
        {
            // get the electrodes
            DataTypes.Actuators[] actuators = container.Actuators;
            // initialize action
            SimulatorAction action = actionQueueItem.Action;
            int actuatorId = Models.HelpfullRetreiveFunctions.getIndexOfActuatorByID(action.ActionOnID, container);
            
            // get the subscribers for the electrode flip
            ArrayList subscribers = Models.HeaterModels.heaterTemperatureCalled(container, (Heater)actuators[actuatorId], action);
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
            SimulatorAction action1 = new SimulatorAction("electrode", 100, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 101, 1);
            ActionQueueItem item2 = new ActionQueueItem(action2, 1);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 34, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 2, 1);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 2, 0);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 100, 1);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 102, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 103, 1);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 104, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 101, 1);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10);
            actionQueueInstructions.Enqueue(item10);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 9);
            actionQueueInstructions.Enqueue(item9);






            return actionQueueInstructions;
        }
        private Queue<ActionQueueItem> generateTestQueueFromReader(string generatedActionQueue, Container container)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            
            int counter = 0;
            int timeStep = 0;
            foreach (string line in generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            { 
            
                if (counter % 2 == 0)
                {
                    timeStep++;
                }
                string[] words = line.Split(' ');
                for(int i = 3; i < words.Length-1; i++)
                {
                   
                    if (words[1].Equals("setel"))
                    {
                        Console.WriteLine("timeStep " + timeStep + "action " + words[1] + "electrodeId" + words[i]);
                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodId(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 1);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }
                    else if (words[1].Equals("clrel"))
                    {
                        Console.WriteLine("timeStep " + timeStep + "action " + words[1] + "electrodeId" + words[i]);
                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodId(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 0);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }
                    
                    
                    
                    
                }
                
                counter++;
            }

            return actionQueueInstructions;

        }
    }

    
}

