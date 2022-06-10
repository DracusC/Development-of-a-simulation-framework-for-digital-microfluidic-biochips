/* BEGIN LayerManager.js */

/*
 * The layer_manager object stores information about the different layers,
 * that are present in the sketch, these layers can be toggled off or on in
 * the selection panel.
 */
let selection_manager = {

    /*
     * The layers object stores information about each layer in the GUI.
     */
    selection_list: {
        real_time: {
            name: "real_time",
            value: "real_time",
            id: "real_time",
            text: "Real-Time Execution",    // Will be shown in layer panel list
            element: "insert",              // Reference - get toggled from here
            checkbox: "insert",
            checked: true
            //layer: "insert"               // Reference - pass to functions
        },
        draw_droplet_groups: {
            name: "droplet_group_draw_call",
            value: "droplet_group_draw_call",
            id: "draw_droplet_group",
            text: "Draw Droplet Groups", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true
            //layer: "insert"         // Reference - pass to functions
        },
        draw_droplet_animations: {
            name: "draw_droplet_animations",
            value: "draw_droplet_animations",
            id: "draw_droplet_animations",
            text: "Animations", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true
            //layer: "insert"         // Reference - pass to functions
        },
        draw_active_electrodes: {
            name: "draw_active_electrodes",
            value: "draw_active_electrodes",
            id: "draw_active_electrodes",
            text: "Draw Active Electrodes", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true
            //layer: "insert"         // Reference - pass to functions
        },
        draw_actuators: {
            name: "draw_actuators",
            value: "draw_actuators",
            id: "draw_actuators",
            text: "Draw Actuators", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true,
            layer: "insert"         // Reference - pass to functions
        },
        draw_sensors: {
            name: "draw_sensors",
            value: "draw_sensors",
            id: "draw_sensors",
            text: "Draw Sensors", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true,
            layer: "insert"         // Reference - pass to functions
        },
        draw_bubbles: {
            name: "draw_bubbles",
            value: "draw_bubbles",
            id: "draw_bubbles",
            text: "Draw Bubbles", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true
            //layer: "insert"         // Reference - pass to functions
        },
        draw_selected_element: {
            name: "draw_selected_element",
            value: "draw_selected_element",
            id: "draw_selected_element",
            text: "Draw Selected", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true,
            layer: "insert"         // Reference - pass to functions
        },
        debug_electrode_text: {
            name: "debug_electrode_text",
            value: "debug_electrode_text",
            id: "db_e_text",
            text: "Electrode IDs", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: false,
            layer: "insert"         // Reference - pass to functions
        },
        draw_droplets: {
            name: "droplet_draw_call",
            value: "droplet_draw_call",
            id: "draw_droplet",
            text: "Draw Droplets", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: false
            //layer: "insert"         // Reference - pass to functions
        }

    },

    /**
     * The function initialize_layers will create toggles and layers
     * for each of the layers contained in the layers property.
     */
    initialize_selection_list: function () {

        for (let layer in this.selection_list) {
            var div = document.createElement('div');
            div.classList.add("form-check");
            div.innerHTML = `<input type='checkbox' name='${this.selection_list[layer].name}' value='${this.selection_list[layer].value}' id='${this.selection_list[layer].id}' class='form-check-input'/>
                             <label for='${this.selection_list[layer].name}' class='form-check-label'>${this.selection_list[layer].text}</label>`;
            this.selection_list[layer].element = div;
            this.selection_list[layer].checkbox = div.querySelector('input');
            this.selection_list[layer].checkbox.checked = this.selection_list[layer].checked;

            document.querySelector("#simulatorGUI").querySelector("#selectionPanel").querySelector('form').append(div);
        }
    },

    /**
     * The function draw_layers will draw the layers in the sketch.
     * @param {any} sketch
     */
    draw_selection_list: function (sketch) {
        for (let layer in this.selection_list) {
            if (this.selection_list[layer].hasOwnProperty("layer") && this.selection_list[layer].checkbox.checked) {
                // Draw
                sketch.image(this.selection_list[layer].layer, 0, 0);
            }
        }
    }
}

/* END LayerManager.js */