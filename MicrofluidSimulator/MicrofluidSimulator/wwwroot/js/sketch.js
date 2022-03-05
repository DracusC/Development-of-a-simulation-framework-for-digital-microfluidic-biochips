/* 
 * Written by Joel A. V. Madsen
 */

// Setup the p5js instance
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

// Global methods called by c#
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
}


// Declare global variables
let arr = [];
let d_info = [];


// Store simulator board info
let simulator_droplets = [];
let simulator_electrodes = [];


// gui_broker acts as a connection between the simulator and gui
let gui_broker = {
    gui_status: true, // Ready for a new update (true/false)
    play_status: false,
    droplets: [],
    electrodes: [],
    next_simulator_step: () => {
        DotNet.invokeMethodAsync('MicrofluidSimulator', 'JSSimulatorNextStep');
    },
    init_board: () => {}
};
let droplet_info = {
    old: [],
    new: []
};

window.gui_broker = gui_broker;

let amount = 0;
let debug_layer;

let sketch = function (p) {

    let step;
    p.setup = function () {
        canvas = p.createCanvas(1, 1);
        console.log(canvas.position());

        debug_layer = p.createGraphics(1, 1);
        console.log("setup");

        //p.frameRate(10);
        //arr = [];
        step = 0.05;

        //let slider = p.createSlider(0, 100, 10, 10);

        let button = p.select("#nextStep");
        button.mousePressed(() => { console.log("HI FROM SKETCH"); });

        

        //let checkbox = p.createCheckbox("test", false);
        //let button = p.createButton("Next Step");
        //button.mousePressed(() => { gui_broker.next_simulator_step(); });
    }




    p.draw = function () {
        p.background(240);

        // Check for next step
        if (amount < 1) {
            amount += step; //console.log(step, amount);
        } else if (gui_broker.play_status) {
            gui_broker.next_simulator_step();
        }

        

        // Draw calls
        draw_electrodes();
        p.image(debug_layer, 0, 0);
        draw_droplet();
        
    }


    // Initialize board values
    function init_board(sizeX, sizeY) {
        p.resizeCanvas(sizeX + 1, sizeY);

        debug_layer = p.createGraphics(sizeX + 1, sizeY);
        debug_layer.clear();

        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            debug_electrode_text(electrode);
        }
    }
    gui_broker.init_board = init_board;


    function draw_electrodes() {
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];

            p.stroke("black");
            p.fill("white");
            if (electrode.Status != 0) { p.fill("red"); }

            // Check the electrode shape
            if (electrode.Shape == 1) {
                //console.log(electrode.ID1, "POLYGON!", electrode.Corners);
                draw_polygon_electrodes(electrode.PositionX, electrode.PositionY, electrode.Corners);
            } else {
                p.rect(electrode.PositionX, electrode.PositionY, electrode.SizeX, electrode.SizeY);
            }

            
            //debug_electrode_text(electrode);
            
        }
    }

    function draw_polygon_electrodes(posX, posY, corners) {
        p.beginShape();
        for (let i = 0; i < corners.length; i++) {
            p.vertex(posX + corners[i][0] + 0.5, posY + corners[i][1] + 0.5);
        }
        p.endShape(p.CLOSE);
    }


    function draw_droplet() {

        for (let i = 0; i < gui_broker.droplets.length; i++) {
            let droplet = gui_broker.droplets[i];

            anim_move(droplet, i);
        }
    }

    function anim_move(droplet, i) {

        p.fill(droplet.Color);
        //p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);

        if (droplet_info.old.length == 0) {
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
        } else {
            let d1x = p.lerp(droplet_info.old[i].PositionX, droplet_info.new[i].PositionX, amount);
            let d1y = p.lerp(droplet_info.old[i].PositionY, droplet_info.new[i].PositionY, amount);

            p.ellipse(d1x, d1y, droplet.SizeX, droplet.SizeY);
        }
    }


    function debug_electrode_text(electrode) {
        let pos_x = 0;
        let pos_y = 0;

        if (electrode.Shape == 1) {
            p.fill(255, 0, 0);
            let corner_sum_x = 0;
            let corner_sum_y = 0;

            
            for (let i = 0; i < electrode.Corners.length; i++) {
                corner_sum_x = parseInt(corner_sum_x) + (parseInt(electrode.PositionX) + parseInt(electrode.Corners[i][0]));
                corner_sum_y = parseInt(corner_sum_y) + (parseInt(electrode.PositionY) + parseInt(electrode.Corners[i][1]));
            }

            pos_x = (corner_sum_x) / (electrode.Corners.length) - debug_layer.textWidth(electrode.ID1) / 2;
            pos_y = (corner_sum_y) / (electrode.Corners.length) + debug_layer.textAscent(electrode.ID1) / 2;

            //console.log(electrode.ID1);
            
        } else {
            debug_layer.fill(0, 255, 0);
            pos_x = electrode.PositionX + electrode.SizeX / 2 - debug_layer.textWidth(electrode.ID1)/2;
            pos_y = electrode.PositionY + electrode.SizeY / 2 + debug_layer.textAscent(electrode.ID1)/2;
        }

        debug_layer.textSize(6);
        debug_layer.text(electrode.ID1, pos_x, pos_y);
    }

    function debug_layer() {
        
    }
};