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
            for(int i = 0; i < container.electrodes.Length; i++)
            {
                if(container.electrodes[i].ID == id)
                {
                    return JsonSerializer.Serialize(container.electrodes[i]);
                }
            }
            for (int i = 0; i < container.droplets.Count; i++)
            {
                Droplets droplet = (Droplets)container.droplets[i];
                if (droplet.ID == id)
                {
                    return JsonSerializer.Serialize(droplet);
                }
            }
            for (int i = 0; i < container.sensors.Length; i++)
            {
                if (container.sensors[i].ID == id)
                {
                    return JsonSerializer.Serialize(container.sensors[i]);
                }
            }
            for (int i = 0; i < container.actuators.Length; i++)
            {
                if (container.actuators[i].ID == id)
                {
                    return JsonSerializer.Serialize(container.actuators[i]);
                }
            }
            return "not found";
        }
    }
}
