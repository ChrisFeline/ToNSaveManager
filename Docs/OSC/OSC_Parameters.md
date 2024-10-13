# General
Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_RoundType`  | `INT`   | The current round type. See the [**Round Type Values**](OSC_RoundType.md) document for more info.
`ToN_Map`        | `INT`   | The current map of this round.
`ToN_Item`       | `INT`   | The current item you are holding. See the [**Item Values**](OSC_Items.md) document for more info.
`ToN_OptedIn`    | `BOOL`  | Is the player opted-in at the lobby

# Events
Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_IsAlive`    | `BOOL`  | Is the player alive or not. Should be `true` on Intermission.
`ToN_IsStarted`  | `BOOL`  | `True` if the round has started and ongoing.
`ToN_Damaged`    | `INT`   | Set to `true` for a few frames when the player takes damage. The number represents the amount of damage taken on that hit. If the player dies this number will be `255`
`ToN_Pages`      | `INT`   | The amount of pages collected on an **Eight Pages** round. This number can be a value from `0` to `8`
`ToN_ItemStatus` | `BOOL`  | (Experimental) Determines the active status of items like the **Chaos Coil**, **Emerald Coil**, **Corkscrew** and **TBH**.
`ToN_Saboteur`   | `BOOL`  | The player is the killer on a Sabotage round.
`ToN_DeathID`    | `INT`   | ...

# Terrors
### Terrors in Round
> These values represent the current index or id of the terror in the current round.<br>
> Since these ID's are index based, the value will be **255** when no terror has spawned.<br>

Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_Terror1`    | `INT`   | The current terror index.
`ToN_Terror2`    | `INT`   | The second terror index. (Bloodbath & Midnight)
`ToN_Terror3`    | `INT`   | The third terror index. (Bloodbath & Midnight)<br>This last one will be an Alternate ID on Midnight.

> Please note that there's some special cases where these values change a bit.
> **Monarch** will use the `Terror3` as identifier and `Terror1` and `Terror2` will be set to **255**.

> **8 PAGES** uses these parameters a bit differently.<br>
> - `Terror1` is the normalized terror ID, meaning it will use the index from the corresponding group.
> - `Terror2` is the group that the `Terror1` belongs to.
>	* `0` - is **Classic**
>	* `1` - is **Alternate**
>	* `2` - is **8 Pages** Original
> - `Terror3` would be the actual index on the **8 PAGES** pool.

### Terrors phase index.
> These values represent the different phases of some terrors.<br>
> For example: **Faker** or **Bliss**.

Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_TPhase1`    | `INT`   | The current terror phase.
`ToN_TPhase2`    | `INT`   | The second terror phase. (Bloodbath)
`ToN_TPhase3`    | `INT`   | The third terror phase. (Bloodbath & Midnight)

# Colors
### HSV - HSL Format (Default)
> If the format is set to **HSV**, these parameters will be sent to **OSC**.<br>
> In **HSL** format, `ToN_ColorV` will change to `ToN_ColorL` instead.

Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_ColorH`     | `FLOAT` | HUD Terror Color (HUE)
`ToN_ColorS`     | `FLOAT` | HUD Terror Color (Saturation)
`ToN_ColorV`<br>`ToN_ColorL`     | `FLOAT` | HUD Terror Color (Value)<br>(Lightness) If format is **HSL**.

### RGB - RGB32 Format
> If the format is set to **RGB**, these parameters will be sent instead of **HSV**.<br>
> In **RGB32** format, these values will be sent as an **Int** (Range 0-255) instead of a **Float**.

Parameter        | Type    | Description
-----------------|---------|--------------------------
`ToN_ColorR`     | `FLOAT` | HUD Terror Color RED channel.
`ToN_ColorG`     | `FLOAT` | HUD Terror Color GREEN channel.
`ToN_ColorB`     | `FLOAT` | HUD Terror Color BLUE channel.

# Encounters
Parameter          | Type    | Description
-------------------|---------|--------------------------
`ToN_EncounterHHI` | `BOOL`  | Hungry Home Invader (Stop raiding my refrigerator)
`ToN_EncounterATR` | `BOOL`  | Atrached
`ToN_EncounterWYB` | `BOOL`  | Wild Yet Bloodthirsty Creature
`ToN_EncounterGLO` | `BOOL`  | Glorbo