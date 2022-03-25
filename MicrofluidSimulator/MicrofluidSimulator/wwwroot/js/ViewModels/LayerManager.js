let layer_manager = {


    layers: {
        draw_droplets: {
            name: "droplet_draw_call",
            value: "droplet_draw_call",
            id: "draw_droplet",
            text: "Draw Droplets", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: false
            //layer: "insert"         // Reference - pass to functions
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
        draw_selected_element: {
            name: "draw_selected_element",
            value: "draw_selected_element",
            id: "drdraw_selected_element",
            text: "Draw Selected", // Will be shown in layer panel list
            element: "insert",      // Reference - get toggled from here
            checkbox: "insert",
            checked: true,
            layer: "insert"         // Reference - pass to functions
        }

    },

    initialize_layers: function () {

        for (let layer in this.layers) {
            var div = document.createElement('div');
            div.innerHTML = `<input type='checkbox' name='${this.layers[layer].name}' value='${this.layers[layer].value}' id='${this.layers[layer].id}'/>
                             <label for='${this.layers[layer].name}'>${this.layers[layer].text}</label>`;
            this.layers[layer].element = div;
            this.layers[layer].checkbox = div.querySelector('input');
            this.layers[layer].checkbox.checked = this.layers[layer].checked;

            gui_controller.getLayerPanel().querySelector('form').append(div);
        }
    },

    draw_layers: function (sketch) {
        for (let layer in this.layers) {
            if (this.layers[layer].hasOwnProperty("layer") && this.layers[layer].checkbox.checked) {
                // Draw
                sketch.image(this.layers[layer].layer, 0, 0);
            }
        }
    }
}