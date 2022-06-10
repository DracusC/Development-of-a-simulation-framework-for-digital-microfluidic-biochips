
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;

namespace MicrofluidSimulator.SimulatorCode.Simulator
{
    public class ActionQueueGenerator
    {

        public static Queue<ActionQueueItem> generateTestQueue()
        {


            Queue<ActionQueueItem> finalizedQueue = generateTestQueueHelper(0);

            for (int i = 1; i < 50; i++)
            {

                finalizedQueue = ActionQueueModels.pushActionQueueToOriginalActionQueue(finalizedQueue, generateTestQueueHelper(i * 18 * 0.16m));

            }





            return finalizedQueue;
        }

        public static Queue<ActionQueueItem> generateTestQueueHelper(decimal timeIncrement)
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

        private static ActionQueueItem generateAction(int actionOnID, int actionChange, decimal time)
        {
            SimulatorAction action = new SimulatorAction("electrode", actionOnID, actionChange);
            ActionQueueItem item = new ActionQueueItem(action, time);

            return item;

        }

        public static Queue<ActionQueueItem> generateTestQueueFromReader(string generatedActionQueue, Container container)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();

            SimulatorAction dummyAction = new SimulatorAction("electrode", 1, 0);
            ActionQueueItem dummyItem = new ActionQueueItem(dummyAction, 1);
            //actionQueueInstructions.Enqueue(dummyItem);

            int counter = 0;
            int timeStep = 1;
            foreach (string line in generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {

                if (counter % 2 == 0 && counter != 0)
                {
                    timeStep++;
                }
                string[] words = line.Split(' ');
                for (int i = 3; i < words.Length - 1; i++)
                {

                    if (words[1].Equals("setel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 1);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }
                    else if (words[1].Equals("clrel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[2]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 0);
                        ActionQueueItem item = new ActionQueueItem(action, timeStep);
                        actionQueueInstructions.Enqueue(item);
                    }




                }

                counter++;
            }
            SimulatorAction action12 = new SimulatorAction("electrode", 419, 1);
            ActionQueueItem item12 = new ActionQueueItem(action12, timeStep + 1);
            SimulatorAction action1 = new SimulatorAction("electrode", 451, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, timeStep + 2);
            SimulatorAction action13 = new SimulatorAction("electrode", 419, 0);
            ActionQueueItem item13 = new ActionQueueItem(action13, timeStep + 3);

            SimulatorAction action3 = new SimulatorAction("electrode", 451, 0);
            ActionQueueItem item3 = new ActionQueueItem(action3, timeStep + 5);


            actionQueueInstructions.Enqueue(item12);
            actionQueueInstructions.Enqueue(item1);
            actionQueueInstructions.Enqueue(item13);

            actionQueueInstructions.Enqueue(item3);







            return actionQueueInstructions;

        }

        public static Queue<ActionQueueItem> generateSimplePathsQueueFromReader(string generatedActionQueue, Container container)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();

            string firstLine = generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
            string[] firstWords = firstLine.Split(' ');
            double timeToSubtract = Convert.ToDouble(firstWords[4].Replace(".", ",")); // float.Parse(firstWords[4], CultureInfo.InvariantCulture);
            Console.WriteLine("TIMETOSUBTRACT " + (timeToSubtract));


            //int initialMs = 0;
            //string empty = "";
            foreach (string line in generatedActionQueue.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {


                string[] words = line.Split(' ');
                //for (int i = 0; i < words.Length; i++)
                //{
                //    if (i == 4)
                //    {
                //        empty += initialMs + " ";

                //        initialMs += 160;
                //    }
                //    else if (i != words.Length - 1)
                //    {
                //        empty += words[i] + " ";

                //    }
                //    else
                //    {
                //        empty += words[i];

                //    }
                //    ;
                //}
                //empty += "\n";




                for (int i = 9; i < words.Length; i++)
                {

                    if (words[7].Equals("setel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[8]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 1);
                        ActionQueueItem item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4].Replace(".", ",")) - timeToSubtract) * 0.001));

                        actionQueueInstructions.Enqueue(item);
                    }
                    else if (words[7].Equals("clrel"))
                    {

                        int electrodeId = Models.HelpfullRetreiveFunctions.getIdOfElectrodByElectrodID(Int32.Parse(words[i]), Int32.Parse(words[8]), container);
                        SimulatorAction action = new SimulatorAction("electrode", electrodeId, 0);
                        ActionQueueItem item = new ActionQueueItem(action, (decimal)((Convert.ToDouble(words[4].Replace(".", ",")) - timeToSubtract) * 0.001));

                        actionQueueInstructions.Enqueue(item);
                    }




                }


            }

            //Console.WriteLine(empty);
            return actionQueueInstructions;

        }
    }
}
