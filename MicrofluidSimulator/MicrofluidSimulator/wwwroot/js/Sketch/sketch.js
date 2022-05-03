/* 
 * Written by Joel A. V. Madsen
 * This file contains the main graphical user interface components,
 * it uses p5js as the framework, for which we can initialize a canvas
 * and draw onto it.
 */


/* 
 * Declaration of script "global" variables.
 * These variables are used by both the global functions but also by the p5js script.
 */
let simulator_droplets = []; // Not used
let simulator_electrodes = []; // Not used
let lerp_amount = 0;     // Used to interpolate between droplet positions.


// Layer for electrodes, since their placement are static
let layer_electrode_shape;


/*
 * P5JS Sketch
 * The sketch function uses the p5js framework to create GUI elements within a Canvas
 */
let sketch = function (p) {

    let step; // Used to increment the interpolation between droplet positions.

    /*
     * Setup is called once on start of the sketch.
     */
    p.setup = function () {
        // Create main canvas
        let canvas = p.createCanvas(1, 1);
        canvas.mouseClicked(onMouseClicked);
        canvas.doubleClicked(onMouseDoubleClicked);
        
        console.log(canvas.position());

        // Used to get sharper edges in sketch
        p.pixelDensity(4);

        // Create layers
        //layer_electrode_id = p.createGraphics(1, 1);
        layer_electrode_shape = p.createGraphics(1, 1);
        console.log("setup");

        //p.frameRate(10);
        step = 0.05;

        let saveclose_button_div = p.select("#saveclose_button_div");
        information_panel_manager.saveclose_button_div = saveclose_button_div.elt;

        let edit_button = p.select("#edit_button");
        information_panel_manager.edit_button = edit_button.elt;
        edit_button.mousePressed(() => {
            information_panel_manager.onEdit();
        });
        //edit_button.style("visibility", "visible");

        let cancel_button = p.select("#cancel_button");
        cancel_button.mousePressed(() => {
            information_panel_manager.onCancel();
        });

        let save_button = p.select("#save_button");
        save_button.mousePressed(() => {
            information_panel_manager.onSave();
        });
    }


    /*
     * Draw is called every frame of the sketch.
     */
    
    p.draw = function () {
        //console.time("DrawTime");
        if ((gui_broker.play_status && layer_manager.layers.draw_droplet_animations.checkbox.checked) || gui_broker.animate) {
            lerp_amount += 0.07; // Maybe make an animation object, or global variables
        }

        /* Redraw the canvas */
        p.background(240);

        /* Draw calls */
        p.image(layer_electrode_shape, 0, 0);

        if (layer_manager.layers.draw_active_electrodes.checkbox.checked) { draw_active_electrodes(); }

        // Draw all direct layers
        layer_manager.draw_layers(p);

        //if (layer_manager.layers.draw_actuators.checkbox.checked) { draw_actuators(); }

        

        if (layer_manager.layers.draw_selected_element.checkbox.checked) { draw_selected_element(layer_manager.layers.draw_selected_element.layer, information_panel_manager.selected_element); }

        if (layer_manager.layers.draw_droplet_groups.checkbox.checked || layer_manager.layers.draw_droplet_animations.checkbox.checked) { draw_grouped_droplets(); }

        if (information_panel_manager.double_clicked) { information_panel_manager.draw_multiple_selection(p); }

        if (layer_manager.layers.draw_bubbles.checkbox.checked) { draw_bubbles(); }
        

        // Controls animations
        if (layer_manager.layers.draw_droplet_animations.checkbox.checked) {

            let on_time_lerp = ((Date.now() - gui_broker.simulator_time) / 1000) / (gui_broker.board.currentTime - gui_broker.simulator_prev_time);
            let on_count_lerp = p.constrain(lerp_amount, 0, 1);

            let c_lerp_amount = p.constrain(p.max(on_time_lerp, on_count_lerp), 0, 1);

            // For same groups
            for (i in gui_broker.droplet_groups) {
                lerpGroupVertices(i, c_lerp_amount);
            }

            // For group splits
            let new_groups = findGroupSplit(0);
            for (i in new_groups) {
                lerpGroupVertices(new_groups[i], c_lerp_amount, 0);
            }
        }

        // Control simulator stepping
        if (gui_broker.play_status && lerp_amount >= 1.3 && !layer_manager.layers.real_time.checkbox.checked && layer_manager.layers.draw_droplet_animations.checkbox.checked) {
            gui_broker.next_simulator_step();
            lerp_amount = 0;
            gui_broker.animate = false;

        } else if (gui_broker.play_status && !layer_manager.layers.draw_droplet_animations.checkbox.checked && !layer_manager.layers.real_time.checkbox.checked) {
            gui_broker.next_simulator_step();

        } else if (gui_broker.animate && lerp_amount >= 1) {
            gui_broker.animate = false;

        }


        if (layer_manager.layers.draw_droplets.checkbox.checked) { draw_droplet(); }


        // Get next simulator step
        if (gui_broker.play_status && (((Date.now() - gui_broker.simulator_time) / 1000) + gui_broker.simulator_prev_time) >= gui_broker.board.currentTime) {
            console.log("Ellapsed time: " + ((Date.now() - gui_broker.simulator_time) / 1000) + " seconds", "Actual time: " + (((Date.now() - gui_broker.simulator_time) / 1000) + gui_broker.simulator_prev_time),"Previous time: " + gui_broker.simulator_prev_time,"Current time: " + gui_broker.board.currentTime);
            lerp_amount = 1;
            gui_broker.next_simulator_step_time(-1);
        }

    }


    function findGroupSplit(groupID) {

        let cur_group = gui_broker.droplet_groups[groupID];
        let prev_group = gui_broker.prev_droplet_groups[groupID];
        if (typeof cur_group == "undefined" || typeof prev_group == "undefined") { return; }

        if (!(Object.keys(gui_broker.prev_droplet_groups).length < Object.keys(gui_broker.droplet_groups).length)) { return; }

        let possible_split_droplets = [];
        for (i in prev_group) {
            for (j in cur_group) {

                if (prev_group[i].ID != cur_group[j].ID) {
                    possible_split_droplets.push(prev_group[i].ID);
                }
            }
        }

        let new_groups = [];
        for (i in possible_split_droplets) {
            for (j in gui_broker.droplets) {
                if (possible_split_droplets[i] == gui_broker.droplets[j].ID && gui_broker.droplets[j].group != groupID) {
                    if (!new_groups.includes(gui_broker.droplets[j].group)) { new_groups.push(gui_broker.droplets[j].group); }
                }
            }
        }

        return new_groups;
    }


    function lerpGroupVertices(groupID, amount, prevGroupID) { // prevGroupID is optional


        let cur_group = gui_broker.droplet_groups[groupID];

        if (typeof prevGroupID == "undefined") { prevGroupID = groupID }

        let prev_group = gui_broker.prev_droplet_groups[prevGroupID];

        if (typeof cur_group == "undefined") { return; }

        if (typeof prev_group != "undefined") {
            let vertex_pairs_before = getGroupVertexPairsForLerp(groupID, prevGroupID);
            let vertex_pairs = getGroupVertexPairsForLerp(groupID, prevGroupID);

            let vertX;
            let vertY;
            //[[[x1, x2], [y1, y2]] ... ]

            for (i in vertex_pairs) {
                vertX = p.lerp(vertex_pairs[i][0][0], vertex_pairs[i][0][1], amount);
                vertY = p.lerp(vertex_pairs[i][1][0], vertex_pairs[i][1][1], amount);


                vertex_pairs[i][0][0] = vertX;
                vertex_pairs[i][1][0] = vertY;

                for (a in cur_group.vertices) {
                    let vert = cur_group.vertices[a];
                    for (b in vertex_pairs_before) {

                        if (vert[0] == vertex_pairs_before[b][0][1] && vert[1] == vertex_pairs_before[b][1][1] && vertex_pairs_before[b][0][1] == vertex_pairs[i][0][1] && vertex_pairs_before[b][1][1] == vertex_pairs[i][1][1]) {
                            gui_broker.droplet_groups[groupID].vertices[a] = [vertX, vertY];
                        }
                    }
                }
            }


        }

        if (typeof gui_broker.droplet_groups[groupID] == "undefined") { return; }

        let points_vector = [];
        for (let j = 0; j < gui_broker.droplet_groups[groupID].vertices.length; j++) {
            points_vector.push(p.createVector(p.round(gui_broker.droplet_groups[groupID].vertices[j][0], 2), p.round(gui_broker.droplet_groups[groupID].vertices[j][1], 2)));
        }

        p.fill(gui_broker.droplet_groups[groupID][0].color);
        drawRounded(p, points_vector, 50);

        /*p.fill("black");
        for (let j = 0; j < gui_broker.droplet_groups[groupID].vertices.length; j++) {
            p.ellipse(gui_broker.droplet_groups[groupID].vertices[j][0], gui_broker.droplet_groups[groupID].vertices[j][1], 3, 3);
        }*/
    }

    function getGroupVertexPairsForLerp(groupID, prevGroupID) {
        let cur_group = gui_broker.droplet_groups[groupID];

        if (typeof prevGroupID == "undefined") { prevGroupID = groupID }
        let prev_group = gui_broker.prev_droplet_groups[prevGroupID];

        if (typeof cur_group == "undefined" || typeof prev_group == "undefined") { return; }

        let vertices_to_lerp = getGroupVerticesToLerp(groupID, prevGroupID);

        let vertex_pairs = [];
        // [[[x1,x2],[y1,y2]] ... ]

        // Find closes point in current group
        for (i in vertices_to_lerp) {
            let min_dist = [9999, "point"]; // min_dist, point
            for (j in cur_group.vertices) {
                let dist = p.dist(vertices_to_lerp[i][0], vertices_to_lerp[i][1], cur_group.vertices[j][0], cur_group.vertices[j][1]);
                if (dist < min_dist[0]) {
                    min_dist = [dist, cur_group.vertices[j]];
                }
            }
            vertex_pairs.push([[vertices_to_lerp[i][0], min_dist[1][0]], [vertices_to_lerp[i][1], min_dist[1][1]]]);
            vertices_to_lerp.push([min_dist[1][0], min_dist[1][1]]);
        }

        return vertex_pairs;

    }

    function getGroupVerticesToLerp(groupID, prevGroupID) {
        let cur_group = gui_broker.droplet_groups[groupID];

        if (typeof prevGroupID == "undefined") { prevGroupID = groupID }
        let prev_group = gui_broker.prev_droplet_groups[prevGroupID];

        if (typeof cur_group == "undefined" || typeof prev_group == "undefined") { return; }

        // Find vertices to lerp
        let newArray = [];

        for (i in prev_group.vertices) {
            let min_dist = 9999;
            for (j in cur_group.vertices) {
                let dist = p.dist(prev_group.vertices[i][0], prev_group.vertices[i][1], cur_group.vertices[j][0], cur_group.vertices[j][1]);
                min_dist = p.min(min_dist, dist);
            }

            if (min_dist > cur_group[0].sizeX / 2) {
                newArray.push([prev_group.vertices[i][0], prev_group.vertices[i][1]]);
            }
        }

        return newArray;
    }



    /* Handle key presses */
    p.keyPressed = function () {

        // Escape will clear the information panel, enter will save it
        if (information_panel_manager.selected_element != null && p.keyCode == 27) {
            information_panel_manager.clear();
        } else if (information_panel_manager.selected_element != null && information_panel_manager.editing && p.keyCode == 13) {
            information_panel_manager.onSave();
        }


        
        if (information_panel_manager.double_clicked) {
            if (p.key <= Object.keys(information_panel_manager.multiple_selection).length) {
                console.log("inside");
                //information_panel_manager.selected_element = information_panel_manager.multiple_selection[Object.keys(information_panel_manager.multiple_selection)[p.key - 1]];
                console.log(Object.keys(information_panel_manager.multiple_selection)[p.key - 1], information_panel_manager.multiple_selection[Object.keys(information_panel_manager.multiple_selection)[p.key - 1]]);

                let type = Object.keys(information_panel_manager.multiple_selection)[p.key - 1];
                let element = information_panel_manager.multiple_selection[Object.keys(information_panel_manager.multiple_selection)[p.key - 1]];
                let groupID = (typeof element[0] === 'undefined') ? null : element[0].group;

                information_panel_manager.selected_element = information_panel_manager.information_filter(type, element, groupID);
                information_panel_manager.information_element = information_panel_manager.information_filter(type, element, groupID);
                information_panel_manager.draw_information(information_panel_manager.information_filter(type, element, groupID));

                information_panel_manager.double_clicked = false;
                information_panel_manager.multiple_selection = null;
            }
        }
    }

    /** Handle single mouse clicks */
    function onMouseClicked() {
        information_panel_manager.double_clicked = false;
        information_panel_manager.multiple_selection = null;

        // Handle click on droplet group
        if (layer_manager.layers.draw_droplet_groups.checkbox.checked) {
            for (let i in gui_broker.droplet_groups) {
                if (polygon_contains(gui_broker.droplet_groups[i].vertices, p.mouseX, p.mouseY)) {
                    information_panel_manager.selected_element = information_panel_manager.information_filter("Group", gui_broker.droplet_groups[i], i);
                    information_panel_manager.information_element = information_panel_manager.information_filter("Group", gui_broker.droplet_groups[i], i);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Group", gui_broker.droplet_groups[i], i));
                    return;
                }
            }
        }

        // Handle click on droplet
        if (layer_manager.layers.draw_droplets.checkbox.checked) {
            for (let i in gui_broker.droplets) {
                let droplet = gui_broker.droplets[i];

                // Check mouse over droplet
                if (p.dist(p.mouseX, p.mouseY, droplet.positionX, droplet.positionY) < droplet.sizeX / 2) {
                    information_panel_manager.selected_element = droplet;
                    information_panel_manager.information_element = information_panel_manager.information_filter("Droplet", droplet);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Droplet", droplet));
                    return;
                }
            }
        }

        // Handle click on sensor
        if (layer_manager.layers.draw_sensors.checkbox.checked) {
            for (let i in gui_broker.board.sensors) {
                let sensor = gui_broker.board.sensors[i];
                let vertexes = [[sensor.positionX, sensor.positionY], [sensor.positionX + sensor.sizeX, sensor.positionY], [sensor.positionX + sensor.sizeX, sensor.positionY + sensor.sizeY], [sensor.positionX, sensor.positionY + sensor.sizeY]];

                if (polygon_contains(vertexes, p.mouseX, p.mouseY)) {
                    information_panel_manager.selected_element = sensor;
                    information_panel_manager.information_element = information_panel_manager.information_filter("Sensor", sensor);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Sensor", sensor));
                    return;
                }
            }
        }

        // Handle click on actuator
        if (layer_manager.layers.draw_actuators.checkbox.checked) {
            for (let i in gui_broker.board.actuators) {
                let actuator = gui_broker.board.actuators[i];
                let vertexes = [[actuator.positionX, actuator.positionY], [actuator.positionX + actuator.sizeX, actuator.positionY], [actuator.positionX + actuator.sizeX, actuator.positionY + actuator.sizeY], [actuator.positionX, actuator.positionY + actuator.sizeY]];

                if (polygon_contains(vertexes, p.mouseX, p.mouseY)) {
                    information_panel_manager.selected_element = actuator;
                    information_panel_manager.information_element = information_panel_manager.information_filter("Actuator", actuator);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Actuator", actuator));
                    return;
                }
            }
        }

        // Handle click on electrode
        for (let i in gui_broker.electrodes) {
            let electrode = gui_broker.electrodes[i];
            if (electrodeContains(electrode, p.mouseX, p.mouseY)) {
                information_panel_manager.selected_element = electrode;
                information_panel_manager.information_element = information_panel_manager.information_filter("Electrode", electrode);
                information_panel_manager.draw_information(information_panel_manager.information_filter("Electrode", electrode));
                return;
            }
        }
    }

    function onMouseDoubleClicked() {


        let list_of_elements = {};
        // Handle click on droplet group
        if (layer_manager.layers.draw_droplet_groups.checkbox.checked) {
            for (let i in gui_broker.droplet_groups) {
                if (polygon_contains(gui_broker.droplet_groups[i].vertices, p.mouseX, p.mouseY)) {
                    list_of_elements["Group"] = gui_broker.droplet_groups[i];
                }
            }
        }

        // Handle click on droplet
        if (layer_manager.layers.draw_droplets.checkbox.checked) {
            for (let i in gui_broker.droplets) {
                let droplet = gui_broker.droplets[i];

                // Check mouse over droplet
                if (p.dist(p.mouseX, p.mouseY, droplet.positionX, droplet.positionY) < droplet.sizeX / 2) {
                    list_of_elements["Droplet"] = (droplet);
                }
            }
        }

        // Handle click on sensor
        if (layer_manager.layers.draw_sensors.checkbox.checked) {
            for (let i in gui_broker.board.sensors) {
                let sensor = gui_broker.board.sensors[i];
                let vertexes = [[sensor.positionX, sensor.positionY], [sensor.positionX + sensor.sizeX, sensor.positionY], [sensor.positionX + sensor.sizeX, sensor.positionY + sensor.sizeY], [sensor.positionX, sensor.positionY + sensor.sizeY]];

                if (polygon_contains(vertexes, p.mouseX, p.mouseY)) {
                    list_of_elements["Sensor"] = sensor;
                }
            }
        }

        // Handle click on actuator
        if (layer_manager.layers.draw_actuators.checkbox.checked) {
            for (let i in gui_broker.board.actuators) {
                let actuator = gui_broker.board.actuators[i];
                let vertexes = [[actuator.positionX, actuator.positionY], [actuator.positionX + actuator.sizeX, actuator.positionY], [actuator.positionX + actuator.sizeX, actuator.positionY + actuator.sizeY], [actuator.positionX, actuator.positionY + actuator.sizeY]];

                if (polygon_contains(vertexes, p.mouseX, p.mouseY)) {
                    list_of_elements["Actuator"] = actuator;
                }
            }
        }

        // Handle click on electrode
        for (let i in gui_broker.electrodes) {
            let electrode = gui_broker.electrodes[i];
            if (electrodeContains(electrode, p.mouseX, p.mouseY)) {
                list_of_elements["Electrode"] = (electrode);
            }
        }
        console.log("double clicked", list_of_elements);

        if (Object.keys(list_of_elements).length > 1) {
            information_panel_manager.selected_element = null;
            information_panel_manager.clear();

            information_panel_manager.double_clicked = true;
            information_panel_manager.multiple_selection = list_of_elements;
        }
    }

    /**
     * Checks whether a point (x, y) is contained within an electrode.
     * @param {any} electrode
     * @param {any} x
     * @param {any} y
     */
    function electrodeContains(electrode, x, y) {
        let vertexes = [];
        if (electrode.shape == 0) {
            vertexes = [[0, 0], [electrode.sizeX, 0], [electrode.sizeX, electrode.sizeY], [0, electrode.sizeY]];
        } else {
            vertexes = electrode.corners;
        }

        let i;
        let j;
        let result = false;
        for (i = 0, j = vertexes.length - 1; i < vertexes.length; j = i++) {
            let ivertX = vertexes[i][0] + electrode.positionX;
            let ivertY = vertexes[i][1] + electrode.positionY;
            let jvertX = vertexes[j][0] + electrode.positionX;
            let jvertY = vertexes[j][1] + electrode.positionY;

            if ((ivertY > y) != (jvertY > y) && (x < (jvertX - ivertX)
                * (y - ivertY) / (jvertY - ivertY) + ivertX)) {
                result = !result;
            }
        }

        return result;
    }

    function polygon_contains(vertexes, x, y) {
        let i;
        let j;
        let result = false;
        for (i = 0, j = vertexes.length - 1; i < vertexes.length; j = i++) {
            let ivertX = vertexes[i][0];
            let ivertY = vertexes[i][1];
            let jvertX = vertexes[j][0];
            let jvertY = vertexes[j][1];

            if ((ivertY > y) != (jvertY > y) && (x < (jvertX - ivertX)
                * (y - ivertY) / (jvertY - ivertY) + ivertX)) {
                result = !result;
            }
        }
        return result;
    }


    function draw_grouped_droplets() {
        // TESTING GROUPED DROPLETS
        for (let i in gui_broker.droplet_groups) {
            let check_size = gui_broker.electrodes[0].sizeX / 2;
            let current_droplet = gui_broker.droplet_groups[i][0];
            let current_droplet_point = [current_droplet.positionX - check_size, current_droplet.positionY - check_size];
            let concave_point = null;

            //let draw_size = current_droplet.sizeX / 2;
            let droplet_draw_point = [current_droplet.positionX - current_droplet.sizeX / 2, current_droplet.positionY - current_droplet.sizeX / 2];

            // Change so case where theres no path from current point is valid
            let points_to_draw = [];

            while (JSON.stringify(points_to_draw).indexOf(JSON.stringify(droplet_draw_point)) == -1) {
                //for (let ab = 0; ab < 4; ab++) {
                let top_left = null
                let top_right = null;
                let bottom_left = null;
                let bottom_right = null;

                for (let j in gui_broker.droplet_groups[i]) {
                    let other_droplet = gui_broker.droplet_groups[i][j];

                    // Check cases
                    if (other_droplet.positionX == current_droplet_point[0] + check_size && other_droplet.positionY == current_droplet_point[1] - check_size) {
                        top_right = other_droplet;
                    } else if (other_droplet.positionX == current_droplet_point[0] - check_size && other_droplet.positionY == current_droplet_point[1] + check_size) {
                        bottom_left = other_droplet;
                    } else if (other_droplet.positionX == current_droplet_point[0] - check_size && other_droplet.positionY == current_droplet_point[1] - check_size) {
                        top_left = other_droplet;
                    } else if (other_droplet.positionX == current_droplet_point[0] + check_size && other_droplet.positionY == current_droplet_point[1] + check_size) {
                        bottom_right = other_droplet;
                    }
                }

                if (top_right == null && bottom_right != null) {

                    // Concave corner case
                    if (top_left != null && bottom_left != null) {
                        concave_point = [top_left.positionX + top_left.sizeY / 2, bottom_right.positionY - bottom_right.sizeY / 2];

                        if (JSON.stringify(points_to_draw).indexOf(
                            JSON.stringify(concave_point)) == -1) {
                            points_to_draw.push(concave_point);
                        }
                    }

                    // Convex corner case
                    if (bottom_left == null) {
                        points_to_draw.push(droplet_draw_point);
                    }

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([bottom_right.positionX - bottom_right.sizeX / 2,
                        bottom_right.positionY - bottom_right.sizeY / 2])) == -1 && bottom_left == null) {
                        points_to_draw.push([bottom_right.positionX - bottom_right.sizeX / 2, bottom_right.positionY - bottom_right.sizeY / 2]);
                    }

                    droplet_draw_point = [bottom_right.positionX + bottom_right.sizeX / 2, bottom_right.positionY - bottom_right.sizeY / 2];
                    current_droplet_point = [current_droplet_point[0] + check_size * 2, current_droplet_point[1]];

                } else if (bottom_right == null && bottom_left != null) {

                    // Concave corner case
                    if (top_left != null && top_right != null) {
                        concave_point = [bottom_left.positionX + bottom_left.sizeY / 2, top_right.positionY + top_right.sizeY / 2];


                        if (JSON.stringify(points_to_draw).indexOf(
                            JSON.stringify(concave_point)) == -1) {
                            points_to_draw.push(concave_point);
                        }
                    }

                    // Convex corner case
                    if (top_left == null) {
                        points_to_draw.push(droplet_draw_point);
                    }

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([bottom_left.positionX + bottom_left.sizeX / 2,
                        bottom_left.positionY - bottom_left.sizeY / 2])) == -1 && top_left == null) {
                        points_to_draw.push([bottom_left.positionX + bottom_left.sizeX / 2, bottom_left.positionY - bottom_left.sizeY / 2]);
                    }

                    droplet_draw_point = [bottom_left.positionX + bottom_left.sizeX / 2, bottom_left.positionY + bottom_left.sizeY / 2];
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] + check_size * 2];

                } else if (bottom_left == null && top_left != null) {

                    // Concave corner case
                    if (bottom_right != null && top_right != null) {
                        concave_point = [bottom_right.positionX - bottom_right.sizeY / 2, top_left.positionY + top_left.sizeY / 2];

                        if (JSON.stringify(points_to_draw).indexOf(
                            JSON.stringify(concave_point)) == -1) {
                            points_to_draw.push(concave_point);
                        }
                    }

                    // Convex corner case
                    if (top_right == null) {
                        points_to_draw.push(droplet_draw_point);
                    }

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([top_left.positionX + top_left.sizeX / 2,
                        top_left.positionY + top_left.sizeY / 2])) == -1 && top_right == null) {
                        points_to_draw.push([top_left.positionX + top_left.sizeX / 2, top_left.positionY + top_left.sizeY / 2]);
                    }

                    droplet_draw_point = [top_left.positionX - top_left.sizeX / 2, top_left.positionY + top_left.sizeY / 2];
                    current_droplet_point = [current_droplet_point[0] - check_size * 2, current_droplet_point[1]];

                } else if (top_left == null && top_right != null) {

                    // Concave corner case
                    if (bottom_right != null && bottom_left != null) {
                        concave_point = [top_right.positionX - top_right.sizeY / 2, bottom_left.positionY - bottom_left.sizeY / 2];

                        if (JSON.stringify(points_to_draw).indexOf(
                            JSON.stringify(concave_point)) == -1) {
                            points_to_draw.push(concave_point);
                        }
                    }

                    // Convex corner case
                    if (bottom_right == null) {
                        points_to_draw.push(droplet_draw_point);
                    }

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([top_right.positionX - top_right.sizeX / 2,
                        top_right.positionY + top_right.sizeY / 2])) == -1 && bottom_right == null) {
                        points_to_draw.push([top_right.positionX - top_right.sizeX / 2, top_right.positionY + top_right.sizeY / 2]);
                    }

                    droplet_draw_point = [top_right.positionX - top_right.sizeX / 2, top_right.positionY - top_right.sizeY / 2];
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] - check_size * 2];
                }
            }


            p.fill(current_droplet.color);

            // Check to see if the droplet group is selected
            if (information_panel_manager.selected_element != null && typeof information_panel_manager.selected_element.groupID != "undefined" && information_panel_manager.selected_element.groupID == current_droplet.group) {
                p.stroke(draw_config.group.selectedBorderColor);
                p.strokeWeight(draw_config.group.selectedBorderWidth);
            } else {
                p.stroke(draw_config.group.borderColor);
                p.strokeWeight(draw_config.group.borderWidth);
            }

            let points_vector = [];
            for (let j = 0; j < points_to_draw.length; j++) {
                points_vector.push(p.createVector(p.round(points_to_draw[j][0], 2), p.round(points_to_draw[j][1], 2)));
            }

            //console.log(points_vector);
            gui_broker.droplet_groups[current_droplet.group].vertices = points_to_draw;

            if (!layer_manager.layers.draw_droplet_animations.checkbox.checked) {
                drawRounded(p, points_vector, 50);
            }


            //p.beginShape();
            /*for (let j = 0; j < points_to_draw.length; j++) {
                p.ellipse(points_to_draw[j][0], points_to_draw[j][1],5,5);
            }*/
            //p.endShape(p.CLOSE);

        }
    }

    /**
     * Will round the corners of a set of vertices.
     * From https://gorillasun.de/blog/An-algorithm-for-polygons-with-rounded-corners
     * @param {any} sketch
     * @param {any} points
     * @param {any} r
     */
    function drawRounded(sketch, points, r) {
        sketch.beginShape();
        for (let i = 0; i < points.length; i++) {
            const a = points[i];
            const b = points[(i + 1) % points.length];
            const c = points[(i + 2) % points.length];
            const ba = a.copy().sub(b).normalize();
            const bc = c.copy().sub(b).normalize();

            // Points in the direction the corner is accelerating towards
            const normal = ba.copy().add(bc).normalize();

            // Shortest angle between the two edges
            const theta = ba.angleBetween(bc);

            // If theta is 0, theres no change in the angle, and we can continue
            if (theta == 0) { continue; }

            // Find the circle radius that would cause us to round off half
            // of the shortest edge. We leave the other half for neighbouring
            // corners to potentially cut.
            const maxR = sketch.min(a.dist(b), c.dist(b)) / 2 * sketch.abs(sketch.sin(theta / 2));
            const cornerR = sketch.min(r, maxR);
            // Find the distance away from the corner that has a distance of
            // 2*cornerR between the edges
            const distance = sketch.abs(cornerR / sketch.sin(theta / 2));

            // Approximate an arc using a cubic bezier
            const c1 = b.copy().add(ba.copy().mult(distance));
            const c2 = b.copy().add(bc.copy().mult(distance));
            const bezierDist = 0.5523; // https://stackoverflow.com/a/27863181
            const p1 = c1.copy().sub(ba.copy().mult(2 * cornerR * bezierDist));
            const p2 = c2.copy().sub(bc.copy().mult(2 * cornerR * bezierDist));
            sketch.vertex(c1.x, c1.y);
            sketch.bezierVertex(
                p1.x, p1.y,
                p2.x, p2.y,
                c2.x, c2.y
            )

            //console.log(p1, p2);
        }
        sketch.endShape(sketch.CLOSE);
    }


    /* Initialize board values */
    function init_board(sizeX, sizeY) {
        gui_broker.sketch_ref = p;

        p.resizeCanvas(sizeX + 1, sizeY);

        //layer_electrode_id = p.createGraphics(sizeX + 1, sizeY);


        for (let layer in layer_manager.layers) {
            if (layer_manager.layers[layer].hasOwnProperty("layer")) {
                layer_manager.layers[layer].layer = p.createGraphics(sizeX + 1, sizeY);
            }
        }
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            debug_electrode_text(layer_manager.layers.debug_electrode_text.layer, electrode);
        }

        draw_actuators(layer_manager.layers.draw_actuators.layer);
        draw_sensors(layer_manager.layers.draw_sensors.layer);

        layer_electrode_shape = p.createGraphics(sizeX + 1, sizeY);

        draw_electrodes_shapes();
    }
    gui_broker.init_board = init_board; // Attach the function to the GUI broker.

    // TODO: Move to layer_panel manager????
    /**
     * Draws the selected element
     * @param {any} layer
     * @param {any} element
     */
    function draw_selected_element(layer, element) {
        layer.clear();
        if (element == null) { return; }

        
        if (element.status != "undefined") {

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
    }

    /* Call to draw active electrodes */
    function draw_active_electrodes() {
        /* TODO: Maybe send information of which electrode is active from the simulator */
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];

            if (electrode.status == 0) { continue; }
            p.fill(draw_config.electrode.activeColor);
            
            // Check the electrode shape
            if (electrode.shape == 1) {
                p.beginShape();
                for (let i = 0; i < electrode.corners.length; i++) {
                    p.vertex(electrode.positionX + electrode.corners[i][0] + 0.5, electrode.positionY + electrode.corners[i][1] + 0.5);
                }
                p.endShape(p.CLOSE);
            } else {
                p.rect(electrode.positionX, electrode.positionY, electrode.sizeX, electrode.sizeY);
            }
        }
    }

    /* Call to draw electrode shapes */
    function draw_electrodes_shapes() {
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];

            layer_electrode_shape.stroke(draw_config.electrode.borderColor);
            layer_electrode_shape.strokeWeight(draw_config.electrode.borderWidth);
            layer_electrode_shape.fill(draw_config.electrode.backgroundColor);
            //if (electrode.status != 0) { layer_electrode_shape.fill("red"); }

            // Check the electrode shape
            if (electrode.shape == 1) {
                draw_polygon_electrode_shapes(electrode.positionX, electrode.positionY, electrode.corners);
            } else {
                layer_electrode_shape.rect(electrode.positionX, electrode.positionY, electrode.sizeX, electrode.sizeY);
            }
        }
    }

    /* Call to draw polygonal shaped electrode shapes */
    function draw_polygon_electrode_shapes(posX, posY, corners) {
        layer_electrode_shape.beginShape();
        for (let i = 0; i < corners.length; i++) {
            layer_electrode_shape.vertex(posX + corners[i][0] + 0.5, posY + corners[i][1] + 0.5);
        }
        layer_electrode_shape.endShape(layer_electrode_shape.CLOSE);
    }

    /* Call to draw droplets */
    function draw_droplet() {

        for (let i = 0; i < gui_broker.droplets.length; i++) {
            let droplet = gui_broker.droplets[i];
            p.fill(droplet.color);
            p.stroke(draw_config.droplet.borderColor);
            p.strokeWeight(draw_config.droplet.borderWidth);
            p.ellipse(droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY);
            //anim_move(droplet, i);
        }
    }

    /* Call to draw actuators */
    function draw_actuators(layer) {
        let actuators = gui_broker.board.actuators;
        actuators.forEach((actuator) => {
            //console.log(actuator);
            let color = layer.color(draw_config.actuator.backgroundColor);
            color.setAlpha(draw_config.actuator.backgroundOpacity);
            layer.fill(color);
            layer.stroke(draw_config.actuator.borderColor);
            layer.strokeWeight(draw_config.actuator.borderWidth);
            layer.rect(actuator.positionX, actuator.positionY, actuator.sizeX, actuator.sizeY);
        })
    }

    /* Call to draw actuators */
    function draw_sensors(layer) {
        let sensors = gui_broker.board.sensors;
        sensors.forEach((sensor) => {
            //console.log(actuator);
            let color = layer.color(draw_config.sensor.backgroundColor);
            color.setAlpha(draw_config.sensor.backgroundOpacity);
            layer.fill(color);
            layer.stroke(draw_config.sensor.borderColor);
            layer.strokeWeight(draw_config.sensor.borderWidth);
            layer.rect(sensor.positionX, sensor.positionY, sensor.sizeX, sensor.sizeY);
        })
    }

    /* Call to draw bubbles */
    function draw_bubbles() {
        let bubbles = gui_broker.board.bubbles;

        if (typeof bubbles != 'undefined') {
            p.noFill();
            p.stroke(draw_config.bubble.borderColor);
            p.strokeWeight(draw_config.bubble.borderWidth);
            bubbles.forEach((bubble) => {
                p.ellipse(bubble.positionX, bubble.positionY, bubble.sizeX, bubble.sizeY);
            })
        }
    }

    /**
     * Call to draw ID's of all electrodes (used for debugging)
     * @param {any} layer
     * @param {any} electrode
     */
    function debug_electrode_text(layer, electrode) {
        let pos_x = 0;
        let pos_y = 0;

        if (electrode.shape == 1) {
            layer.fill(0, 255, 0);
            let corner_sum_x = 0;
            let corner_sum_y = 0;

            
            for (let i = 0; i < electrode.corners.length; i++) {
                corner_sum_x = parseInt(corner_sum_x) + (parseInt(electrode.positionX) + parseInt(electrode.corners[i][0]));
                corner_sum_y = parseInt(corner_sum_y) + (parseInt(electrode.positionY) + parseInt(electrode.corners[i][1]));
            }

            pos_x = (corner_sum_x) / (electrode.corners.length) - layer.textWidth(electrode.ID) / 2;
            pos_y = (corner_sum_y) / (electrode.corners.length) + layer.textAscent(electrode.ID) / 2;
            
        } else {
            layer.fill(255, 200, 0);
            pos_x = electrode.positionX + electrode.sizeX / 2 - layer.textWidth(electrode.ID)/2;
            pos_y = electrode.positionY + electrode.sizeY / 2 + layer.textAscent(electrode.ID)/2;
        }

        layer.textSize(6);
        layer.text(electrode.ID, pos_x, pos_y);
    }

};