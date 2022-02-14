namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ActionQueueItem
    {
        DataTypes.Action action;
        float time;

        public ActionQueueItem(DataTypes.Action action, float time)
        {
            this.action = action;
            this.time = time;
        }

        public DataTypes.Action Action { get => action; set => action = value; }
        public float Time { get => time; set => time = value; }
    }
}
