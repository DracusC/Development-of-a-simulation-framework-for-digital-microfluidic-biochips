/* 
 * Written by Joel A. V. Madsen
 */

// Global function to setup the p5js instance
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

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

    // TEST
    var checkList = document.getElementById('list1');
    checkList.getElementsByClassName('anchor')[0].onclick = function (evt) {
        if (checkList.classList.contains('visible'))
            checkList.classList.remove('visible');
        else
            checkList.classList.add('visible');
    }

    console.log(checkList);
    console.log(document.getElementById("debug1"));
    debug1 = document.getElementById("debug1");
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
        draw_active_electrodes();

        if (debug1 != null && debug1.checked) {
            p.image(layer_electrode_id, 0, 0);
        }

        draw_droplet();
    }





    /* Initialize board values */
    function init_board(sizeX, sizeY) {
        p.resizeCanvas(sizeX + 1, sizeY);

        layer_electrode_id = p.createGraphics(sizeX + 1, sizeY);
        layer_electrode_shape = p.createGraphics(sizeX + 1, sizeY);
        layer_electrode_id.clear();
        layer_electrode_id.clear();

        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            debug_electrode_text(electrode);
        }

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
    function debug_electrode_text(electrode) {
        let pos_x = 0;
        let pos_y = 0;

        if (electrode.Shape == 1) {
            layer_electrode_id.fill(0, 255, 0);
            let corner_sum_x = 0;
            let corner_sum_y = 0;

            
            for (let i = 0; i < electrode.Corners.length; i++) {
                corner_sum_x = parseInt(corner_sum_x) + (parseInt(electrode.PositionX) + parseInt(electrode.Corners[i][0]));
                corner_sum_y = parseInt(corner_sum_y) + (parseInt(electrode.PositionY) + parseInt(electrode.Corners[i][1]));
            }

            pos_x = (corner_sum_x) / (electrode.Corners.length) - layer_electrode_id.textWidth(electrode.ID1) / 2;
            pos_y = (corner_sum_y) / (electrode.Corners.length) + layer_electrode_id.textAscent(electrode.ID1) / 2;
            
        } else {
            layer_electrode_id.fill(255, 200, 0);
            pos_x = electrode.PositionX + electrode.SizeX / 2 - layer_electrode_id.textWidth(electrode.ID1)/2;
            pos_y = electrode.PositionY + electrode.SizeY / 2 + layer_electrode_id.textAscent(electrode.ID1)/2;
        }

        layer_electrode_id.textSize(6);
        layer_electrode_id.text(electrode.ID1, pos_x, pos_y);
    }

};