# WebSocket API - Documentation
### Port: `11398`
This app provied a WebSocket server API that allows you to listen to in-game events in real-time.

The event data will be sent as a json object string.

## Possible Event Objects
```json
// Sent when you first connect to the WebSocket, this event contains every other event captured by the Save Manager in the "Args" property.
"Type": "CONNECTED",
"DisplayName": "Kittenji", // Your VRChat display name
"UserID": "usr_7ac745b8-e50e-4c9c-95e5-8e7e3bcde682", // Your VRChat user id
"Args": [
    // All the buffered events will be contained here
]
```

```json
// The STATS event is sent when a stat value is changed. This object contains the name of the Stat in question and the value corresponding to it.
"Type": "STATS",
"Name": "Survivals", // The name of the stat changed.
"Value": 170 // Value can be of type int, float, bool or string.
```

```json
// Sent when the terror is set / revealed or hidden.
"Type": "TERRORS",
"Command": 0, // (0:Set) (2:Unkown) (1:Revealed) (4:Undecided) (255:Reset)
"Names": [
    "Akumii-kari" // An array with the name of each individual terror
],
"DisplayName": "Akumii-kari", // The display name of all the Terrors combined
"DisplayColor": 3808028 // (UINT) The display color of the Terror(s) the way it appears on the HUD in decimal value. (Supports Bloodbaths and Midnights)
```

```json
// Sent when a new round starts, or when a round is over.
"Type": "ROUND_TYPE",
"Command": 1, // (1:Started) (0:Ended)
"Value": 1, // The Round Type int value, the same as OSC.
"Name": "Classic", // Raw Display Name.
"DisplayName": "Classic", // Localized Display Name
"DisplayColor": 16777215 // (UINT) The display color of the round in decimal value.
```

```json
// Sent when the current map/location is changed.
"Type": "LOCATION",
"Command": 1, // (1:Set) (0:Reset)
"Name": "Hell", // The name of the map
"Creator": "DarkGrey", // Name of the map Creator
"Origin": "" // The map origin, usually the game/media it originates from.
```

```json
// Sent when an item is grabbed or dropped.
"Type": "ITEM",
"Command": 1, // (1:Grab) (0:Drop)
"Name": "Torch of Obsession",
"ID": 69 // The ID of the item in question, should be the same as the OSC value.
```

```json
// Sent when user joins a new instance, this event is also sent when you first connect to the websocket.
"Type": "INSTANCE",
"Value": "wrld_00000000-0000-0000-0000-000000000000:000000~private(usr_00000000-0000-0000-0000-000000000000)~region(use)"
```

```json
// Sent when an round is active state changes. Usually when it starts or ends.
"Type": "ROUND_ACTIVE",
"Value": true // BOOL
```

```json
// Sent when the user is Alive or Dead on the round.
"Type": "ALIVE",
"Value": true // Set to false when the user dies in a round. Should be set to true on intermission.
```

```json
// Sent when the user gets reborn using Maxwell.
"Type": "REBORN",
"Value": true // Set to false when the user dies or the round is over.
```

```json
// Sent when the user opts in or out of the game. Usually when the enter the lobby or respawn.
"Type": "OPTED_IN",
"Value": true // Set to false if the user respawns, true if they walk into the lobby.
```

```json
// Sent when the user gets posessed.
"TYPE": "IS_SABOTEUR",
"Value": true // Set back to false when the round is over.
```

```json
// Sent when a page is collected on 8 Pages.
"TYPE": "PAGE_COUNT",
"Value": 4 // The amount of pages collected.
```

```json
// Sent when the user takes some damage.
"TYPE": "DAMAGED",
"Value": 40 // The amount of damage taken.
```

```json
// Sent when a player in the round dies.
"Type": "DEATH",
"Name": "Kittenji",
"Message": "Kittenji spontaneously combusted.",
"IsLocal": true
```

```json
// Sent when a player joins the instance.
"TYPE": "PLAYER_JOIN",
"Value": "Kittenji" // The name of the player that joined.
```

```json
// Sent when a player leaves the instance.
"TYPE": "PLAYER_LEAVE",
"Value": "Kittenji" // The name of the player that left, coward.
```

```json
// Sent when a save code is created.
// This event is not buffered and will not be included in the CONNECTED event.
"TYPE": "SAVED",
"Value": "GENERATED_SAVE_CODE_VALUE"
```