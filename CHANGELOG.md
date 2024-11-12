# Latest Changelog

- Fixed player names showing their GUID on the tooltips after VRChat's latest update.
	- In addition, the GUID is sent on the WebSocket API too.
- OSC Damage event got an overhaul, you can use JavaScript to manipulate the value of this parameter so it can be used as a float in other "things".
	- For example, if you set "Damage / 100" as the code and the damage value is '50', this will be evaluated to '0.5' before sending the value.
- Added new WebSocket API event: MASTER_CHANGE
- Date list now sorts using Universal Time. This prevents unwanted behaviour when 'Daylight Saving' event occurs.