/* 
 * Written by Joel A. V. Madsen
 */


// Global function to setup the p5js instance
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};


// TODO: Look into reflections
// Global methods that can be called by C# scripts
window.update_board = (container_string) => {
    var board = JSON.parse(container_string);
    gui_broker.droplets = board.Droplets;
    gui_broker.electrodes = board.Electrodes;

    droplet_info.old = droplet_info.new;
    droplet_info.new = gui_broker.droplets;

    //information_panel_manager.draw_information(board.Electrodes[130]);
    gui_broker.get_droplet_groups();

    amount = 0;
}
window.change_play_status = (status) => {
    gui_broker.play_status = !gui_broker.play_status;
};
window.initialize_board = (information) => {
    var JSONinformation = JSON.parse(information);
    console.log(JSONinformation);
    gui_broker.init_board(JSONinformation.sizeX, JSONinformation.sizeY + 1);

    

    gui_controller.showGUI();
    gui_controller.changeBoardName(JSONinformation.platform_name);
    //console.log(gui_controller.getInputNodes());

    layer_manager.initialize_layers();
}


let gui_controller = {
    simulatorGUI: document.querySelector("#simulatorGUI"),
    getLayerPanel: () => { return this.simulatorGUI.querySelector("#layerPanel") },
    getInputNodes: () => { return this.simulatorGUI.querySelector("#layerPanel").getElementsByTagName('INPUT'); },
    getInformaitonPanel: () => { return this.simulatorGUI.querySelector("#information"); },
    changeBoardName: (name) => { this.simulatorGUI.querySelector("#simulatorView span").innerHTML = name; },
    showGUI: () => { simulatorGUI.style.visibility = "visible"; }
}



let information_panel_manager = {
    //information_panel: gui_controller.getInformaitonPanel(),
    selected_element: null,
    draw_information: (element) => {
        gui_controller.getInformaitonPanel().innerHTML = "";

        let div = document.createElement("div");
        
        for (let key in element) {
            let innerDiv = document.createElement("div");
            let innerSpan = document.createElement("span");
            innerDiv.innerHTML = key + ": ";
            innerSpan.innerHTML = element[key];
            innerDiv.append(innerSpan);
            div.append(innerDiv);
        }

        
        //div.innerHTML = JSON.stringify(element);
        //console.log(JSON.stringify(element));
        gui_controller.getInformaitonPanel().append(div);
    }
}


