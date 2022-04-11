﻿// Global function to setup the p5js instance
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

window.start_update_timer = () => {
    console.time("UpdateTimer");
}
window.end_update_timer = () => {
    console.timeEnd("UpdateTimer");
}

// TODO: Look into reflections
// Global methods that can be called by C# scripts
window.update_board = (_container_string) => {
    console.time("DeserializeTimer");
    let container_string = BINDING.conv_string(_container_string);

    var board = JSON.parse(container_string);
    gui_broker.board = board;
    gui_broker.droplets = board.droplets;
    gui_broker.electrodes = board.electrodes;

    //information_panel_manager.draw_information(board.Electrodes[130]);
    gui_broker.prev_droplet_groups = gui_broker.droplet_groups;
    gui_broker.get_droplet_groups();

    information_panel_manager.dynamic_update();

    console.timeEnd("DeserializeTimer");
    console.timeEnd("UpdateTimer");

    document.querySelector("#simulatorTime").innerHTML = gui_broker.board.currentTime;
}

window.change_play_status = (status) => {
    gui_broker.play_status = !gui_broker.play_status;
};

window.initialize_board = (information) => {
    // Remove any previous sketch
    if (gui_broker.sketch_ref != null) {
        gui_broker.sketch_ref.remove();
    }

    var JSONinformation = JSON.parse(information);
    console.log(JSONinformation);
    gui_broker.init_board(JSONinformation.sizeX, JSONinformation.sizeY + 1);

    gui_controller.showGUI();
    gui_controller.changeBoardName(JSONinformation.platform_name);

    //document.querySelector("#defaultCanvas0").style.width = "1000px";

    layer_manager.initialize_layers();
}

window.get_selected_element = () => {
    return JSON.stringify(information_panel_manager.selected_element);
}

window.gui_broker = gui_broker;