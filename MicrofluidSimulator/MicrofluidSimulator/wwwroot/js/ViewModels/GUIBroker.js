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
    simulator_time: 0,
    simulator_prev_time: 0,
    simulator_time_step: -1,
    data_to_download: [],
    board: {},
    droplets: [],
    electrodes: [],
    droplet_groups: {},
    prev_droplet_groups: {},

    /**
     * The function next_simulator_step calls on the simulator to run the next step
     * and send the corrosponding data.
     */
    next_simulator_step: () => {
        DotNet.invokeMethod('MicrofluidSimulator', 'JSSimulatorNextStep');
    },

    /**
     * The function next_simulator_step_time signals the simulator to take run
     * until a given time.
     */
    next_simulator_step_time: (time) => {
        DotNet.invokeMethod('MicrofluidSimulator', 'nextStepTime', time);
    },

    /**
     * The function update_simulator_container is used to send edited information
     * to the simulator.
     * @param {any} type
     * @param {any} JSONString
     */
    update_simulator_container: function (type, JSONString) {
        DotNet.invokeMethod('MicrofluidSimulator', 'updateSimulatorContainer', type, JSONString);
    },

    /**
     * The function goto_simulator_step is used to signal the simulator to go
     * to a given time.
     */
    goto_simulator_time_step: (time) => {
        DotNet.invokeMethod('MicrofluidSimulator', 'gotoSimulatorTimeStep', time);
    },

    /**
     * The function init_board is used to initialize the GUI board.
     */
    init_board: () => { console.log("BEFORE LOADED"); }, // Onload this will be defined in sketch.js

    /**
     * The function get_droplet_gorups will calculate the droplet groupings,
     * from the droplet data.
     */
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