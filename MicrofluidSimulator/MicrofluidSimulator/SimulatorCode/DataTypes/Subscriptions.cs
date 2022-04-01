namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Subscriptions
    {
        //int subscriptionType; 

        public Subscriptions(int subscription)
        {
            this.subscriptionType = subscriptionType;
        }

        public Subscriptions()
        {
            this.subscriptionType = -1; //null
        }

        public int subscriptionType { get; set; }
    }
}
