/*
 * This is a dumb version of the virtual machine, it is used for testing purposes,
 * since we do not have access to the actual virtual machine.
 */

using MicrofluidSimulator.SimulatorCode.Simulator;

namespace MicrofluidSimulator.DumbVM
{
    public class DumbVM
    {
        private Simulator simulator;

        public DumbVM(Simulator simulator)
        {
            this.simulator = simulator
        }

        // Main loop
        public static void Main(string[] args)
        {

        }

        private void turnOnHeaterAtTime(float time, int heaterID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if(currentSimulatorTime == time)
            {
                simulator.setActuatorTargetTemperature(heaterID, 100);
                
            }
        }

    }
}
