/* This object stores information, such as colors and other configurations for the simulator GUI */

let draw_config = {
    droplet: {
        borderColor: "#000000",
        borderWidth: 1
    },

    electrode: {
        backgroundColor: "#FFFFFF",
        borderColor: "#000000",
        borderWidth: 1,
        activeColor: "#FF0000"
    },

    group: {
        borderColor: "#000000",
        borderWidth: 1,
        selectedBorderColor: "#000000",
        selectedBorderWidth: 2
    },

    actuator: {
        backgroundColor: "#FF0000",
        backgroundOpacity: 100, // 0-255
        borderColor: "#FF0000",
        borderWidth: 1
    },

    sensor: {
        backgroundColor: "#1AA7EC",
        backgroundOpacity: 100, // 0-255
        borderColor: "#1AA7EC",
        borderWidth: 1
    },

    bubble: {
        backgroundColor: "none",
        backgroundOpacity: 0,   // 0-255
        borderColor: "#000000",
        borderWidth: 1
    }
}


/* Gold electrode preset */
/*
let draw_config = {
    droplet: {
        borderColor: "#000000",
        borderWidth: 0
    },

    electrode: {
        backgroundColor: "#FFD700",
        borderColor: "#000000",
        borderWidth: 1,
        activeColor: "#FF0000"
    },

    group: {
        borderColor: "#000000",
        borderWidth: 0
    },

    actuator: {
        backgroundColor: "#FF0000",
        backgroundOpacity: 100, // 0-255
        borderColor: "#FF0000",
        borderWidth: 1
    },

    sensor: {
        backgroundColor: "#1AA7EC",
        backgroundOpacity: 100, // 0-255
        borderColor: "#1AA7EC",
        borderWidth: 1
    },

    bubble: {
        backgroundColor: "none",
        backgroundOpacity: 0, // 0-255
        borderColor: "#000000",
        borderWidth: 1
    }
}*/

/* Random preset Color */
/*
let draw_config = {
    droplet: {
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 0
    },

    electrode: {
        backgroundColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 1,
        activeColor: "#" + Math.floor(Math.random() * 16777215).toString(16)
    },

    group: {
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 0
    },

    actuator: {
        backgroundColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        backgroundOpacity: 100, // 0-255
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 1
    },

    sensor: {
        backgroundColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        backgroundOpacity: 100, // 0-255
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 1
    },

    bubble: {
        backgroundColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        backgroundOpacity: 0, // 0-255
        borderColor: "#" + Math.floor(Math.random() * 16777215).toString(16),
        borderWidth: 1
    }
}
*/