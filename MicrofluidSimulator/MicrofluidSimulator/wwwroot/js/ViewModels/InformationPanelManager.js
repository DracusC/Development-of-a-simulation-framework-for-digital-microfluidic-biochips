/* BEGIN InformationPanelManager.js */


/*
 * The information_panel_manager object is used to store both information
 * and logic, for which information about the sketch elements can be displayed.
 */
let information_panel_manager = {
    saveclose_button_div: null,
    edit_button: null,
    save_button: null,
    close_button: null,
    multiple_selection: null,
    double_clicked: false,
    selected_element: null,
    selected_element_type: null,
    information_element: null,
    editing: false,
    before_edit_values: {},

    /*
    * The object display_info stores the information that will be displayed
    * in the information panel, and which values can be edited.
    */
    display_info: {
        Droplet: {
            ID: "",
            substance_name: "",
            color: "",
            sizeX: 0,
            positionX: 0,
            positionY: 0,
            temperature: 0,
            volume: 0,
            group: 0
        },
        Droplet_editable: ["volume", "temperature", "color"],
        Electrode: {
            name: "",
            ID: "",
            status: 0,
            positionX: 0,
            positionY: 0,
            subscriptions: []
        },
        Electrode_editable: ["status"],
        Group: {
            groupID: 0,
            substance_name: "",
            color: "",
            temperature: 0,
            volume: 0,
            Type: ""
            //droplets: []
        },
        Group_editable: ["volume", "temperature", "color"],
        Actuator: {
            name: "",
            ID: 0,
            actuatorID: 0,
            type: "",
            valueActualTemperature: 0,
            valueDesiredTemperature: 0,
            valuePowerStatus: 0
        },
        Actuator_editable: [],
        Sensor: {
            name: "",
            ID: 0,
            sensorID: 0,
            type: "",
            valueRed: 0,
            valueBlue: 0,
            valueGreen: 0,
            valueTemperature: 0
        },
        Sensor_editable: []
    },

    /**
     * The function information_filer will take the type of element, the element
     * and filter it, such that only the information we choose is displayed.
     *
     * There are two other arguments, namely the element, and groupID for group type. 
     * @param {any} type
     */
    information_filter: function (type) {
        let returnVal;
        this.selected_element_type = type;

        switch (type) {
            case ("Droplet"):
                let droplet = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = droplet[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
            case ("Electrode"):
                let electrode = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = electrode[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
            case ("Group"):

                let id = arguments[2];
                let group = arguments[1];

                returnVal = { ...this.display_info[type] };
                returnVal.droplets = [];
                returnVal.groupID = id;

                returnVal.substance_name = group[0].substance_name;
                returnVal.color = group[0].color;

                for (let i = 0; i < group.length; i++) {
                    let droplet = group[i];
                    returnVal.droplets.push(droplet.ID);
                    for (let key in droplet) {
                        if (key == "volume") { returnVal.volume += droplet[key] }
                        if (key == "temperature") { returnVal.temperature += droplet[key] }
                    }
                }

                returnVal.temperature = returnVal.temperature / group.length;

                returnVal.Type = type;
                return returnVal;
                break;

            case ("Actuator"):
                let actuator = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = actuator[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;

            case ("Sensor"):
                let sensor = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = sensor[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
        }
    },

    /**
     * This function controls the condition, where the user can select multiple
     * elements with a double click, this can happen if more than one element
     * is stacked in the sketch.
     * @param {any} layer
     */
    draw_multiple_selection: function (layer) {

        let s_list = [];
        for (let i = 0; i < Object.keys(information_panel_manager.multiple_selection).length; i++) {
            s_list.push((i + 1) + " " + Object.keys(information_panel_manager.multiple_selection)[i]);
        }

        layer.textSize(14);
        layer.fill("#1b6ec2");
        let max_width = Math.max.apply(Math, s_list.map(function (o) { return layer.textWidth(o); }))
        let max_height = layer.textAscent(s_list[0]) * s_list.length + 10 + 5 * s_list.length;

        layer.stroke("#000000");
        layer.rect(layer.mouseX, layer.mouseY, max_width + 20, max_height, 5);

        layer.noStroke();
        layer.fill("#ffffff");
        for (let i in s_list) {
            layer.text(s_list[i], layer.mouseX + 10, layer.mouseY + (layer.textAscent(s_list[i]) * (parseInt(i) + 1)) + 5 * (parseInt(i) + 1));
        }
        layer.stroke("#000000");
    },
    /**
     * Draws the selected element
     * @param {any} layer
     * @param {any} element
     */
    draw_selected_element: function(layer, element) {
        layer.clear();
        if (element == null) { return; }

        if (typeof element.status != "undefined") {

            // Electrode
            for (let i = 0; i < gui_broker.electrodes.length; i++) {
                let electrode = gui_broker.electrodes[i];

                if (electrode.ID != element.ID) { continue; }

                layer.noFill();
                layer.stroke("blue");
                layer.strokeWeight(3);

                // Check the electrode shape
                if (electrode.shape == 1) {
                    layer.beginShape();
                    for (let i = 0; i < electrode.corners.length; i++) {
                        layer.vertex(electrode.positionX + electrode.corners[i][0] + 0.5, electrode.positionY + electrode.corners[i][1] + 0.5);
                    }
                    layer.endShape(layer.CLOSE);
                } else {
                    layer.rect(electrode.positionX, electrode.positionY, electrode.sizeX, electrode.sizeY);
                }
            }
        } else {

        }
    },

    /**
     * This function will take an element and display the correct information
     * in the information panel.
     * @param {any} element
     */
    draw_information: function (element) {

        this.onCancel();

        let informationPanel = document.querySelector("#simulatorGUI").querySelector("#information");
        let informationView = informationPanel.querySelector("#informationElements");
        informationView.innerHTML = "";

        let div = document.createElement("div");

        for (let key in element) {
            if (!(key in this.display_info[element.Type])) { continue; }

            let innerDiv = document.createElement("div");
            let innerInput = document.createElement("input");

            innerDiv.innerHTML = key + ": ";
            innerDiv.classList.add("information_item");
            innerInput.value = element[key];
            innerInput.readOnly = true;
            innerInput.classList.add("input_readonly");

            innerDiv.append(innerInput);
            div.append(innerDiv);
        }

        informationView.append(div);
    },

    /**
     * This function controls the logic of editing values of an element.
     */
    onEdit: function () {

        if (this.selected_element == null) { return; }

        this.before_edit_values = {};

        this.edit_button.style.visibility = "hidden";
        this.saveclose_button_div.style.visibility = "visible";
        this.editing = true;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        var div_arr = Array.prototype.slice.call(divs);
        var input_arr = Array.prototype.slice.call(inputs);

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                this.before_edit_values[attribute] = input.value;
                input.readOnly = false;
                input.classList.remove("input_readonly");
            }
        }
    },

    /**
     * This function controls the logic of canceling an edit on an element.
     */
    onCancel: function () {
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        this.editing = false;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        var div_arr = Array.prototype.slice.call(divs);
        var input_arr = Array.prototype.slice.call(inputs);

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                input.value = this.before_edit_values[attribute];
                input.readOnly = true;
                input.classList.add("input_readonly");
            }
        }
    },

    /**
     * The function onSave controls the logic for saving the edit of element values,
     * and will call upon the gui_broker to send the information to the simulator.
     */
    onSave: function () {
        
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        this.editing = false;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        let div_arr = Array.prototype.slice.call(divs);
        let input_arr = Array.prototype.slice.call(inputs);

        let values_to_send = {};

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                input.readOnly = true;
                input.classList.add("input_readonly");

                // TODO: Check if the input value is ok!
                values_to_send[attribute] = input.value;
            }
        }

        if (this.selected_element_type == "Group") {
            values_to_send.droplets = this.selected_element.droplets;
            values_to_send.groupID = this.selected_element.groupID;
        } else {
            values_to_send.ID = this.selected_element.ID;
        }

        // Send the new element values to the simulator
        gui_broker.update_simulator_container(this.selected_element_type, JSON.stringify(values_to_send));
    },

    /**
     * The clear function will clear the information panel from any selected element.
     */
    clear: function () {
        this.editing = false;
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        this.selected_element = null;
        this.information_element = null;
        this.selected_element_type = null;

        let div = document.querySelector("#informationElements");
        div.innerHTML = "";
    },

    /**
     * The dynamic_update function allows of the selection of elements,
     * even when the simulation is running and the elements of the GUI are updating.
     * This allows realtime viewing of the attributes changing.
     */
    dynamic_update: function () {

        if (this.selected_element == null) { return; }

        let cur_element = this.selected_element;
        let new_element = null;
        let type = this.selected_element_type.toLowerCase();
        let groupID = (typeof cur_element.groupID === 'undefined') ? null : cur_element.groupID;

        if (type == "group") {
            let group_list = gui_broker.droplet_groups;
            new_element = group_list[groupID];

        } else {

            let element_list = gui_broker.board[type + "s"];
            element_list.forEach((element) => {
                if (element.ID == cur_element.ID) {
                    new_element = element;
                }
            })
        }

        // If the element is deleted clear the panel
        if (new_element == null) { this.clear(); return; }

        // Update information
        this.selected_element = this.information_filter(this.selected_element_type, new_element, groupID);
        this.information_element = this.information_filter(this.selected_element_type, new_element, groupID);
        this.draw_information(this.information_filter(this.selected_element_type, new_element, groupID));
    }

}

/* END InformationPanelManager.js */
