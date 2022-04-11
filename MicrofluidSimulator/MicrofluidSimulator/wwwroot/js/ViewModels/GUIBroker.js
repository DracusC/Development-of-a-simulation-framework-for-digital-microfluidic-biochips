/* BEGIN GUIBroker.js */

/*
 * gui_broker object 
 * This object is used to store data from the simulator. 
 * It basically acts as the broker between the GUI and simulation.
 */
let gui_broker = {
    sketch_ref: null,
    play_status: false,
    animate: false,
    board: {},
    droplets: [],
    electrodes: [],
    droplet_groups: {},
    prev_droplet_groups: {},
    next_simulator_step: () => {
        DotNet.invokeMethod('MicrofluidSimulator', 'JSSimulatorNextStep');
    },
    next_simulator_step_time: (time) => {
        DotNet.invokeMethod('MicrofluidSimulator', 'nextStepTime', time);
    },
    update_simulator_container: function (type, JSONString) {
        DotNet.invokeMethod('MicrofluidSimulator', 'updateSimulatorContainer', type, JSONString);
    },
    goto_simulator_time_step: (time) => {
        DotNet.invokeMethod('MicrofluidSimulator', 'gotoSimulatorTimeStep', time);
    },
    init_board: () => { console.log("BEFORE LOADED"); },
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