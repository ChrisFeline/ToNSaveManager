# Scripts API Documentation
> (Under Construction)
> Some APIs might change and more will be added later on.
> You can request changes or additions by creating an Issue here or posting a request on the Discord server.

- To enable the use of scripts, simply create a folder called `scripts` under the same folder as the executable.
- Create or copy a `.js` file inside this folder and have fun.

## Logging
> There's only 4 available logging functions since I am using a custom engine.
```js
console.log("MEOW!");
console.error("OUCH!");
console.warn("OOF!");
print("RUN!!!");
```
```js
// You can show Dialog Boxes with the 'alert' command.
// ? are optional args
let result = alert(String message, AlertButtons buttons?, AlertIcon icon?);

// Examples:
alert("Hello World");
alert("Hello World", AlertButtons.OKCancel);
alert("Hello World", AlertButtons.OKCancel, AlertIcon.Error);

// You can compare the result with <AlertResult> properties:
if (result == AlertResult.OK) { /* DO SOMETHING */ }

// See the property table below for possible values:
```
| AlertResult | AlertButtons | AlertIcon |
| - | - | - |
| **AlertResult**.None     |                                    | **AlertIcon**.None
| **AlertResult**.OK       | **AlertButtons**.OK                | **AlertIcon**.Hand
| **AlertResult**.Cancel   | **AlertButtons**.OKCancel          | **AlertIcon**.Question
| **AlertResult**.Abort    | **AlertButtons**.AbortRetryIgnore  | **AlertIcon**.Exclamation
| **AlertResult**.Retry    | **AlertButtons**.RetryCancel       | **AlertIcon**.Asterisk
| **AlertResult**.Ignore   |                                    | **AlertIcon**.Stop
| **AlertResult**.Yes      | **AlertButtons**.YesNoCancel       | **AlertIcon**.Error
| **AlertResult**.No       |                                    | **AlertIcon**.Warning
| **AlertResult**.TryAgain | **AlertButtons**.CancelTryContinue | **AlertIcon**.Information
| **AlertResult**.Continue |                                    | 

## Sending OSC Parameters & Values
```js
// General
OSC.Send(string path, object value); 
OSC.SendFloat(string path, float value);
OSC.SendInt(string path, int value);
OSC.SendBool(string path, bool value);
OSC.SendParameter(string name, float/int/bool value);
// Parameters & Chatbox
OSC.SendChatbox(string message, bool direct, bool complete); // Send chatbox text.
// direct:
//     true  - Send the text immediately, bypassing the keyboard.
//     false - send the text to the keyboard.
// complete:
//     true  - Trigger the notification SFX.
//     false - Do not play notification SFX.
OSC.SetChatboxTyping(bool value); // Toggle the typing indicator on or off.
// Axis
OSC.MoveVertical(float value);   // Move forwards (1) or Backwards (-1)
OSC.MoveHorizontal(float value); // Move right (1) or left (-1)
OSC.LookHorizontal(float value); // Look Left and Right.
OSC.UseAxisRight(float value);   // ...
OSC.GrabAxisRight(float value);  // ...
OSC.MoveHoldFB(float value);     // Move a held object forwards (1) and backwards (-1)
OSC.SpinHoldCW(float value);     // Spin a held object Clockwise or Counter-Clockwise.
OSC.SpinHoldUD(float value);     // Spin a held object Up or Down.
OSC.SpinHoldLR(float value);     // Spin a held object Left or Right.
// Buttons
OSC.MoveForward(bool value);     // Move forward while true.
OSC.MoveBackward(bool value);    // Move backwards while true.
OSC.MoveLeft(bool value);        // Move left while true.
OSC.MoveRight(bool value);       // Move right while true.
OSC.LookLeft(bool value);        // Turn to the left while this is 1. Smooth in Desktop, VR will do a snap-turn if Comfort Turning is on.
OSC.LookRight(bool value);       // ...
OSC.Jump(bool value);            // Jump if the world supports it.
OSC.Run(bool value);             // Walk faster if the world supports it.
OSC.ComfortLeft(bool value);     // Snap-Turn to the left - VR Only.
OSC.ComfortRight(bool value);    // Snap-Turn to the right - VR Only.
OSC.GrabRight(bool value);       // Grab the item highlighted by your right hand - VR Only.
OSC.DropRight(bool value);       // Drop the item held in your right hand - VR Only.
OSC.UseRight(bool value);        // Use the item highlighted by your right hand - VR Only.
OSC.GrabLeft(bool value);        // ...
OSC.DropLeft(bool value);        // ...
OSC.UseLeft(bool value);         // ...
OSC.PanicButton(bool value);     // Turn on Safe Mode.
OSC.QuickMenuToggleLeft(bool value);  // Toggle QuickMenu On/Off. Will toggle upon receiving '1' if it's currently '0'. 
OSC.QuickMenuToggleRight(bool value); // ...
OSC.Voice(bool value);           // Toggle Voice
OSC.SetAvatar(string id);        // Changes the user's avatar in VRC (Must on favorites or uploaded)
```
```js
// Subscribing to OSC Events...
OSC.Register("/avatar/parameters/VelocityZ", function(path, values) {
    let value = values[0];
    console.log(value);
});

// You can subscribe to avatar parameters directly.
OSC.RegisterParam("YourCoolToggle", function(path, values) {
    // ...
})
```

