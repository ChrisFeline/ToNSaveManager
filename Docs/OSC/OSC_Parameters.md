# General
Parameter Name   | Type    | Description
-----------------|---------|--------------------------
`ToN_RoundType`  | `INT`   | The current round type. See the [**Round Type Values**](OSC_RoundType.md) document for more info.
`ToN_Map`        | `INT`   | The current map of this round.
`ToN_OptedIn`    | `BOOL`  | Is the player opted-in at the lobby

# Events
Parameter Name   | Type    | Description
-----------------|---------|--------------------------
`ToN_IsAlive`    | `BOOL`  | Is the player alive or not. Should be `true` on Intermission.
`ToN_Damaged`    | `INT`  | Set to `true` for a few frames when the player takes damage. The number represents the amount of damage taken on that hit. If the player dies this number will be `255`
`ToN_Pages`      | `INT`   | The amount of pages collected on an **Eight Pages** round. This number can be a value from `0` to `8`
`ToN_ItemStatus` | `BOOL`  | (Experimental) Determines the active status of items like the **Chaos Coil**, **Emerald Coil**, **Corkscrew** and **TBH**.
`ToN_Saboteur`   | `BOOL`  | The player is the killer on a Sabotage round.

# Terrors
Parameter Name   | Type    | Description
-----------------|---------|--------------------------
`ToN_Terror1`    | `INT`   | The current terror index.
`ToN_Terror2`    | `INT`   | The second terror index. (Bloodbath & Midnight)
`ToN_Terror3`    | `INT`   | The third terror index. (Bloodbath & Alternate on Midnight)
`ToN_TPhase1`    | `INT`   | The current terror phase.
`ToN_TPhase2`    | `INT`   | The second terror phase. (Bloodbath)
`ToN_TPhase3`    | `INT`   | The third terror phase. (Bloodbath & Midnight)

# Colors
Parameter Name   | Type    | Description
-----------------|---------|--------------------------
`ToN_ColorH`     | `FLOAT` | HUD Terror Color (HUE)
`ToN_ColorS`     | `FLOAT` | HUD Terror Color (Saturation)
`ToN_ColorV`     | `FLOAT` | HUD Terror Color (Value)

# Encounters
Parameter Name     | Type    | Description
-------------------|---------|--------------------------
`ToN_EncounterHHI` | `BOOL`  | Hungry Home Invader (Stop raiding my refrigerator)
`ToN_EncounterATR` | `BOOL`  | Atrached
`ToN_EncounterWYB` | `BOOL`  | Wild Yet Bloodthirsty Creature
`ToN_EncounterGLO` | `BOOL`  | Glorbo
`ToN_EncounterEPF` | `BOOL`  | Shadow Evil Purple Foxy