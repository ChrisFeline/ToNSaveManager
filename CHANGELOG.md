# Latest Changelog

- Scripts now have additional features and APIs
  * Implemented 'setInterval' and 'setTimeout'
  * Added audio resampling for the Audio API, audio files with 48khz should work now.
  * Added AudioPlayer.PlayURL  (Play audio from url)
  * Added AudioPlayer.PlayYTDL (Play audio using yt-dlp)
  * Added HTTP.GetString       (Get the content of an url as a string)

- NEW!: Added JavaScript plugins support, you can now extend this tool by creating your own scripts that can also be shared.
  * JavaScript documentation can be found in the Github repo, under Docs/Scripts.md
  * This feature is still on an experimental phase and requires advanced JS understanding. Join the discord linked in the README for feedback, suggestions or requests.
- Updated Objectives list with the new items. (Finally)
- Fixed death event Decay and Cooldown not holding their values in settings.
- Fixed King's Kit item not being detected properly for OSC.
- Fixed ocassional division by zero.
- New launch parameter: --console