using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Reflection;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        
        public Simulator(Queue<ActionQueueItem> actionQueue, Container container, ElectrodesWithNeighbours[] electrodesWithNeighbours, string generatedActionQueue)
        {
            

            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(container, electrodesWithNeighbours);
            this.actionQueue = generateTestQueueFromReader(generatedActionQueue, container);

            this.droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;


            
            
            this.container = container;

            this.initialActionQueue = new Queue<ActionQueueItem>(this.actionQueue);
            this.initialContainer = HelpfullRetreiveFunctions.createCopyOfContainer(container);
            ((Heater)container.actuators[0]).SetTargetTemperature(100);
            ((Heater)container.actuators[2]).SetTargetTemperature(100);
            simulatorRunAllModels();

            //Console.WriteLine("initialContainer info" + this.initialContainer.droplets[0].positionX);
            //restartSimulator(container, electrodesWithNeighbours, generatedActionQueue);
            //ArrayList containerConfigurations = new ArrayList();
        }

        public void restartSimulator()
        {
            
            this.container = HelpfullRetreiveFunctions.createCopyOfContainer(this.initialContainer);
            this.actionQueue = new Queue<ActionQueueItem>(this.initialActionQueue);
            this.droplets = this.container.droplets;
            simulatorRunAllModels();



        }
        
        public Container container { get; set; }
        public List<Droplets> droplets { get; set; }
        public Queue<ActionQueueItem> actionQueue { get; set; }

        public Queue<ActionQueueItem> initialActionQueue { get; set; }

        public Container initialContainer { get; set; }

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
            float maximumTimeStep = 1f;
            // only execute if action exists in queue
            float targetTime = container.currentTime + timeStepLength;
            bool executeAStep = false;

            
            if (timeStepLength == -1)
            {
                if (actionQueue.Count > 1)
                {
                    ActionQueueItem actionPeek = actionQueue.Peek();
                    targetTime = actionPeek.time;
                    executeAStep = true;
                }
                else
                {
                    targetTime = container.currentTime;
                    executeAStep = false;
                }
            }



            while (targetTime > container.currentTime || executeAStep)
            {
                ArrayList subscribers = new ArrayList();
                
                if (actionQueue.Count > 1)
                {
                    container.timeStep = 0;
                    ActionQueueItem actionPeekForTime = actionQueue.Peek();
                    if (actionPeekForTime.time == container.currentTime)
                    {
                        // store the first action in the queue and dequeue it
                        bool noMoreActions = false;
                        ActionQueueItem action = actionQueue.Dequeue();

                        // print the timestamp of the action we're about to execute
                        Console.WriteLine("action number " + action.time);

                        //containerConfigurations.Add(container);
                        //Get the first action execute and get back the list of subscribers to the specific action
                        subscribers = executeAction(action, container);
                        executeAStep = false;
                        while (!noMoreActions)
                        {
                            if (actionQueue.Count > 0)
                            {
                                ActionQueueItem actionPeek = actionQueue.Peek();
                                if (action.time == actionPeek.time)
                                {
                                    ActionQueueItem nextAction = actionQueue.Dequeue();
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
                        subscribers = container.subscribedDroplets;
                        if(actionPeekForTime.time > targetTime)
                        {
                            container.timeStep = targetTime - container.currentTime;
                            executeAStep = false;
                        }else 
                        {
                            container.timeStep = actionPeekForTime.time - container.currentTime;
                            executeAStep = true;
                        }
                    }
                }
                else
                {
                    //get subscribers to delta time
                    //subscribers = new ArrayList();
                    container.timeStep = targetTime - container.currentTime;
                    subscribers = container.subscribedDroplets;
                    executeAStep = false;
                }


                //check if we step a too large time amount
                if (container.timeStep > maximumTimeStep)
                {
                    container.timeStep = maximumTimeStep;
                }

 
                


                Queue<int> subscriberQueue = new Queue<int>();
                foreach (int subscriber in subscribers)
                {
                    subscriberQueue.Enqueue(subscriber);
                }


                //Console.WriteLine("subscriber queue " + subscriberQueue.Count());
                while (subscriberQueue.Count() > 0)
                {
                    int subscriber = subscriberQueue.Dequeue();
                    //Console.WriteLine("SUBSCRIBERS " + subscriber);
                    int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                    if (index != -1)
                    {
                        //Console.WriteLine("INDEX " + index);
                        //Console.WriteLine("DROPLETS LENGTH" + droplets.Count);
                        Droplets droplet = (Droplets)droplets[index];
                        handelSubscriber(container, droplet, subscriberQueue);
                    }
                }

                
                
                foreach (int actuatorID in container.subscribedActuators)
                {
                    Actuators actuator = HelpfullRetreiveFunctions.getActuatorById(container, actuatorID);
                    if(actuator != null)
                    {
                        executeActuatorModel(container, actuator);
                    }
                    
                }

                
                // initial move of droplets
                foreach (Bubbles bubble in container.bubbles)
                {
                    foreach (Droplets droplet in container.droplets)
                    {
                        
                        if (BubbleModels.bubbleIsOneDroplet(droplet, bubble))
                        {
                            int rightPosX = droplet.positionX + GlobalVariables.RECTANGULARELECTRODESIZE;
                            int rightPosY = droplet.positionY;
                            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, rightPosX, rightPosY)){
                                bubble.positionX = droplet.positionX - 1;
                            }

                            int leftPosX = droplet.positionX - GlobalVariables.RECTANGULARELECTRODESIZE;
                            int leftPosY = droplet.positionX;
                            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, leftPosX, leftPosY))
                            {
                                bubble.positionX = droplet.positionX + 1;
                            }

                            int bottomPosX = droplet.positionX;
                            int bottomPosY = droplet.positionX + GlobalVariables.RECTANGULARELECTRODESIZE;
                            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, bottomPosX, bottomPosY))
                            {
                                bubble.positionX = droplet.positionY - 1;
                            }

                            int topPosX = droplet.positionX;
                            int topPosY = droplet.positionX - GlobalVariables.RECTANGULARELECTRODESIZE;
                            if (HelpfullRetreiveFunctions.hasNeighbouringDroplet(container, topPosX, topPosY))
                            {
                                bubble.positionX = droplet.positionY + 1;
                            }

                            BubbleModels.moveBubble(droplet, bubble);
                            



                        }

                    }

                }

                // merge bubbles
                foreach (int bubbleID in container.subscribedBubbles)
                {
                    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleById(container, bubbleID);
                    if (bubble != null && bubble.toRemove == false)
                    {
                        executeBubbleModel(container, bubble, container.bubbles);
                    }

                }
                ArrayList subscribedBubbles = HelpfullRetreiveFunctions.copyOfSubscribedBubbles(container.subscribedBubbles);
                foreach (int bubbleID in subscribedBubbles)
                {
                    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleById(container, bubbleID);
                    if (bubble != null && bubble.toRemove == true)
                    {
                        container.bubbles.Remove(bubble);
                        container.subscribedBubbles.Remove(bubbleID);
                    }

                }

                
                container.currentTime = container.currentTime + container.timeStep;

            }


            
            
            
            
        }

        private void executeBubbleModel(Container container, Bubbles bubble, List<Bubbles> bubbles)
        {
            BubbleModels.bubbleMerge(container, bubble, bubbles);
            
        }

        private void executeActuatorModel(Container container, Actuators actuator)
        {
            
            switch (actuator.type)
            {
                case "heater":
                    
                    ((Heater)actuator).SetPowerStatus();
                    HeaterActuatorModels.heaterTemperatureChange(container, (Heater) actuator);
                    break;
            }
            
        }

        public void simulatorRunAllModels()
        {
            Console.WriteLine("This method was inacted based on an edit of variables!");
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodes = container.electrodes;
            Console.WriteLine("Electrode with id:"+ electrodes[194].ID + " has status: " + electrodes[194].status);
            foreach (Droplets droplet in droplets)
            {
                SubscriptionModels.dropletSubscriptions(container, droplet);
            }

            container.timeStep = 0;
            ArrayList subscribers = container.subscribedDroplets;
            Queue<int> subscriberQueue = new Queue<int>();
            foreach (int subscriber in subscribers)
            {
                subscriberQueue.Enqueue(subscriber);
                //Console.WriteLine("ENQUING " + subscriber);
            }


            //Console.WriteLine("subscriber queue " + subscriberQueue.Count());
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

            int num = 0;
            foreach(int i in container.subscribedDroplets)
            {
                num++;
            }
            Console.WriteLine("there are now :" + num + " droplets");
        }


        private void handelSubscriber(Container container, Droplets caller, Queue<int> subscriber)
        {
            if(caller.nextModel < caller.modelOrder.Count())
            {
                String nextModel = caller.modelOrder[caller.nextModel];
                caller.nextModel++;
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
                        caller.nextModel = 0;
                    }

                }
                else
                {
                    caller.nextModel = 0;
                }
                
            }
            else
            {
                caller.nextModel = 0;
            }
            
        }

        private ArrayList executeModel(Container container, Droplets caller, String model)
        {
            switch (model)
            {
                case "split":
                    return Models.DropletModels.dropletSplit(container, caller);
                case "merge":
                    return Models.DropletMerge.dropletMerge(container, caller);
                case "temperature":
                    return Models.DropletTemperatureModels.dropletTemperatureChange(container, caller);
                case "color":
                    return Models.DropletColorModels.dropletColorChange(container, caller);
                case "makeBubble":
                    return Models.BubbleModels.makeBubble(container, caller);
                    ;
            }
            return null;
        }

        /* Switch that reads the action and determines what needs to be calles*/
        private ArrayList executeAction(ActionQueueItem action, Container container)
        {
            String actionName = action.action.actionName;
            float lastHeaterCallTime = 0;
            switch (actionName)
            {
                case "electrode":
                    return executeElectrodeAction(action, container);
                    break;
                //case "heater":

                //    return executeHeaterAction(action, container, lastHeaterCallTime);
                // this should be called by an api instead
                    break;

            }
            return null;
        }

        // this method should be refactored to an api call
        //private ArrayList executeHeaterAction(ActionQueueItem actionQueueItem, Container container, float lastHeaterCallTime)
        //{
        //    // get the electrodes
        //    DataTypes.Actuators[] actuators = container.actuators;
        //    // initialize action
        //    SimulatorAction action = actionQueueItem.action;
        //    int actuatorId = Models.HelpfullRetreiveFunctions.getIndexOfActuatorByID(action.actionOnID, container);
            
        //    // get the subscribers for the electrode flip
        //    ArrayList subscribers = Models.HeaterActuatorModels.heaterTemperatureCalled(container, (Heater)actuators[actuatorId], action);
        //    return subscribers;
        //}


        private ArrayList executeElectrodeAction(ActionQueueItem actionQueueItem, Container container)
        {
            // get the electrodes
            Electrode[] electrodeBoard = container.electrodes;
            // initialize action
            DataTypes.SimulatorAction action = actionQueueItem.action;
            // get the id of the electro of which the action is executed on
            int electrodeId = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfElectrodeByID(action.actionOnID, container);
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
                        
                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodId(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 1);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }
                    else if (words[1].Equals("clrel"))
                    {
                        
                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodId(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 0);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }
                    
                    
                    
                    
                }
                
                counter++;
            }
            SimulatorAction action12 = new SimulatorAction("electrode", 419, 1);
            ActionQueueItem item12 = new ActionQueueItem(action12, timeStep+1);
            SimulatorAction action1 = new SimulatorAction("electrode", 451, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, timeStep+2);
            SimulatorAction action13 = new SimulatorAction("electrode", 419, 0);
            ActionQueueItem item13 = new ActionQueueItem(action13, timeStep+3);
            SimulatorAction action2 = new SimulatorAction("electrode", 483, 1);
            ActionQueueItem item2 = new ActionQueueItem(action2, timeStep+4);
            SimulatorAction action3 = new SimulatorAction("electrode", 451, 0);
            ActionQueueItem item3 = new ActionQueueItem(action3, timeStep+5);
            SimulatorAction action4 = new SimulatorAction("electrode", 483, 0);
            ActionQueueItem item4 = new ActionQueueItem(action3, timeStep + 6);

            actionQueueInstructions.Enqueue(item12);
            actionQueueInstructions.Enqueue(item1);
            actionQueueInstructions.Enqueue(item13);
            actionQueueInstructions.Enqueue(item2);
            actionQueueInstructions.Enqueue(item3);
            actionQueueInstructions.Enqueue(item4);


            return actionQueueInstructions;

        }
    }

    
}

