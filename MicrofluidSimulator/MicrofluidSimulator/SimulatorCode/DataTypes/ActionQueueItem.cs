namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ActionQueueItem
    {
        //DataTypes.SimulatorAction action;
        //float time;

        public ActionQueueItem(DataTypes.SimulatorAction action, float time)
        {
            this.action = action;
            this.time = time;
        }

        public DataTypes.SimulatorAction action { get; set; }
        public float time { get; set; }
    }
}