## Listening to events
> You can see a list of objects the `OnReady()` event can receive on the [WebSocketAPI DOCS](./WebSocketAPI.md).
```js
// Called when an event happens, these are the same events as the WebSocketAPI events.
function OnEvent(args) {}

// Called for each tick on the log parser, this is under a consistent timer, which can be modified under settings.
function OnTick() {}

// Called when the tool has started and is ready, you should mostly do things after this event, or whatever... I'm not a fun cop.
function OnReady() {}

// Called every time a line is read on the VRChat logs
function OnLine(line) {}

// It is required to export these functions for them to be detected and registered.
export { OnEvent, OnTick, OnReady, OnLine }
```

## Custom WebSocketAPI Events
```js
WS.Enabled; // Bool, true if the WebSocketAPI is enabled by the user
WS.SendEvent("custom_event_name", value);

// The object received by the WebSocket will be structured this way:
{
  "Type": "CUSTOM",
  "Source": "script_name_idk.js",
  "Name": "custom_event_name", // The name of your custom defined event name.
  "Value": 170 // The value you sent on this event, it can also be a JS object.
}
```

## Storing Data
> Try not to abuse this too much, it writes to a json file with the same name as your JavaScript file.
```js
storage.set("best_number_ever", 42);
storage.get("best_number_ever") // returns 42, null if value is not set or invalid
storage.del("best_number_ever") // deletes the value from storage
```

## Game State
> Various game state variables are stored in a global variable defined as `TON`.
### Stats & Lobby Data
```js
// Basic Stats (TON.Stats)
TON.Stats.Rounds       // Total amount of rounds
TON.Stats.Deaths       // Total amount of deaths
TON.Stats.Survivals    // Total amount of survivals
TON.Stats.DamageTaken  // Total damage taken. (Innacurate)
TON.Stats.TopStuns     // Total amount of times player stunned a terror
TON.Stats.TopStunsAll  // Total amount of times all players stunned a terror
```

```js
// Lobby Stats (TON.StatsLobby)
// ... Inherits Basic Stats but only saved per lobby
TON.StatsLobby.PlayersOnline // Player count in the lobby
TON.StatsLobby.IsOptedIn     // TRUE if the player is opted-in and not respawned
TON.StatsLobby.DisplayName   // Player display name in-game
TON.StatsLobby.DiscordName   // Player discord name if using Rich Presence
TON.StatsLobby.InstanceURL   // Player instance location URL
```

```js
// Round Stats (TON.StatsRound)
TON.StatsRound.TerrorName  // Terror name
TON.StatsRound.RoundType   // Round type name
TON.StatsRound.MapName     // ...
TON.StatsRound.MapCreator  // ...
TON.StatsRound.MapOrigin   // ...
TON.StatsRound.ItemName    // Held item name
TON.StatsRound.IsAlive     // Is player currently alive
TON.StatsRound.IsReborn    // Is player saved by Maxwell
TON.StatsRound.IsKiller    // Is player killer on a Sabotage round
TON.StatsRound.IsStarted   // Round has started and ongoing
TON.StatsRound.RoundInt    // Round Type INT value
TON.StatsRound.MapInt      // Map Index INT value.
TON.StatsRound.PageCount   // Number of pages collected on a 8-Pages round
// ... Only 2 are Inherited from Basic Stats
TON.StatsRound.Stuns       // Amount of times player stunned a terror this round
TON.StatsRound.StunsAll    // Amount of times all players stunned a terror this round
```

### Properties
```js
TON.IsEmulated     // Is this round emulated with '--emulator'
TON.IsAlive        // ...
TON.IsReborn       // Is player saved by Maxwell
TON.IsRoundActive  // Is the round started and ongoing?
TON.IsSaboteour    // Is player the killer in a Sabotage round
TON.IsOptedIn      // Is player opted-in and not respawned
TON.PageCount      // Number of pages collected on a 8-Pages round
// Instance Info
TON.PlayerCount    // Player count in the lobby
TON.DisplayName    // Player display name in-game
TON.DiscordName    // Player discord name if using Rich Presence
TON.InstanceURL    // Player instance location URL
```

```js
TON.RoundType; // Current Round Type (INT)
// Possible Values:
  1 // Classic
  2 // Fog
  3 // Punished
  4 // Sabotage
  5 // Cracked
  6 // Bloodbath
  7 // Double_Trouble
  8 // EX
  9 // Ghost
 10 // Unbound
 50 // Midnight
 51 // Alternate
 52 // Fog_Alternate
 53 // Ghost_Alternate
100 // Mystic_Moon
101 // Blood_Moon
102 // Twilight
103 // Solstice
104 // RUN
105 // Eight_Pages
106 // GIGABYTE
107 // Cold_Night
```

