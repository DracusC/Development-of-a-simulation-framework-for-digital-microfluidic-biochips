namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ActionQueueItem
    {
        DataTypes.SimulatorAction action;
        float time;

        public ActionQueueItem(DataTypes.SimulatorAction action, float time)
        {
            this.action = action;
            this.time = time;
        }

        public DataTypes.SimulatorAction Action { get => action; set => action = value; }
        public float Time { get => time; set => time = value; }
    }
}
