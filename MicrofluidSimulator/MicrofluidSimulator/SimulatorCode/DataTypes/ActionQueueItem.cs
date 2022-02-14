namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ActionQueueItem
    {
        Action action;
        float time;

        public ActionQueueItem(Action action, float time)
        {
            this.action = action;
            this.time = time;
        }

        public Action Action { get => action; set => action = value; }
        public float Time { get => time; set => time = value; }
    }
}
