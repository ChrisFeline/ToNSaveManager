# Templates - Documentation
Templates are used for content replacement for features like the Chatbox, Rich Presence, File Info writing etc.

These templates can be modified to include keywords that would be replaced with a specific stat value.

These keywords can be inserted by wrapping the key with `{` and `}`.

> Example:
> ```
> My survivals on this lobby: {LobbySurvivals}
> ```
> will be replaced as
> ```
> My survivals on this lobby: 22
> ```

You can also use JavaScript for more somehow advanced text replacement. Just use the `<js>` tag.

> Example:
> ```
> <js>RoundStunsAll < 1 ? 'No Stuns Last Round :(' : 'Current Round Stuns : ' + {RoundStunsAll}</js>
> ```
> will be replaced as
> ```
> > if RoundStunsAll is 1 or higher
> Current Round Stuns : 64
>
> > if RoundStunsAll is less than 1
> No Stuns Last Round :(
> ```

You can combine these two replacement methods.
> Example:
> ```
> {Stuns} stuns - Random JS number <js>Math.random()</js>
> ```
> will be replaced as
> ```
> 44 stuns - Random JS number 0.11645544468
> ```

# Available Stats Keys
These keys are available to be included on templates.

| Name | Key | Description |
| - | - | - |
| Deaths | `{Deaths}` | Amount of times you have died of unnatural causes. |
| Survivals | `{Survivals}` | Amount of times you have miraculously survived a round. |
| Damage Taken | `{DamageTaken}` | Total damage taken. |
| Top Stuns | `{TopStuns}` | Highest amount of times you have stunned a Terror in a round. |
| Top Stuns All | `{TopStunsAll}` | Highest amount of times you and other players have stunned a Terror in a round. |
| Stuns | `{Stuns}` | Amount of times you have stunned a Terror. |
| Stuns All | `{StunsAll}` | Amount of times you and other players have stunned a Terror. |
| Players Online | `{PlayersOnline}` | Represents the amount of players in the instance. |
| Display Name | `{DisplayName}` | Your display name in VRChat. |
| Discord Name | `{DiscordName}` | Your display name in Discord.<br>Rich Presence must be enabled. |
| Instance URL | `{InstanceURL}` | The current full URL of the instance you joined. |
| Lobby Rounds | `{LobbyRounds}` | Total amount of rounds played on this lobby. This is the same as (LobbyDeaths + LobbySurvivals = LobbyRounds) |
| Lobby Deaths | `{LobbyDeaths}` | Amount of times you have died of unnatural causes on this lobby. |
| Lobby Survivals | `{LobbySurvivals}` | Amount of times you have miraculously survived a round on this lobby. |
| Lobby Damage Taken | `{LobbyDamageTaken}` | Total damage taken on this lobby. |
| Lobby Top Stuns | `{LobbyTopStuns}` | Highest amount of times you have stunned a Terror in a round on this lobby. |
| Lobby Top Stuns All | `{LobbyTopStunsAll}` | Highest amount of times you and other players have stunned a Terror in a round on this lobby. |
| Lobby Stuns | `{LobbyStuns}` | Amount of times you have stunned a Terror on this lobby. |
| Lobby Stuns All | `{LobbyStunsAll}` | Amount of times you and other players have stunned a Terror on this lobby. |
| Terror Name | `{TerrorName}` | Displays the Monster's name that is currently active on the round. |
| Round Type | `{RoundType}` | Name of the current round type. |
| Map Name | `{MapName}` | Name of the Map this round is currently taking place on. |
| Map Creator | `{MapCreator}` | Name of the map creator. |
| Map Origin | `{MapOrigin}` | Displays where this map originates from. |
| Item Name | `{ItemName}` | The name of the current item held. |
| Is Alive | `{IsAlive}` | True if the player is currently alive on a round. |
| Is Killer | `{IsKiller}` | True if the player is currently the killer on a Sabotage round. |
| Is Started | `{IsStarted}` | True if a round is started and ongoing. |
| Round INT | `{RoundInt}` | Represents the int value of the current round type. |
| Map INT | `{MapInt}` | Represents the int value of the current map. |
| Page Count | `{PageCount}` | The amount of pages collected during an 8 Pages round. |
| Round Stuns | `{RoundStuns}` | Amount of times you have stunned a Terror in the round. |
| Round Stuns All | `{RoundStunsAll}` | Amount of times you and other players have stunned a Terror in the round. |