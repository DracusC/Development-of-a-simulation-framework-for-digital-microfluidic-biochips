/* 
 * Written by Joel A. V. Madsen
 */


/* 
 * Declaration of script "global" variables.
 * These variables are used by both the global functions but also by the p5js script.
 */
let simulator_droplets = [];
let simulator_electrodes = [];
let amount = 0;     // Used to interpolate between droplet positions.


// TODO: Refrator into a layer array, that will automatically create them
//let layer_electrode_id;
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
        console.log(canvas.position());

        // Used to get sharper edges in sketch
        p.pixelDensity(4);

        //p.frameRate(30);

        // Create layers
        //layer_electrode_id = p.createGraphics(1, 1);
        layer_electrode_shape = p.createGraphics(1, 1);
        console.log("setup");

        //p.frameRate(10);
        step = 0.05;

        let button = p.select("#edit_button");
        information_panel_manager.edit_btn = button.elt;
        button.mousePressed(() => {
            information_panel_manager.onEdit();
            //console.log("HI FROM SKETCH");
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
            //gui_broker.next_simulator_step(); // Calls for the next "step" of the simulation
        }

        /* Draw calls */
        //draw_electrodes();
        p.image(layer_electrode_shape, 0, 0);



        if (layer_manager.layers.draw_active_electrodes.checkbox.checked) { draw_active_electrodes(); }

        // Draw all direct layers
        layer_manager.draw_layers(p);

        if (layer_manager.layers.draw_droplets.checkbox.checked) { draw_droplet(); }

        if (layer_manager.layers.draw_selected_element.checkbox.checked) { draw_selected_element(layer_manager.layers.draw_selected_element.layer, information_panel_manager.selected_element); }

        if (layer_manager.layers.draw_droplet_groups.checkbox.checked) { draw_grouped_droplets(); }

        if (gui_broker.play_status) {
            gui_broker.next_simulator_step();
        }
    }

    /** Handle single mouse clicks */
    function onMouseClicked() {

        // Handle click on droplet group
        if (layer_manager.layers.draw_droplet_groups.checkbox.checked) {
            for (let i in gui_broker.droplet_groups) {
                if (polygon_contains(gui_broker.droplet_groups[i].vertices, p.mouseX, p.mouseY)) {
                    information_panel_manager.selected_element = information_panel_manager.information_filter("Group", i, gui_broker.droplet_groups[i]);
                    information_panel_manager.information_element = information_panel_manager.information_filter("Group", i, gui_broker.droplet_groups[i]);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Group", i, gui_broker.droplet_groups[i]));
                    return;
                }
            }
        }

        // Handle click on droplet
        if (layer_manager.layers.draw_droplets.checkbox.checked) {
            for (let i in gui_broker.droplets) {
                let droplet = gui_broker.droplets[i];

                // Check mouse over droplet
                if (p.dist(p.mouseX, p.mouseY, droplet.PositionX, droplet.PositionY) < droplet.SizeX / 2) {
                    information_panel_manager.selected_element = droplet;
                    information_panel_manager.information_element = information_panel_manager.information_filter("Droplet", droplet);
                    information_panel_manager.draw_information(information_panel_manager.information_filter("Droplet", droplet));
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


    function droplet_group_information_filter(id, group) {
        let group_info = {
            Group_ID: id,
            Substance_name: group[0].Substance_name,
            Color: group[0].Color,
            Temperature: 0,
            Volume: 0,
            Droplets: []
        };
        //console.log(group);

        // WIP
        for (let i = 0; i < group.length; i++) {
            //console.log(group[i]);
            let droplet = group[i];
            group_info.Droplets.push(droplet.ID1);
            for (let key in droplet) {
                if (key == "Volume") { group_info.Volume += droplet[key] }
                if (key == "Temperature") { group_info.Temperature += droplet[key] }
            }
        }

        group_info.Temperature = group_info.Temperature / group.length;

        return group_info;
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
            let check_size = gui_broker.electrodes[0].SizeX / 2;
            let current_droplet = gui_broker.droplet_groups[i][0];
            let current_droplet_point = [current_droplet.PositionX - check_size, current_droplet.PositionY - check_size];

            //let draw_size = current_droplet.SizeX / 2;
            let droplet_draw_point = [current_droplet.PositionX - current_droplet.SizeX / 2, current_droplet.PositionY - current_droplet.SizeX / 2];

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
                    if (other_droplet.PositionX == current_droplet_point[0] + check_size && other_droplet.PositionY == current_droplet_point[1] - check_size) {
                        top_right = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] - check_size && other_droplet.PositionY == current_droplet_point[1] + check_size) {
                        bottom_left = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] - check_size && other_droplet.PositionY == current_droplet_point[1] - check_size) {
                        top_left = other_droplet;
                    } else if (other_droplet.PositionX == current_droplet_point[0] + check_size && other_droplet.PositionY == current_droplet_point[1] + check_size) {
                        bottom_right = other_droplet;
                    }
                }

                if (top_right == null && bottom_right != null) {
                    points_to_draw.push(droplet_draw_point);

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([bottom_right.PositionX - bottom_right.SizeX / 2,
                            bottom_right.PositionY - bottom_right.SizeY / 2])) == -1) {
                        points_to_draw.push([bottom_right.PositionX - bottom_right.SizeX / 2, bottom_right.PositionY - bottom_right.SizeY / 2]);
                    }

                    droplet_draw_point = [bottom_right.PositionX + bottom_right.SizeX / 2, bottom_right.PositionY - bottom_right.SizeY / 2];
                    current_droplet_point = [current_droplet_point[0] + check_size * 2, current_droplet_point[1]];

                } else if (bottom_right == null && bottom_left != null) {
                    points_to_draw.push(droplet_draw_point);

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([bottom_left.PositionX + bottom_left.SizeX / 2,
                            bottom_left.PositionY - bottom_left.SizeY / 2])) == -1) {
                        points_to_draw.push([bottom_left.PositionX + bottom_left.SizeX / 2, bottom_left.PositionY - bottom_left.SizeY / 2]);
                    }

                    droplet_draw_point = [bottom_left.PositionX + bottom_left.SizeX / 2, bottom_left.PositionY + bottom_left.SizeY / 2];
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] + check_size * 2];
                    
                } else if (bottom_left == null && top_left != null) {
                    points_to_draw.push(droplet_draw_point);

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([top_left.PositionX + top_left.SizeX / 2,
                            top_left.PositionY + top_left.SizeY / 2])) == -1) {
                        points_to_draw.push([top_left.PositionX + top_left.SizeX / 2, top_left.PositionY + top_left.SizeY / 2]);
                    }

                    droplet_draw_point = [top_left.PositionX - top_left.SizeX / 2, top_left.PositionY + top_left.SizeY / 2];
                    current_droplet_point = [current_droplet_point[0] - check_size * 2, current_droplet_point[1]];

                } else if (top_left == null && top_right != null) {
                    points_to_draw.push(droplet_draw_point);

                    if (JSON.stringify(points_to_draw).indexOf(
                        JSON.stringify([top_right.PositionX - top_right.SizeX / 2,
                            top_right.PositionY + top_right.SizeY / 2])) == -1) {
                        points_to_draw.push([top_right.PositionX - top_right.SizeX / 2, top_right.PositionY + top_right.SizeY / 2]);
                    }

                    droplet_draw_point = [top_right.PositionX - top_right.SizeX / 2, top_right.PositionY - top_right.SizeY / 2];
                    current_droplet_point = [current_droplet_point[0], current_droplet_point[1] - check_size * 2];
                }
            }

            p.fill(current_droplet.Color);

            let points_vector = [];
            for (let j = 0; j < points_to_draw.length; j++) {
                points_vector.push(p.createVector(p.round(points_to_draw[j][0],2), p.round(points_to_draw[j][1],2)));
            }

            //console.log(points_vector);
            gui_broker.droplet_groups[current_droplet.Group].vertices = points_to_draw;

            drawRounded(p, points_vector, 50);
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

        layer_electrode_shape = p.createGraphics(sizeX + 1, sizeY);
        //layer_electrode_id.clear();
        //layer_electrode_id.clear();

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