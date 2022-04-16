/* BEGIN Global.js */

/**
 * This file contains functions declared in the global state,
 * by that it is inferred that they are bound to the window object.
 */

/**
 * The function setp5 initializes the p5js instance (the sketch),
 * and binds it to the element with id 'container'.
 */
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

/**
 * The functions start_update_timer and end_update_timer are used to
 * start and end timer used for time analysis.
 */
window.start_update_timer = () => {
    console.time("UpdateTimer");
}
window.end_update_timer = () => {
    console.timeEnd("UpdateTimer");
}

/**
 * The function update_board is one of the main driving functions of the
 * GUI-backend, it is used to update the information in the GUI directly from
 * the simulator.
 * @param {any} _container_string
 */
window.update_board = (_container_string) => {

    /* Convert JSON string to an object */
    let container_string = BINDING.conv_string(_container_string);
    var board = JSON.parse(container_string);

    /* Used to calculate time difference, which allows for realtime execution. */
    if (typeof gui_broker.board.currentTime != "undefined") {
        gui_broker.simulator_prev_time = gui_broker.board.currentTime;
    }

    /* Update the gui_broker information */
    gui_broker.board = board;
    gui_broker.droplets = board.droplets;
    gui_broker.electrodes = board.electrodes;
    gui_broker.prev_droplet_groups = gui_broker.droplet_groups;
    gui_broker.get_droplet_groups();
    
    /* Allow dynamic selection */
    information_panel_manager.dynamic_update();

    /* End timer */
    console.timeEnd("UpdateTimer");

    /* Update the displayed time in the GUI */
    document.querySelector("#simulatorTime").innerHTML = gui_broker.board.currentTime;

    /* Trigger animation */
    animate_once();
}

/**
 * The function change_play_status is used pause or play the simulator, from
 * the GUI.
 * @param {any} status
 */
window.change_play_status = (status) => {
    gui_broker.play_status = !gui_broker.play_status;
};

/**
 * The function initialize_board is used to initialize the information needed
 * to create the GUI.
 * @param {any} information
 */
window.initialize_board = (information) => {

    /*
     * TODO: Check if we can fix initial realtime execution
     * by giving more information here!
     */

    /* Remove any previous sketch */
    if (gui_broker.sketch_ref != null) {
        gui_broker.sketch_ref.remove();
    }

    /* Convert JSON string to object */
    var JSONinformation = JSON.parse(information);
    gui_broker.init_board(JSONinformation.sizeX, JSONinformation.sizeY + 1);
    
    /* Display GUI elements */
    gui_controller.showGUI();
    gui_controller.changeBoardName(JSONinformation.platform_name);
    document.querySelector("#edit_button").style.visibility = "visible";

    //document.querySelector("#defaultCanvas0").style.width = "1000px";

    layer_manager.initialize_layers();
}

window.get_selected_element = () => {
    return JSON.stringify(information_panel_manager.selected_element);
}

/**
 * The function animate_once, will trigger one animation cycle in the GUI.
 */
window.animate_once = () => {
    gui_broker.animate = true;
    lerp_amount = 0;
}

/**
 * The function restart_board is called when we trigger a restart.
 */
window.restart_board = () => {
    gui_broker.animate = false;
    gui_broker.simulator_prev_time = 0;
    lerp_amount = 1;
}

/**
 * The function start_simulator_time is used to start the time,
 * we use for realtime execution.
 */
window.start_simulator_time = () => {
    gui_broker.simulator_time = Date.now();
}

/**
 * The function download_data is used to download simulator data,
 * into JSON format.
 */
window.download_data = () => {
    let jsonData = JSON.stringify(gui_broker.data_to_download);

    /* Download the data by forcing a click on an anchor element. */
    var dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(jsonData);
    var dlAnchorElem = document.getElementById('downloadAnchorElem');
    dlAnchorElem.setAttribute("href", dataStr);
    dlAnchorElem.setAttribute("download", "data.json");
    dlAnchorElem.click();

    gui_broker.data_to_download = [];
}

/**
 * The function send_download_data is used by the simulator to send the data
 * to the GUI, which will then be formatted, so that it's ready for download.
 * @param {any} jsonData
 */
window.send_download_data = (jsonData) => {
    let obj = JSON.parse(jsonData);
    gui_broker.data_to_download.push(obj);
}

/* Make the gui_broker a global object */
window.gui_broker = gui_broker;

/* END Global.js */