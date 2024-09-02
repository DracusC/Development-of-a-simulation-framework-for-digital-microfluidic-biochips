import asyncio
import websockets
import json
import msvcrt
import threading

# Initialize the starting action
action = {
    "Type": "action",
    "Data": {
        "action": {
            "actionName": "electrode",
            "actionOnID": 100,  # Start with ID 100
            "actionChange": 1
        },
        "time": 0.5
    }
}

actionClear = {
    "Type": "action",
    "Data": {
        "action": {
            "actionName": "electrode",
            "actionOnID": 100,  # Start with ID 100
            "actionChange": 0
        },
        "time": 1.5
    }
}


sensorRequest = {
    "Type": "sensor_request",
    "Data": {
        "ID": 725
    }
}

# List to keep track of connected clients
connected_clients = set()

def get_key():
    """Get a single keypress from the user and return it as bytes."""
    return msvcrt.getch()

def update_action_id(key):
    """Update the actionOnID based on the arrow key pressed."""
    global action
    current_id = action["Data"]["action"]["actionOnID"]

    # Handle arrow keys (which are two bytes in msvcrt)
    if key == b'\xe0':  # First byte of special keys
        key = msvcrt.getch()  # Read second byte
        if key == b'H':  # Arrow Up
            if current_id > 32:
                action["Data"]["action"]["actionOnID"] -= 32
        elif key == b'P':  # Arrow Down
            if current_id <= 512:
                action["Data"]["action"]["actionOnID"] += 32
        elif key == b'M':  # Arrow Right
            if current_id % 32 != 0:
                action["Data"]["action"]["actionOnID"] += 1
        elif key == b'K':  # Arrow Left
            if current_id % 32 != 1:
                action["Data"]["action"]["actionOnID"] -= 1

    # Handle quit case
    elif key == b'q':  # Quit if 'q' is pressed
        return False

    return True

def handle_keypresses(loop):
    """Thread function to handle keypresses."""
    while True:
        key = get_key()
        if not update_action_id(key):
            break
        
        # Send a message to all connected clients
        if key != b'\r':  # Don't send action on Enter key
            asyncio.run_coroutine_threadsafe(send_action_to_clients(), loop)
            print("Sending action")
        elif key == b'\r':  # Enter key
            asyncio.run_coroutine_threadsafe(send_sensor_request_to_clients(), loop)
            print("Enter key pressed, sending sensor request.")

async def echo(websocket, path):
    # Register the client
    connected_clients.add(websocket)
    try:
        async for message in websocket:
            print(f"Received message: {message}")
    except websockets.exceptions.ConnectionClosed as e:
        print(f"Connection closed: {e}")
    finally:
        # Unregister the client
        connected_clients.remove(websocket)

async def send_action_to_clients():
    for client in connected_clients:
        if client.open:
            json_action = json.dumps(action)
            print(f"Sending action: {json_action}")
            await client.send(json_action)

            #clear
            actionClear["Data"]["action"]["actionOnID"] = action["Data"]["action"]["actionOnID"]

            json_actionClear = json.dumps(actionClear)
            print(f"Sending action: {json_actionClear}")
            await client.send(json_actionClear)

async def send_sensor_request_to_clients():
    for client in connected_clients:
        if client.open:
            json_request = json.dumps(sensorRequest)
            print(f"Sending sensor request: {sensorRequest}")
            await client.send(json_request)

async def main():
    # Start the WebSocket server
    server = await websockets.serve(echo, "localhost", 8765)
    print("WebSocket server started on ws://localhost:8765")

    # Get the main event loop
    loop = asyncio.get_running_loop()

    # Start a thread to handle keypresses and pass the main loop to it
    keypress_thread = threading.Thread(target=handle_keypresses, args=(loop,), daemon=True)
    keypress_thread.start()

    # Run the WebSocket server indefinitely
    await server.wait_closed()

if __name__ == "__main__":
    asyncio.run(main())
