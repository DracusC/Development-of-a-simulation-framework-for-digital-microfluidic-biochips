namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ActionQueueItem
    {
        
        // an actionqueue item contains the action itself as well as the time of which it should be executed
        public ActionQueueItem(SimulatorAction action, decimal time)
        {
            this.action = action;
            this.time = time;
        }

        public DataTypes.SimulatorAction action { get; set; }
        public decimal time { get; set; }
    }
}
