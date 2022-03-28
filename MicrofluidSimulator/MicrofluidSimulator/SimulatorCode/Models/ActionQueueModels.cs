using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Text.Json; 
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ActionQueueModels
    {
        public Queue<ActionQueueItem> pushActionQueueToStartOfOriginalActionQueue(Queue<ActionQueueItem> originalActionQueue, Queue<ActionQueueItem> actionQueueToPush)
        {
            for(int i = 0; i < originalActionQueue.Count; i++)
            {
                actionQueueToPush.Enqueue(originalActionQueue.Dequeue());
            }

            return actionQueueToPush;
        }

        public Queue<ActionQueueItem> pushActionToStartOfOriginalActionQueue(Queue<ActionQueueItem> originalActionQueue, ActionQueueItem actionToPush)
        {
            Queue<ActionQueueItem> newActionQueue = new Queue<ActionQueueItem>();
            newActionQueue.Enqueue(actionToPush);
            for (int i = 0; i < originalActionQueue.Count+1; i++)
            {
                newActionQueue.Enqueue(originalActionQueue.Dequeue());
            }

            return newActionQueue;
        }

        public String getJsonDescriptionOfElementById(Container container, int id)
        {
            for(int i = 0; i < container.Electrodes.Length; i++)
            {
                if(container.Electrodes[i].ID1 == id)
                {
                    return JsonSerializer.Serialize(container.Electrodes[i]);
                }
            }
            for (int i = 0; i < container.Droplets.Count; i++)
            {
                Droplets droplet = (Droplets)container.Droplets[i];
                if (droplet.ID1 == id)
                {
                    return JsonSerializer.Serialize(droplet);
                }
            }
            for (int i = 0; i < container.Sensors.Length; i++)
            {
                if (container.Sensors[i].ID1 == id)
                {
                    return JsonSerializer.Serialize(container.Sensors[i]);
                }
            }
            for (int i = 0; i < container.Actuators.Length; i++)
            {
                if (container.Actuators[i].ID1 == id)
                {
                    return JsonSerializer.Serialize(container.Actuators[i]);
                }
            }
            return "not found";
        }
    }
}
