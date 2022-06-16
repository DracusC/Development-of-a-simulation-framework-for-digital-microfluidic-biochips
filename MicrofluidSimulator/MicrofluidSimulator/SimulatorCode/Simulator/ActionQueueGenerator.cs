
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{
    public class ActionQueueGenerator
    {

        /// <summary>
        /// Function that loops the "finalizedQueue" as many times as necessary
        /// </summary>
        /// <returns></returns>
        public static Queue<ActionQueueItem> generateActionQueue()
        {


            Queue<ActionQueueItem> finalizedQueue = generateColorSortingActionQueueHelper(0);

            for (int i = 1; i < 50; i++)
            {

                finalizedQueue = ActionQueueModels.pushActionQueueToOriginalActionQueue(finalizedQueue, generateColorSortingActionQueueHelper(i * 18 * 0.16m));

            }





            return finalizedQueue;
        }

        /// <summary>
        /// Helper function used to easily generate the color sorting action queue
        /// </summary>
        /// <param name="timeIncrement"></param>
        /// <returns></returns>
        public static Queue<ActionQueueItem> generateColorSortingActionQueueHelper(decimal timeIncrement)
        {

            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            int[] electrodeOrder = new int[] {318, 317, 318, 316, 317, 315, 316, 314, 315, 313, 314, 312, 313, 311, 312, 310, 311, 309, 310,
            308, 309, 307, 308, 306, 307, 305, 306, 304, 305, 303, 304, 302, 303, 301, 302, 300, 301, 299, 300, 298, 299, 298};

            for (int i = 0; i < 42; i++)
            {
                if (i < 2)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m + timeIncrement));
                }
                else if (i == 41)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m + timeIncrement));
                }
                else if (i % 2 == 0)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m + timeIncrement));
                }
                else
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m + timeIncrement));
                }

            }

            return actionQueueInstructions;
        }

        /// <summary>
        /// Helper function to generate an action
        /// </summary>
        /// <param name="actionOnID"></param>
        /// <param name="actionChange"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static ActionQueueItem generateAction(int actionOnID, int actionChange, decimal time)
        {
            SimulatorAction action = new SimulatorAction("electrode", actionOnID, actionChange);
            ActionQueueItem item = new ActionQueueItem(action, time);

            return item;

        }

        /// <summary>
        /// Function to generate an action queue from txt file
        /// </summary>
        /// <param name="generatedActionQueue"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Queue<ActionQueueItem> generateActionQueueFromReader(string generatedActionQueue, Container container, string browserLanguage)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();

            string firstLine = generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
            string[] firstWords = firstLine.Split(' ');
            double timeToSubtract;
            if (!browserLanguage.Equals("en"))
            {
                timeToSubtract = Convert.ToDouble(firstWords[4].Replace(".", ","));
            }
            else
            {
                timeToSubtract = Convert.ToDouble(firstWords[4]);
            }
 

            foreach (string line in generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {

                string[] words = line.Split(' ');
                

                for (int i = 9; i < words.Length; i++)
                {

                    if (words[7].Equals("setel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[8]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 1);
                        ActionQueueItem item;
                        if (!browserLanguage.Equals("en"))
                        {
                            item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4].Replace(".", ",")) - timeToSubtract) * 0.001));
                        }
                        else
                        {
                            item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4]) - timeToSubtract) * 0.001));
                        }
                        

                        actionQueueInstructions.Enqueue(item);
                    }
                    else if (words[7].Equals("clrel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[8]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 0);
                        ActionQueueItem item;
                        if (!browserLanguage.Equals("en"))
                        {
                            item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4].Replace(".", ",")) - timeToSubtract) * 0.001));
                        }
                        else
                        {
                            item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4]) - timeToSubtract) * 0.001));
                        }


                        actionQueueInstructions.Enqueue(item);
                    }




                }


            }

            
            return actionQueueInstructions;

        }
    }
}
