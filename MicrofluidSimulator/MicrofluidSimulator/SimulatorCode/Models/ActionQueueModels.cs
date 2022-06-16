using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Text.Json; 
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ActionQueueModels
    {

        
        /// <summary>
        /// Intertwines the action queue with the original action queue
        /// </summary>
        /// <param name="originalActionQueue"></param>
        /// <param name="actionQueueToPush"></param>
        /// <returns></returns>
        public static Queue<ActionQueueItem> pushActionQueueToOriginalActionQueue(Queue<ActionQueueItem> originalActionQueue, Queue<ActionQueueItem> actionQueueToPush)
        {
            Queue<ActionQueueItem> newQueue = new Queue<ActionQueueItem>();
    
            // traverse the queue
            while(originalActionQueue.Count > 0 && actionQueueToPush.Count > 0)
            {
                if(originalActionQueue.Peek().time > actionQueueToPush.Peek().time)
                {
                    newQueue.Enqueue(actionQueueToPush.Dequeue());
                }else if(originalActionQueue.Peek().time < actionQueueToPush.Peek().time)
                {
                    newQueue.Enqueue(originalActionQueue.Dequeue());
                }else if (originalActionQueue.Peek().time == actionQueueToPush.Peek().time)
                {
                    newQueue.Enqueue(originalActionQueue.Dequeue());
                    newQueue.Enqueue(actionQueueToPush.Dequeue());
                }

                if (!(originalActionQueue.Count > 0))
                {
                    while(actionQueueToPush.Count > 0) { 
                        newQueue.Enqueue(actionQueueToPush.Dequeue());
                    }
                    return newQueue;
                }else if(!(actionQueueToPush.Count > 0))
                {
                    while(originalActionQueue.Count > 0) { 
                        newQueue.Enqueue(originalActionQueue.Dequeue());
                    }
                    return newQueue;
                }
            }

            if(!(originalActionQueue.Count > 0))
            {
                while (actionQueueToPush.Count > 0)
                {
                    newQueue.Enqueue(actionQueueToPush.Dequeue());
                }
            }
            

            return newQueue;
        }
    }
}
