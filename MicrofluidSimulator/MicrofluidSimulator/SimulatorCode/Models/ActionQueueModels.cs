using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Text.Json; 
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ActionQueueModels
    {
        // method to add an action queue in the front of the original action queue
        public static Queue<ActionQueueItem> pushActionQueueToStartOfOriginalActionQueue(Queue<ActionQueueItem> originalActionQueue, Queue<ActionQueueItem> actionQueueToPush, float timeSkip)
        {
            Queue<ActionQueueItem> originalActionQueueToPush = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(originalActionQueue);
            // traverse the queue
            foreach (ActionQueueItem itemToPush in originalActionQueueToPush)
            {
                
                itemToPush.time += timeSkip;
                Console.WriteLine("DEQUING ITEM "+ itemToPush.action.actionName + " , " + itemToPush.action.actionOnID + ", " + itemToPush.time);
                // add the original queues elements to the end of the new queue
                actionQueueToPush.Enqueue(itemToPush);
            }

            return actionQueueToPush;
        }

        // method to add an action in the front of the action queue
        public static Queue<ActionQueueItem> pushActionToStartOfOriginalActionQueue(Queue<ActionQueueItem> originalActionQueue, ActionQueueItem actionToPush, float timeSkip)
        {
            // initialzie the new queue we want to return
            Queue<ActionQueueItem> newActionQueue = new Queue<ActionQueueItem>();
            // push the action to the start of the queue
            newActionQueue.Enqueue(actionToPush);
            // traverse the original queue
            for (int i = 0; i < originalActionQueue.Count+1; i++)
            {
                ActionQueueItem itemToPush = originalActionQueue.Dequeue();
                itemToPush.time += timeSkip;
                // add the original queues elements to the end of the new queue
                newActionQueue.Enqueue(itemToPush);
            }

            return newActionQueue;
        }


        public String getJsonDescriptionOfElementById(Container container, int id)
        {
            // traverse the elecrodes linearly
            for(int i = 0; i < container.electrodes.Length; i++)
            {
                // find the electrode in question
                if(container.electrodes[i].ID == id)
                {
                    // return the serialized electrode
                    return JsonSerializer.Serialize(container.electrodes[i]);
                }
            }
            // traverse the droplets linearly
            for (int i = 0; i < container.droplets.Count; i++)
            {
                Droplets droplet = (Droplets)container.droplets[i];
                // find the droplet in question
                if (droplet.ID == id)
                {
                    // return the serialized droplet
                    return JsonSerializer.Serialize(droplet);
                }
            }
            // traverse the sensors linearly
            for (int i = 0; i < container.sensors.Length; i++)
            {
                // find the sensor in question
                if (container.sensors[i].ID == id)
                {
                    // return the serialized sensor
                    return JsonSerializer.Serialize(container.sensors[i]);
                }
            }
            // traverse the actuators linearly
            for (int i = 0; i < container.actuators.Length; i++)
            {
                // find the actuator in question
                if (container.actuators[i].ID == id)
                {
                    // return the serialized sensor
                    return JsonSerializer.Serialize(container.actuators[i]);
                }
            }
            return "not found";
        }
    }
}