let layer_manager = {


    layers: {
        draw_droplets: {
            name: "droplet_draw_call",
            value: "droplet_draw_call",
            id: "draw_droplet",
            text: "Draw Droplets", // Will be shown in layer panel list
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


/* 
 * Declaration of script "global" variables.
 * These variables are used by both the global functions but also by the p5js script.
 */
let simulator_droplets = [];
let simulator_electrodes = [];
let amount = 0;     // Used to interpolate between droplet positions.


// TODO: Refrator into a layer array, that will automatically create them
let layer_electrode_id;
let layer_electrode_shape;
let debug1 = document.getElementById("debug1"); // CHANGE to debug_checklist

/* 
 * gui_broker object 
 * This object is used to store data from the simulator. 
 * It basically acts as the broker between the GUI and simulation.
 */
let gui_broker = {
    play_status: false,
    droplets: [],
    electrodes: [],
    droplet_groups: { },
    next_simulator_step: () => {
        DotNet.invokeMethodAsync('MicrofluidSimulator', 'JSSimulatorNextStep');
    },
    init_board: () => { },
    get_droplet_groups: function () {
        this.droplet_groups = {};

        for (let i = 0; i < this.droplets.length; i++) {
            if (typeof this.droplet_groups[this.droplets[i].Group] == "undefined") {
                this.droplet_groups[this.droplets[i].Group] = [(this.droplets[i])];
            } else {
                this.droplet_groups[this.droplets[i].Group].push(this.droplets[i]);
            }
        }
    }
};

/*
 * Droplet_info is used by the animation function, 
 * and stores the data from the old position of all droplets, and the new positions.
 */
let droplet_info = {
    old: [],
    new: []
};

/* Attach the GUI broker to the window, so it can be "seen" by C# scripts */
window.gui_broker = gui_broker;

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
        console.log(canvas.position());

        // Create layers
        layer_electrode_id = p.createGraphics(1, 1);
        layer_electrode_shape = p.createGraphics(1, 1);
        console.log("setup");

        //p.frameRate(10);
        step = 0.05;

        let button = p.select("#nextStep");
        button.mousePressed(() => {
            console.log("HI FROM SKETCH");
            // Below is a test for resize
            //let container = document.getElementById("container");
            //console.log(window.innerWidth);
            //document.getElementById("defaultCanvas0").style.width = (window.innerWidth * 0.8) + "px";
            //document.getElementById("defaultCanvas0").style.height = ((window.innerWidth * 0.8) * (400/860)) + "px";
        });
    }

    function onMouseClicked() {
        console.log("logging: " + p.mouseX + ", " + p.mouseY);

        for (let i in gui_broker.droplets) {
            let droplet = gui_broker.droplets[i];

            // Check mouse over droplet
            if (p.dist(p.mouseX, p.mouseY, droplet.PositionX, droplet.PositionY) < droplet.SizeX/2) {
                information_panel_manager.selected_element = droplet;
                information_panel_manager.draw_information(droplet);
                return;
            }
        }

        for (let i in gui_broker.electrodes) {
            if (electrodeContains(gui_broker.electrodes[i], p.mouseX, p.mouseY)) {
                information_panel_manager.selected_element = gui_broker.electrodes[i];
                information_panel_manager.draw_information(gui_broker.electrodes[i]);
                return;
            }
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
        if (electrode.Shape == 0) {
            vertexes = [[0, 0], [electrode.SizeX, 0], [electrode.SizeX, electrode.SizeY], [0, electrode.SizeY]];
        } else {
            vertexes = electrode.Corners;
        }

        let i;
        let j;
        let result = false;
        for (i = 0, j = vertexes.length - 1; i < vertexes.length; j = i++) {
            let ivertX = vertexes[i][0] + electrode.PositionX;
            let ivertY = vertexes[i][1] + electrode.PositionY;
            let jvertX = vertexes[j][0] + electrode.PositionX;
            let jvertY = vertexes[j][1] + electrode.PositionY;

            if ((ivertY > y) != (jvertY > y) && (x < (jvertX - ivertX)
                * (y - ivertY) / (jvertY - ivertY) + ivertX)) {
                result = !result;
            }
        }

        return result;
    }



    /*
     * Draw is called every frame of the sketch.
     */
    p.draw = function () {
        p.background(240);

        // Increment the step used for position animation
        if (amount < 1) {
            amount += step;
        } else if (gui_broker.play_status) {
            gui_broker.next_simulator_step(); // Calls for the next "step" of the simulation
        }

        /* Draw calls */
        //draw_electrodes();
        p.image(layer_electrode_shape, 0, 0);



        if (layer_manager.layers.draw_active_electrodes.checkbox.checked) { draw_active_electrodes(); }

        // Draw all direct layers
        layer_manager.draw_layers(p);

        if (layer_manager.layers.draw_droplets.checkbox.checked) { draw_droplet(); }

        if (layer_manager.layers.draw_selected_element.checkbox.checked) { draw_selected_element(layer_manager.layers.draw_selected_element.layer, information_panel_manager.selected_element); }





        // TESTING
        for (let i in gui_broker.droplet_groups) {
            let size = gui_broker.electrodes[0].SizeX/2;
            let current_droplet = gui_broker.droplet_groups[i][0];
            let current_droplet_point = [current_droplet.PositionX - size, current_droplet.PositionY - size];

            // Change so case where theres no path from current point is valid
            let points_to_draw = [];

            while (JSON.stringify(points_to_draw).indexOf(JSON.stringify(current_droplet_point)) == -1) {
            //for (let c = 0; c < 14; c++) {
                let top_left = null
                let top_right = null;
                let bottom_left = null;
                let bottom_right = null;

                //console.log(JSON.stringify(points_to_draw).indexOf(JSON.stringify(current_droplet_point)));

                for (let j in gui_broker.droplet_groups[i]) {
                    let other_droplet = gui_broker.droplet_groups[i][j];
                    //if (current_droplet == other_droplet) { continue; }
                    //console.log(other_droplet.ID1);
                    // Check cases
                    if (other_droplet.PositionX == current_droplet_point[0] + size && other_droplet.PositionY == current_droplet_point[1] - size) {
                        top_right = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] - size && other_droplet.PositionY == current_droplet_point[1] + size) {
                        bottom_left = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] - size && other_droplet.PositionY == current_droplet_point[1] - size) {
                        top_left = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] + size && other_droplet.PositionY == current_droplet_point[1] + size) {
                        bottom_right = other_droplet;
                    }
                }
                //console.log(top_left, top_right, bottom_left, bottom_right);

                if (top_right == null && bottom_right != null) {
                    points_to_draw.push(current_droplet_point);
                    current_droplet_point = [current_droplet_point[0] + size * 2, current_droplet_point[1]];
                } else if (bottom_right == null && bottom_left != null) {
                    points_to_draw.push(current_droplet_point);
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] + size * 2];
                    //points_to_draw.push(current_droplet_point);
                } else if (bottom_left == null && top_left != null) {
                    points_to_draw.push(current_droplet_point);
                    current_droplet_point = [current_droplet_point[0] - size * 2, current_droplet_point[1]];
                    //points_to_draw.push(current_droplet_point);
                } else if (top_left == null && top_right != null) {
                    points_to_draw.push(current_droplet_point);
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] - size * 2];
                    //points_to_draw.push(current_droplet_point);
                }
            }
            //}


            //console.log(points_to_draw);



            /*for (let j = 0; j < points_to_draw.length - 1; j++) {
                p.stroke("green");
                p.line(points_to_draw[j][0], points_to_draw[j][1], points_to_draw[j + 1][0], points_to_draw[j + 1][1]);
                p.fill("green");
                p.ellipse(points_to_draw[j][0], points_to_draw[j][1], 3, 3);
                p.ellipse(points_to_draw[j+1][0], points_to_draw[j+1][1], 3, 3);
            }*/
            p.fill(current_droplet.Color);
            p.beginShape();
            for (let j = 0; j < points_to_draw.length; j++) {
                p.vertex(points_to_draw[j][0], points_to_draw[j][1]);
            }
            p.endShape(p.CLOSE);

        }
        
        



        // DRAW DROPLET GROUPS (WIP)
        // For each group
        /*for (let i in gui_broker.droplet_groups) {
            let points = [];

            // For each droplet in group
            for (let j in gui_broker.droplet_groups[i]) {

                let droplet = gui_broker.droplet_groups[i][j];
                //let top, bottom, left, right = null;
                let top = null;
                let bottom = null;
                let left = null;
                let right = null;
                // For all other droplets in that group
                for (let k in gui_broker.droplet_groups[i]) {
                    let other_droplet = gui_broker.droplet_groups[i][k];

                    if (other_droplet == droplet) { continue; }
                    

                    if (p.dist(droplet.PositionX, droplet.PositionY,
                        other_droplet.PositionX, other_droplet.PositionY) <= gui_broker.electrodes[0].SizeX) {

                        if (other_droplet.PositionY < droplet.PositionY) { top = other_droplet; }
                        else if (other_droplet.PositionY > droplet.PositionY) { bottom = other_droplet; }
                        else if (other_droplet.PositionX < droplet.PositionX) { left = other_droplet; }
                        else if (other_droplet.PositionX > droplet.PositionX) { right = other_droplet; }
                    }

                    
                }


                if (top == null) {
                    points.push([droplet.PositionX - droplet.SizeX / 2, droplet.PositionY - droplet.SizeY / 2]);
                    points.push([droplet.PositionX + droplet.SizeX / 2, droplet.PositionY - droplet.SizeY / 2]);
                }
                if (right == null) {
                    points.push([droplet.PositionX + droplet.SizeX / 2, droplet.PositionY - droplet.SizeY / 2]);
                    points.push([droplet.PositionX + droplet.SizeX / 2, droplet.PositionY + droplet.SizeY / 2]);
                }
                if (bottom == null) {
                    points.push([droplet.PositionX + droplet.SizeX / 2, droplet.PositionY + droplet.SizeY / 2]);
                    points.push([droplet.PositionX - droplet.SizeX / 2, droplet.PositionY + droplet.SizeY / 2]);
                }
                if (left == null) {
                    points.push([droplet.PositionX - droplet.SizeX / 2, droplet.PositionY + droplet.SizeY / 2]);
                    points.push([droplet.PositionX - droplet.SizeX / 2, droplet.PositionY - droplet.SizeY / 2]);
                }
            }


            let current_point = points[0];
            let points_to_draw = [current_point];

            


            /*var sorted_points = points.sort(function (a, b) {
                if (a[0] == b[0]) {
                    return a[1] - b[1];
                }
                return a[0] - b[0];
            });
            
            
            var current_point = sorted_points[0];
            var points_to_draw = [current_point];

            // Find all ascending X values with same Y
            for (let j in sorted_points) {
                if (sorted_points[j][0] > current_point[0] && sorted_points[j][1] == current_point[1] &&
                    JSON.stringify(points_to_draw).indexOf(JSON.stringify(sorted_points[j])) == -1) {

                    points_to_draw.push(sorted_points[j]);
                }
            }

            current_point = points_to_draw[points_to_draw.length - 1];

            // Find all ascending Y values with same X
            for (let j = sorted_points.length - 1; j >= 0; j--) {
                if (sorted_points[j][0] == current_point[0] && sorted_points[j][1] > current_point[1] &&
                    JSON.stringify(points_to_draw).indexOf(JSON.stringify(sorted_points[j])) == -1) {

                    points_to_draw.push(sorted_points[j]);
                }
            }

            // Find all 

            console.log("psort", sorted_points);
            console.log("pdraw", points_to_draw);
            
            //for (let a = 0; a < points.length - 1; a++) {
             //   p.line(points[a][0], points[a][1], points[a+1][0], points[a+1][1]);
            //}
            
            for (let a = 0; a < points.length; a++) {
                p.fill("black");
                p.ellipse(points[a][0], points[a][1], 2, 2);
            }
        }*/
    }


    /* Initialize board values */
    function init_board(sizeX, sizeY) {
        p.resizeCanvas(sizeX + 1, sizeY);

        layer_electrode_id = p.createGraphics(sizeX + 1, sizeY);


        for (let layer in layer_manager.layers) {
            if (layer_manager.layers[layer].hasOwnProperty("layer")) {
                layer_manager.layers[layer].layer = p.createGraphics(sizeX + 1, sizeY);
            }
        }
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            debug_electrode_text(layer_manager.layers.debug_electrode_text.layer, electrode);
        }

        layer_electrode_shape = p.createGraphics(sizeX + 1, sizeY);
        layer_electrode_id.clear();
        layer_electrode_id.clear();

        /*for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            debug_electrode_text(layer_electrode_id, electrode);
        }*/

        draw_electrodes_shapes();
    }
    gui_broker.init_board = init_board; // Attach the function to the GUI broker.


    /**
     * Draws the selected element
     * @param {any} layer
     * @param {any} element
     */
    function draw_selected_element(layer, element) {
        layer.clear();
        if (element == null) { return; }

        
        if (element.Status != "undefined") {

            // Electrode
            for (let i = 0; i < gui_broker.electrodes.length; i++) {
                let electrode = gui_broker.electrodes[i];

                if (electrode != element) { continue; }

                layer.noFill();
                layer.stroke("blue");
                layer.strokeWeight(3);

                // Check the electrode shape
                if (electrode.Shape == 1) {
                    layer.beginShape();
                    for (let i = 0; i < electrode.Corners.length; i++) {
                        layer.vertex(electrode.PositionX + electrode.Corners[i][0] + 0.5, electrode.PositionY + electrode.Corners[i][1] + 0.5);
                    }
                    layer.endShape(layer.CLOSE);
                } else {
                    layer.rect(electrode.PositionX, electrode.PositionY, electrode.SizeX, electrode.SizeY);
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

            if (electrode.Status == 0) { continue; }
            p.fill("red");
            
            // Check the electrode shape
            if (electrode.Shape == 1) {
                p.beginShape();
                for (let i = 0; i < electrode.Corners.length; i++) {
                    p.vertex(electrode.PositionX + electrode.Corners[i][0] + 0.5, electrode.PositionY + electrode.Corners[i][1] + 0.5);
                }
                p.endShape(p.CLOSE);
            } else {
                p.rect(electrode.PositionX, electrode.PositionY, electrode.SizeX, electrode.SizeY);
            }
        }
    }

    /* Call to draw electrode shapes */
    function draw_electrodes_shapes() {
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];

            layer_electrode_shape.stroke("black");
            layer_electrode_shape.fill("white");
            //if (electrode.Status != 0) { layer_electrode_shape.fill("red"); }

            // Check the electrode shape
            if (electrode.Shape == 1) {
                draw_polygon_electrode_shapes(electrode.PositionX, electrode.PositionY, electrode.Corners);
            } else {
                layer_electrode_shape.rect(electrode.PositionX, electrode.PositionY, electrode.SizeX, electrode.SizeY);
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
            p.fill(droplet.Color);
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
            //anim_move(droplet, i);
        }
    }

    /* Position (movement) animation */
    function anim_move(droplet, i) {

        p.fill(droplet.Color);

        if (droplet_info.old.length == 0 || typeof droplet_info.old[i] == "undefined") {
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
        } else {
            let d1x = p.lerp(droplet_info.old[i].PositionX, droplet_info.new[i].PositionX, amount);
            let d1y = p.lerp(droplet_info.old[i].PositionY, droplet_info.new[i].PositionY, amount);

            p.ellipse(d1x, d1y, droplet.SizeX, droplet.SizeY);
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

        if (electrode.Shape == 1) {
            layer.fill(0, 255, 0);
            let corner_sum_x = 0;
            let corner_sum_y = 0;

            
            for (let i = 0; i < electrode.Corners.length; i++) {
                corner_sum_x = parseInt(corner_sum_x) + (parseInt(electrode.PositionX) + parseInt(electrode.Corners[i][0]));
                corner_sum_y = parseInt(corner_sum_y) + (parseInt(electrode.PositionY) + parseInt(electrode.Corners[i][1]));
            }

            pos_x = (corner_sum_x) / (electrode.Corners.length) - layer.textWidth(electrode.ID1) / 2;
            pos_y = (corner_sum_y) / (electrode.Corners.length) + layer.textAscent(electrode.ID1) / 2;
            
        } else {
            layer.fill(255, 200, 0);
            pos_x = electrode.PositionX + electrode.SizeX / 2 - layer.textWidth(electrode.ID1)/2;
            pos_y = electrode.PositionY + electrode.SizeY / 2 + layer.textAscent(electrode.ID1)/2;
        }

        layer.textSize(6);
        layer.text(electrode.ID1, pos_x, pos_y);
    }

};