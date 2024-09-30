# Latest Changelog

## Important Changes
- Every country or region should be able to use this app now.
	- It now runs on Invariant Culture. This fixes the app not being able to parse the logs in some countries.
	- This means if your friends from 'Tailand' couldn't use the app, they might be able to now lol
- Updated for new ToN Update. Yes, it's time! Create a new instance!

## Additions
- Added WebSocket API. You can connect to the save manager and receive in-game events via a websocket to facilitate the creation of visual tools like interactive stream overlays or trackers.
	- Additionally, this feature adds support for the live tracker at `tontrack.me`
- Added stats window. Here you can track stats like rounds won/lost, killer stuns, etc...
- Added Discord Rich Presence support, yes... you can show you're friends how addicted you're to the game.
- Added OpenRGB support, you can color your keyboard and other peripherals based on in-game events. (Like round type or terror color)
- Added chatbox stats display. (Only shown on Intermissions)
	- You can display your stats in the chatbox, but the chatbox will be hidden on purpose when participating on a round. Chatbox will be displayed again after death or end of round.
	- You can customize the chatbox message template.
	- For example: "Stuns - `{LobbyStuns}`" will be displayed as "Stuns - 3"
- Added 'Round Info to File' in settings.
	- This feature lets you create templates that will write to a text file, this file can be read by streaming tools like OBS to display round info about your suffering to your audience.
- Added more keyboard navigation. (Suggested by YoBro)
- Save code will now display the Map where you survived.

## Fixes & Improvements
- Log now parses from room join every time you launch the app. This fixes some terror names not being displayed when the app is opened in the middle of a run.
- Fixed save code delete warning message being the same as log history delete message.
- Rename & Import context menu buttons are disabled on entries that are not custom collections or favorites.
- Fixed clipboard clearing it's content after closing the app.
- App should detect your native UI language properly at first launch now.

### OSC Changes
* Please open README.md to see more OSC specifications.
- Some RoundType values has changed, please revisit your avatar and the documentation for more info.
- Added an OSC emulator window that allows you to emulate and test ToN encounters without actually being in the world. This should help avatar creators test their parameters and animations.
	- To open the emulator, launch the app with --emulator as a console argument.
	- This will be locked for at least a week to prevent spoilers.
- Added parameters for the HUD terror color, allowing you to colorize your avatar with HSV.
- Eight Pages is now fully supported.
- Supports alternates on Fog rounds.
- Added parameters for special encounters.
- Added parameters for some terror phases. (Faker & Bliss)
- 'RUN' round type is now properly detected.

### Localization
- Added German localization (Provided by @sageyx2002)
- Added Simplified Chinese localization (Provided by @Fallen-ice)