# Scripts - Documentation
> (Under Construction)
> These APIs can change at any moment during beta.

> Undocumented Global variable: `TON` contains a lot of in-game related values.

- To enable the use of scripts, simply create a folder called `scripts` under the same folder as the executable.
- Create a `.js` file inside this folder and have fun.

## Logging
> There's only 3 available logging functions since I am using a custom engine.
```js
console.log("MEOW!");
console.error("OUCH!");
console.warn("OOF!");
```

## Sending OSC Parameters
```js
OSC.SendInt(parameterName, value);
OSC.SendFloat(parameterName, value);
OSC.SendBool(parameterName, value);
OSC.SendAvatar(avatarId);
OSC.SendChatbox(chatboxMessage, direct, complete);
```

## Listening to events
```js
// Called when an event happens, these are the same events as the WebSocketAPI events.
function OnEvent(args) {
	
}

// Called for each tick on the log parser, this is under a consistent timer, which can be modified under settings.
function OnTick() {
	
}

// Called when the tool has started and is ready, you should mostly do things after this event, or whatever... I'm not a fun cop.
function OnReady() {
	
}

// Called every time a line is read on the VRChat logs
function OnLine(line) {
	
}

// It is required to export these functions for them to be detected and registered.
export { OnEvent, OnTick, OnReady, OnLine }
```

## Storing Data
> Try not to abuse this too much, it writes to a json file with the same name as your JavaScript file.
```
storage.set("best_number_ever", 42);
storage.get("best_number_ever") // returns 42, null if value is not set or invalid
storage.del("best_number_ever") // deletes the value from storage
```