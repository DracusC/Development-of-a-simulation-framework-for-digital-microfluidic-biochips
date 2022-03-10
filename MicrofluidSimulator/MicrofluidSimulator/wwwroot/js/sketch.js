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

    amount = 0;
}
window.change_play_status = (status) => {
    gui_broker.play_status = !gui_broker.play_status;
};
window.initialize_board = (information) => {
    var JSONinformation = JSON.parse(information);
    console.log(JSONinformation);
    gui_broker.init_board(JSONinformation.sizeX, JSONinformation.sizeY + 1);

    // Get the debug
    debug1 = document.getElementById("debug1");

    gui_controller.showGUI();
    gui_controller.changeBoardName(JSONinformation.platform_name);
    //console.log(gui_controller.getInputNodes());
    console.log("UNDER");
    layer_manager.initialize_layers();
}


let gui_controller = {
    simulatorGUI: document.querySelector("#simulatorGUI"),
    getLayerPanel: () => { return this.simulatorGUI.querySelector("#layerPanel") },
    getInputNodes: () => { return this.simulatorGUI.querySelector("#layerPanel").getElementsByTagName('INPUT'); },
    changeBoardName: (name) => { this.simulatorGUI.querySelector("#simulatorView span").innerHTML = name; },
    showGUI: () => { simulatorGUI.style.visibility = "visible"; }
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
    next_simulator_step: () => {
        DotNet.invokeMethodAsync('MicrofluidSimulator', 'JSSimulatorNextStep');
    },
    init_board: () => {}
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
        canvas = p.createCanvas(1, 1);
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
    }





    /* Initialize board values */
    function init_board(sizeX, sizeY) {
        p.resizeCanvas(sizeX + 1, sizeY);

        layer_electrode_id = p.createGraphics(sizeX + 1, sizeY);

        console.log("check this too", layer_manager.layers.debug_electrode_text);
        /*for (let layer in layer_manager.layers) {
            console.log(layers);//[layer].layer = p.createGraphics(sizeX + 1, sizeY);
        }*/

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

            anim_move(droplet, i);
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


    /*
     * Call to draw ID's of all electrodes (used for debugging)
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