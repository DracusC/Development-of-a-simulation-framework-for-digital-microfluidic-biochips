using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Globalization;
using System.Collections;
using MicrofluidSimulator.SimulatorCode.Models;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        
        public Simulator(Queue<ActionQueueItem> actionQueue, Container container, ElectrodesWithNeighbours[] electrodesWithNeighbours, string generatedActionQueue)
        {

            Console.WriteLine("CURRENT DIRECTORY " + Directory.GetCurrentDirectory());
            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(container, electrodesWithNeighbours);
            
            
            //this.actionQueue = ActionQueueGenerator.generateSimplePathsQueueFromReader(generatedActionQueue, container);
            this.actionQueue = ActionQueueGenerator.generateTestQueue();


            this.droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;


            
            
            this.container = container;
            this.initialActionQueue = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(this.actionQueue);
            //this.initialActionQueue = new Queue<ActionQueueItem>(this.actionQueue);
            this.initialContainer = HelpfullRetreiveFunctions.createCopyAndResetContainer(container);


            simulatorStep(-2);
            
            
        }

        public void restartSimulator()
        {
            
            this.container = HelpfullRetreiveFunctions.createCopyAndResetContainer(this.initialContainer);
            this.actionQueue = new Queue<ActionQueueItem>(this.initialActionQueue);
            this.droplets = this.container.droplets;


            simulatorStep(-2);



        }
        
        public Container container { get; set; }
        public List<Droplets> droplets { get; set; }
        public Queue<ActionQueueItem> actionQueue { get; set; }

        public Queue<ActionQueueItem> initialActionQueue { get; set; }

        public Container initialContainer { get; set; }

        //Simulator step allows the user to run the simulator for a given time amount
        public void simulatorStep(decimal timeStepLength)
        {
            decimal maximumTimeStep = 0.1m;
            // only execute if action exists in queue
            decimal targetTime = container.currentTime + timeStepLength;
            bool executeAStep = false;
            bool mustRunAllModels = false;
            bool mustRunAllModelsOnInputFromGui = false;

            //If the simulator is called with -2 run all models for all droplets, aswell as update their subscripitions etc.
            // This is for use at for example use input change to data
            if (timeStepLength == -2)
            {
                mustRunAllModelsOnInputFromGui = true;
                targetTime = container.currentTime;
                List<Droplets> droplets = container.droplets;
                foreach (Droplets droplet in droplets)
                {
                    SubscriptionModels.dropletSubscriptions(container, droplet);
                    int diam = DropletUtillityFunctions.getDiameterOfDroplet(droplet.volume);
                    droplet.sizeX = diam;
                    droplet.sizeY = diam;
                }
            }

            //If the simulator is called with -1 the simulator will run until the next action on the queue is executed
            //if there are no more actions it steps 1 second
            if (timeStepLength == -1)
            {
                if (actionQueue.Count > 0)
                {
                    ActionQueueItem actionPeek = actionQueue.Peek();
                    targetTime = actionPeek.time;
                    executeAStep = true;
                }
                else
                {
                    targetTime = container.currentTime + 1;
                    executeAStep = false;
                }
            }

            //splitTime = 0;
            //mergeTime = 0;
            //tempTime = 0;
            //colorTime = 0;
            //bubbleTime = 0;

            //var stopwatchO = new System.Diagnostics.Stopwatch();
            //stopwatchO.Start();

            //Loop that allows the simulator to exectue the models multiple times, until the requested time is reached
            while (targetTime > container.currentTime || executeAStep || mustRunAllModelsOnInputFromGui)
            {
               
                ArrayList subscribers = new ArrayList();
                //if there is actions on the queue
                if (actionQueue.Count > 0)
                {
                    container.timeStep = 0;
                    ActionQueueItem actionPeekForTime = actionQueue.Peek();
                    // if the action on the queue is at the current time
                    if (actionPeekForTime.time == container.currentTime)
                    {
                        // store the first action in the queue and dequeue it
                        mustRunAllModels = true;

                        bool noMoreActions = false;
                        ActionQueueItem action = actionQueue.Dequeue();

                        //execute the action and and get all subscribers to the it
                        ArrayList extraSubscribers = executeAction(action, container);
                        //For each of the subscribers check that they exist and store them along with their entiry group.
                        foreach (int subscriber in extraSubscribers)
                        {
                            int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                            if (index != -1)
                            {
                                Droplets droplet = (Droplets)droplets[index];
                                List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, droplet.group);
                                foreach (Droplets d in groupMembers)
                                {
                                    if (!subscribers.Contains(d.ID))
                                    {
                                        subscribers.Add(d.ID);
                                    }
                                }

                            }


                        }

                        
                        //Check if there are other actions that have to be executed on the same time
                        while (!noMoreActions)
                        {
                            if (actionQueue.Count > 0)
                            {
                                ActionQueueItem actionPeek = actionQueue.Peek();
                                // if the action on the queue is at the current time
                                if (action.time == actionPeek.time)
                                {
                                    ActionQueueItem nextAction = actionQueue.Dequeue();
                                    //execute the action and and get all subscribers to the it
                                    extraSubscribers = executeAction(nextAction, container);
                                    //For each of the subscribers check that they exist and store them along with their entiry group.
                                    foreach (int subscriber in extraSubscribers)
                                    {
                                        int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                                        if (index != -1)
                                        {
                                            Droplets droplet = (Droplets)droplets[index];
                                            
                                            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container,droplet.group);
                                            foreach(Droplets d in groupMembers)
                                            {
                                                if (!subscribers.Contains(d.ID))
                                                {
                                                    subscribers.Add(d.ID);
                                                }
                                            }
                                           
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

                        //executeAStep  is set to false, since the actions on the current time is executed
                        executeAStep = false;


                    }
                    //if there is not an action at current time increment the time step, to the target time
                    // or if there is an action before that, set the time increment to this time
                    else
                    {
                        subscribers = container.subscribedDroplets;
                        if(actionPeekForTime.time > targetTime)
                        {
                            container.timeStep = targetTime - container.currentTime;
                            mustRunAllModels = false;
                            executeAStep = false;
                        }else 
                        {
                            //if there is a action between current time, and target time, set time step to this and set executeASTep to true
                            container.timeStep = actionPeekForTime.time - container.currentTime;
                            mustRunAllModels = false;

                            executeAStep = true;
                        }
                    }
                }
                else
                {
                    //there are no actions on the queue'

                    //get subscribers to delta time, this is often all droplets
                    container.timeStep = targetTime - container.currentTime;
                    subscribers = container.subscribedDroplets;
                    mustRunAllModels = false;

                    executeAStep = false;
                }


                //check if we step a too large time amount, if it is set it to the maximum amount
                if (container.timeStep > maximumTimeStep)
                {
                    container.timeStep = maximumTimeStep;
                }

 
                

                //If no action has occured, or if ther is an order frm gui to run all models, then start the model when the time sensetive models start in order to save time
                Queue<int> subscriberQueue = new Queue<int>();
                foreach (int subscriber in subscribers)
                {
                    subscriberQueue.Enqueue(subscriber);
                    if (!mustRunAllModels && !mustRunAllModelsOnInputFromGui)
                    {
                        int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                        Droplets droplet = (Droplets)droplets[index];
                        //set the next model to where the time sensetive models start
                        droplet.nextModel = droplet.beginOfTimeSensitiveModels;
                    }
                }

                

                
                //Keep runing models for droplets as long as there is subscribed droplets on the queue
                while (subscriberQueue.Count > 0)
                {
                    int subscriber = subscriberQueue.Dequeue();
                    int index = HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
                    if (index != -1)
                    {
                        Droplets droplet = (Droplets)droplets[index];
                        handelSubscriber(container, droplet, subscriberQueue);
                    }
                }
                
                

                
                
                foreach (int actuatorID in container.subscribedActuators)
                {
                    Actuators actuator = HelpfullRetreiveFunctions.getActuatorByID(container, actuatorID);
                    if(actuator != null)
                    {
                        executeActuatorModel(container, actuator);
                    }
                    
                }


                ArrayList bubbleSubscribers = container.subscribedBubbles;
                Queue<int> bubbleSubscribersQueue = new Queue<int>();
                foreach (int subscriber in bubbleSubscribers)
                {
                    bubbleSubscribersQueue.Enqueue(subscriber);
                }



                //Console.WriteLine("subscriber queue " + subscriberQueue.Count());
                while (bubbleSubscribersQueue.Count > 0)
                {
                    int subscriber = bubbleSubscribersQueue.Dequeue();
                    //Console.WriteLine("SUBSCRIBERS " + subscriber);
                    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleByID(container, subscriber);

                    if (bubble != null && bubble.toRemove == false)
                    {

                        handleBubbleSubscriber(container, bubble, bubbleSubscribersQueue);
                    }
                }



                // merge bubbles
                //foreach (int bubbleID in container.subscribedBubbles)
                //{
                //    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleById(container, bubbleID);
                //    if (bubble != null && bubble.toRemove == false)
                //    {
                //        executeBubbleModel(container, bubble;
                //    }

                //}
                ArrayList subscribedBubbles = HelpfullRetreiveFunctions.copyOfSubscribedBubbles(container.subscribedBubbles);
                foreach (int bubbleID in subscribedBubbles)
                {
                    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleByID(container, bubbleID);
                    if (bubble != null && bubble.toRemove == true)
                    {
                        container.bubbles.Remove(bubble);
                        container.subscribedBubbles.Remove(bubbleID);
                    }

                }


                mustRunAllModelsOnInputFromGui = false;
                container.currentTime = container.currentTime + container.timeStep;

            }


            //stopwatchO.Stop();
            //Console.WriteLine("Split time: " + tempTime + " ms");
            //Console.WriteLine("Merge time: " + tempTime + " ms");
            //Console.WriteLine("Temp time: " + tempTime + " ms");
            //Console.WriteLine("Color time: " + tempTime + " ms");
            //Console.WriteLine("Bubble time: " + tempTime + " ms");

            //Console.WriteLine("While SubQ: " + stopwatchO.ElapsedMilliseconds + " ms");

        }

        
        /// <summary>
        /// Functions for handling models of components in the simulator
        /// </summary>
        /// <param name="container"></param>
        /// <param name="bubble"></param>
        /// <param name="subscriber"></param>
        private static void handleBubbleSubscriber(Container container, Bubbles bubble, Queue<int> subscriber)
        {
            if (bubble.nextModel < bubble.modelOrder.Length)
            {
                String nextModel = bubble.modelOrder[bubble.nextModel];
                bubble.nextModel++;
                ArrayList newSubscribers = executeBubbleModel(container, bubble, nextModel);
                if (newSubscribers != null)
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
                        bubble.nextModel = 0;
                    }

                }
                else
                {
                    bubble.nextModel = 0;
                }

            }
            else
            {
                bubble.nextModel = 0;
            }




        }
        private static ArrayList executeBubbleModel(Container container, Bubbles caller, String model)
        {
            switch (model)
            {
                case "move":
                    return BubbleModels.moveBubble(container, caller);
                case "merge":
                    return BubbleModels.bubbleMerge(container, caller);
                    ;
            }
            return null;
        }

        private static void executeActuatorModel(Container container, Actuators actuator)
        {
            
            switch (actuator.type)
            {
                case "heater":
                    
                    
                    HeaterActuatorModels.heaterTemperatureChange(container, (Heater) actuator);
                    break;
            }
            
        }

        //public void simulatorRunAllModels()
        //{
        //    Console.WriteLine("This method was inacted based on an edit of variables!");
        //    List<Droplets> droplets = container.droplets;
        //    Electrode[] electrodes = container.electrodes;
        //    foreach (Droplets droplet in droplets)
        //    {
        //        SubscriptionModels.dropletSubscriptions(container, droplet);
        //    }

        //    container.timeStep = 0;
        //    ArrayList subscribers = container.subscribedDroplets;
        //    Queue<int> subscriberQueue = new Queue<int>();
        //    foreach (int subscriber in subscribers)
        //    {
        //        subscriberQueue.Enqueue(subscriber);
        //        //Console.WriteLine("ENQUING " + subscriber);
        //    }

        //    //Console.WriteLine("subscriber queue " + subscriberQueue.Count());
        //    while (subscriberQueue.Count() > 0)
        //    {
        //        int subscriber = subscriberQueue.Dequeue();
        //        int index = MicrofluidSimulator.SimulatorCode.Models.HelpfullRetreiveFunctions.getIndexOfDropletByID(subscriber, container);
        //        if (index != -1)
        //        {
        //            Droplets droplet = (Droplets)droplets[index];
        //            handelSubscriber(container, droplet, subscriberQueue);
        //        }
        //    }

            

        //    int num = 0;
        //    foreach(int i in container.subscribedDroplets)
        //    {
        //        num++;
        //    }
        //    Console.WriteLine("there are now :" + num + " droplets");
        //}

        //Function that takes a subscribed droplet, and execute the next model for it in order.
        private static void handelSubscriber(Container container, Droplets caller, Queue<int> subscriber)
        {

            if(caller.nextModel < caller.modelOrder.Length)
            {
                //Get the action from the droplets own list of models
                String nextModel = caller.modelOrder[caller.nextModel];
                caller.nextModel++;

                //switch on the model recieved from the droplet
                ArrayList newSubscribers = executeModel(container,caller,nextModel);
                
                //If the model returns subscribers add these to the queue
                //if not reset the models order
                if (newSubscribers != null)
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

        private static ArrayList executeModel(Container container, Droplets caller, String model)
        {
            switch (model)
            {
                case "split":
                    //var stopwatch = new System.Diagnostics.Stopwatch();
                    //stopwatch.Start();
                    //var a = Models.DropletModels.dropletSplit(container, caller);
                    //stopwatch.Stop();
                    //splitTime += stopwatch.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("-.--------------Split time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
                    return Models.DropletModels.dropletSplit(container, caller); ;
                case "merge":
                    //var stopwatch1 = new System.Diagnostics.Stopwatch();
                    //stopwatch1.Start();
                    //a = Models.DropletMerge.dropletMerge(container, caller);
                    //mergeTime += stopwatch1.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("Merge time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
                    return Models.DropletMerge.dropletMerge(container, caller);
                case "temperature":
                    //var stopwatch2 = new System.Diagnostics.Stopwatch();
                    //stopwatch2.Start();
                    //a = Models.DropletTemperatureModels.dropletTemperatureChange(container, caller);
                    //tempTime += stopwatch2.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("Temp time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
                    return Models.DropletTemperatureModels.dropletTemperatureChange(container, caller); ;
                case "color":
                    //var stopwatch3 = new System.Diagnostics.Stopwatch();
                    //stopwatch3.Start();
                    //a = Models.DropletColorModels.dropletColorChange(container, caller);
                    //colorTime += stopwatch3.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("Color time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
                    return Models.DropletColorModels.dropletColorChange(container, caller);
                case "makeBubble":
                    //var stopwatch4 = new System.Diagnostics.Stopwatch();
                    //stopwatch4.Start();
                    //a = Models.BubbleModels.makeBubble(container, caller);
                    //bubbleTime += stopwatch4.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("Bubble time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
                    return Models.DropletEvaporationModels.makeBubble(container, caller);
            }
            return null;
        }

        /* Switch that reads the action and determines what needs to be calles*/
        private static ArrayList executeAction(ActionQueueItem action, Container container)
        {
            String actionName = action.action.actionName;
            float lastHeaterCallTime = 0;
            switch (actionName)
            {
                case "electrode":
                    return executeElectrodeAction(action, container);

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


        private static ArrayList executeElectrodeAction(ActionQueueItem actionQueueItem, Container container)
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


        
        

        
    }

    
}

