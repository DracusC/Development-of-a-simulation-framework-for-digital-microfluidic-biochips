using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        public static void dropletSubscriptions(Electrodes[] electrodeBoard, Droplets[] droplets, Droplets caller)
        {

            int posX = caller.PositionX / 20;
            int posY = caller.PositionY / 20;
            for(int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if( x>=0 && x<32 && y>= 0 && y < 32){
                        electrodeBoard[(posX+x)+(posY+y)*32].Subscriptions.Add(caller.ID1);
                    }
                }
            }
        }
    }
}