```js
TON.RoundColor;   // Display (HUD) color of the current round.
TON.DisplayColor; // Display (HUD) color of the current terror.
```

```js
TON.Location; // Contains data for the current map in round.
TON.Location.Name    // Current map name
TON.Location.ID      // Current map ID (Same as OSC)
TON.Location.Creator // Name of the map creator
TON.Location.Origin  // Name of the map origin
TON.Location.IsEmpty // TRUE if location is empty/not-active.
```

```js
TON.Item; // Constains data for the currently held Item
TON.Item.Name     // The item name
TON.Item.ID       // The item ID (Same as OSC)
TON.Item.Store    // The type of store this item is from
    0 // Enkephalin Shop
    1 // Survival Shop
    2 // Event Shop
    3 // Special Events
    4 // Role Items (Like Contributors)
TON.Item.Points   // Points required to unlock this item. (INT)
TON.Item.IsEmpty  // TRUE if held item is empty/not-active.
```
> [List of item IDs](./OSC/OSC_Items.md)

```js
TON.Terrors;   // An array of terrors on the current round. Empty if none.
TON.Terrors[0] // Each element returns a <TerrorInfo> object.
// <TerrorInfo> properties:
TerrorInfo.IsEmpty;   // Info is empty or invalid for this object instance.
TerrorInfo.Name;      // Processed name for this terror (Includes LVL)
TerrorInfo.Group;     // The group this terror belongs to (INT)
    0 // Terrors
    1 // Alternates
    2 // EightPages
    3 // Unbound
    4 // Moons
    5 // Specials
    6 // Events
TerrorInfo.Phase;     // The current phase of the terror as INT (Like Sonic/Faker)
TerrorInfo.Encounter; // ID of the special encounter if any (Like Atrached)
TerrorInfo.Level;     // Level of this terror when duplicated on Bloodbath round
TerrorInfo.Value;     // Returns a <Terror> object that contains terror data.
// <Terror> properties:
Terror.Name;          // Name of the terror on the index.
Terror.ID;            // Internal ID of the terror.
Terror.CantBB;        // Can't appear in blood-bath. (BOOL)
Terror.Stunnable;     // Can be stunned? (BOOL)
Terror.Group;         // Same as <TerrorInfo>.Group
Terror.Color;         // The color for this terror.
```

## Save Manager Data
> Save Manager data and properties are accessible with the global variable `SM`
```js
SM.LatestSave;            // Returns the latest save entry.
SM.LatestSave.Content     // This is the save code generated in-game.
SM.LatestSave.Note        // Custom note attached to this entry.
SM.LatestSave.TimeStamp   // Date time for this save entry.
SM.LatestSave.PlayerCount // Player count at the time of saving.
SM.LatestSave.Players     // Player names present in the lobby at the time of saving.
SM.LatestSave.RT          // Round Type. Same values as TON.RoundType
SM.LatestSave.TD          // Terror Data, this is a TerrorInfo[] array. Same values as ToN.Terrors
SM.LatestSave.RResult     // Round Result (INT)
    0 // Respawn
    1 // Win
    2 // Leaving
    3 // Lost < never happens, kept for legacy
    4 // Forgot to save lol
    5 // REBORN (Died with Maxwell)

SM.LatestSave.CopyToClipboard(); // Copy the content of this save to the clipboard.
```

## File System
> You are only allowed to locate files inside the scripts folder.
```js
// Reading a file
FS.Read(path); // Returns a string with the file's content
// Writing a file
FS.Write(path, content);  // 'content' must be the string to write to this file.
// Checking if file exists
FS.Exists(path); // Returns 'true' if the file is found.

FS.GetFiles(path); // Returns an array of files inside a folder
FS.GetDirs(path); // Returns an array of directories inside a folder
```
The `path` is resolved relative to the script location.<br>
For example, on a script located at `scripts/cats/meow.js` passing `hello.txt` will resolve this path as `scripts/cats/hello.txt`

## Playing Audio (Experimental)
> Paths follow the same rules as **File System**.
```js
AudioPlayer.Play(path);       // Play audio file at path.
AudioPlayer.Stop();           // Stops the audio player.
AudioPlayer.GetVolume();      // Gets the current volume value.
AudioPlayer.SetVolume(value); // Sets the volume value (Value must be a float from 0.0 to 1.0)

AudioPlayer.IsPlaying;        // 'true' if something is playing.
```

## Text to Speech
```js
TTS.Speak(prompt);    // Speak the prompted string into TTS.
TTS.Stop();           // Stops the TTS from speaking.
TTS.GetVolume();      // Gets the current volume value.
TTS.SetVolume(value); // Sets the volume value (Value must be a int from 0 to 100)

TTS.IsSpeaking;       // 'true' if TTS is still playing the previous prompt.
```

## HTTP Requests
```js
HTTP.GetString(url); // Returns a string with the content of the url (null if an error happens)
```