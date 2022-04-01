/* BEGIN GUIBroker.js */

/*
 * gui_broker object 
 * This object is used to store data from the simulator. 
 * It basically acts as the broker between the GUI and simulation.
 */
let gui_broker = {
    play_status: false,
    droplets: [],
    electrodes: [],
    droplet_groups: {},
    next_simulator_step: () => {
        DotNet.invokeMethodAsync('MicrofluidSimulator', 'JSSimulatorNextStep');
    },
    next_simulator_step_time: (time) => {
        DotNet.invokeMethodAsync('MicrofluidSimulator', 'nextStepTime', time);
    },
    init_board: () => { },
    get_droplet_groups: function () {
        this.droplet_groups = {};

        for (let i = 0; i < this.droplets.length; i++) {
            if (typeof this.droplet_groups[this.droplets[i].group] == "undefined") {
                this.droplet_groups[this.droplets[i].group] = [(this.droplets[i])];
            } else {
                this.droplet_groups[this.droplets[i].group].push(this.droplets[i]);
            }
        }
    }
};

/* END GUIBroker.js */