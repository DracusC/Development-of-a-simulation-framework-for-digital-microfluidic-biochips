using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Globalization;
using System.Collections;
using MicrofluidSimulator.SimulatorCode.Models;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{


    public class Simulator
    {
        /// <summary>
        /// Constructor for the simulator
        /// </summary>
        /// <param name="actionQueue"></param>
        /// <param name="container"></param>
        /// <param name="electrodesWithNeighbours"></param>
        /// <param name="generatedActionQueue"></param>
        public Simulator(Queue<ActionQueueItem> actionQueue, Container container, ElectrodesWithNeighbours[] electrodesWithNeighbours, string generatedActionQueue, string browserLanguage)
        {

            
            //Initialize all data, board of electrodes, droplets etc.
            Initialize.Initialize init = new Initialize.Initialize();
            container = init.initialize(container, electrodesWithNeighbours);

            if(generatedActionQueue != null)
            {
                this.actionQueue = ActionQueueGenerator.generateActionQueueFromReader(generatedActionQueue, container, browserLanguage);
            }
            else
            {
                this.actionQueue = ActionQueueGenerator.generateActionQueue();
            }


            this.droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;


            
            
            this.container = container;
            this.initialActionQueue = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(this.actionQueue);
            //this.initialActionQueue = new Queue<ActionQueueItem>(this.actionQueue);
            this.initialContainer = HelpfullRetreiveFunctions.createCopyAndResetContainer(container);


            simulatorStep(-2);
            
            
        }

        /// <summary>
        /// Function to restart the simulator reinitializing the container and all values
        /// </summary>
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


        /// <summary>
        /// Simulator step allows the GIU to run the simulator for a given time amount 
        /// </summary>
        /// <param name="timeStepLength"></param>
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
                
                

                
                // Run models for actuators
                foreach (int actuatorID in container.subscribedActuators)
                {
                    Actuators actuator = HelpfullRetreiveFunctions.getActuatorByID(container, actuatorID);
                    if(actuator != null)
                    {
                        executeActuatorModel(container, actuator);
                    }
                    
                }


                // Get the subscribed bubles and run their models
                ArrayList bubbleSubscribers = container.subscribedBubbles;
                Queue<int> bubbleSubscribersQueue = new Queue<int>();
                foreach (int subscriber in bubbleSubscribers)
                {
                    bubbleSubscribersQueue.Enqueue(subscriber);
                }

                while (bubbleSubscribersQueue.Count > 0)
                {
                    int subscriber = bubbleSubscribersQueue.Dequeue();
                    Bubbles bubble = HelpfullRetreiveFunctions.getBubbleByID(container, subscriber);

                    if (bubble != null && bubble.toRemove == false)
                    {

                        handleBubbleSubscriber(container, bubble, bubbleSubscribersQueue);
                    }
                }


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
        /// <summary>
        /// Switch for executing models for bubbles
        /// New models should be added to switch in order to be run correctly
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="model"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Switch for executing models for Actuators
        /// New models should be added to switch in order to be run correctly
        /// </summary>
        /// <param name="container"></param>
        /// <param name="actuator"></param>
        private static void executeActuatorModel(Container container, Actuators actuator)
        {
            
            switch (actuator.type)
            {
                case "heater":
                    
                    
                    HeaterActuatorModels.heaterTemperatureChange(container, (Heater) actuator);
                    break;
            }
            
        }


        /// <summary>
        /// Function that takes a subscribed droplet, and execute the next model for it in order.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="subscriber"></param>
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

        /// <summary>
        /// Switch for executing models for Droplets
        /// New models should be added to switch in order to be run correctly
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static ArrayList executeModel(Container container, Droplets caller, String model)
        {
            switch (model)
            {
                case "split":
                    return Models.DropletSplit.dropletSplit(container, caller); ;
                case "merge":
                    return Models.DropletMerge.dropletMerge(container, caller);
                case "temperature":
                    return Models.DropletTemperatureModels.dropletTemperatureChange(container, caller); ;
                case "color":
                    return Models.DropletColorModels.dropletColorChange(container, caller);
                case "makeBubble":
                    return Models.DropletEvaporationModels.makeBubble(container, caller);
            }
            return null;
        }


        /// <summary>
        /// Switch that reads the action and determines what needs to be calles
        /// </summary>
        /// <param name="action"></param>
        /// <param name="container"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Execute changes to electrodes from the action queue
        /// </summary>
        /// <param name="actionQueueItem"></param>
        /// <param name="container"></param>
        /// <returns></returns>
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

