# Open RGB - Documentation

This program has [OpenRGB](https://openrgb.org) implementation.
This document will try to help you understand how to setup the setup for this feature.
We will be working a lot with the `JSON` file format so basic understanding of `JSON` syntax is necessary.


# Enable Implementation
- Setup OpenRGB
  - Download OpenRGB from https://openrgb.org/#downloads
  - Extract it and execute **OpenRGB.exe**
    - If You don't see your devices in the first tab launch the program as   administrator.
  - Navigate to the `SDK Server` tab and click on the `Start Server` button.
- Open the Save Manager
  - Go to `Settings` and toggle on the `OpenRGB Enabled` feature.
  - To edit and configure your setup, click on `(Open JSON Setup)` or open the file `OpenRGB_Setup.json`
    - A highlighted text editor is highly recommended. Like [Notepad++](https://notepad-plus-plus.org/) for a lightweight experience or [VSCode](https://code.visualstudio.com/) as an alternative.
  - Edit the template as you wish. More info about the syntax below.


# Definitions & Syntax
Here you can see the main structure of your JSON file.
> **NOTE:** To see a list of Device IDs and LED indexes you can use, a file will be generated when you first connect to the OpenRGB server named `openrgb_device_keys.json`.

## Device Object
> Append your device object to the `Entries` array.<br>
Set the ID of the device you want to target into the `"DeviceID": #` property.
```json
{
    "DeviceID": 2,
    "Areas": [
        ...
    ]
}
```

## Area Object
> Append your area object to the `Areas` array under your device object<br>
Areas will be used to light up a specific set of LEDs on your device.<br>
You can keep the `"Leds": []` property empty to set the entire device to the group color.
```json
{
    "DarkOnStart": true,
    "FlashOnMidnight": true,
    "Group": 0,
    "Leds": [ 0, 1 ]
}
```

## Area Properties
| Property Name | Type | Description | Example |
| - | - | - | - |
| **Group** | `INT` | The color group to use on this area for LEDs.<br>`0` - Terror Color<br>`1` - Round Color<br>`2` - (Alive / Dead) Color | `Group: 0`<br>`Group: 1`<br>`Group: 2` |
| **Leds** | `Array<INT>` | The id's or indexes for the LEDs this area will affect.<br>Set the indexes of the LEDs you want to target inside of the array. | `"Leds": [0, 1, 2]`<br>`"Leds": [2, 8]`
| **UseRange** | `BOOL` | If **True**, the `Leds` property array will be interpreted as a range instead.<br>Example 1: `[2, 8]` will color every led from index **2 to 8**<br>Example 2: `[2, 8, 12, 23]` will color from **2 to 8** and from **12 to 23** | `"UseRange": true` |
| **DarkOnStart** | `BOOL` | If **True**, the LEDs for this arrea will be set to black when the round is about to start, and then light up again when the terror spawns. | `"DarkOnStart": true` |
| **FlashOnMidnight** | `BOOL` | If **True**, the LEDs for this area will flash or blink when a **Midnight** round is taking place and the terror is spawned. | `"FlashOnMidnight": true`

# Examples
> **NOTE:** Do not copy paste these examples and expect them to just work for your setup. Use this as a guide on how to structure your setup. Good luck!

```json
// Device with Round Type color that flashes on Midnights and uses Led indexes 0 and 1.
{
    "DeviceID": 2,
    "Areas": [
        {
            "FlashOnMidnight": true,
            "Group": 1,
            "Leds": [ 0, 1 ]
        }
    ],
    "//": "You can define multiple devices"
}
```

```json
// Device with Terror color that flashes on midnights and uses all the Leds on this device.
{
    "DeviceID": 2,
    "Areas": [
        {
            "FlashOnMidnight": true,
            "Group": 1,
            "Leds": [ ]
        }
    ],
    "//": "You can define multiple devices"
}
```

```json
// Device with (Alive/Dead) color that uses a range of Leds from index 2 to 9
{
    "DeviceID": 2,
    "Areas": [
        {
            "Group": 2,
            "Leds": [ 2, 9 ],
            "UseRange": true
        }
    ],
    "//": "You can define multiple devices"
}
```


```json
// Device with Terrors colors on the first area, and Round colors on the second one.
{
    "DeviceID": 2,
    "Areas": [
        {
            "Group": 0,
            "Leds": [ 0, 1, 2, 3 ,4 ]
        },
        {
            "Group": 1,
            "Leds": [ 8, 12 ],
            "UseRange": true
        }
    ],
    "//": "You can define multiple devices"
}
```

```json
// Device that colors all the LEDs to Terror color, and overrides other LEDs to Round color.
{
    "DeviceID": 2,
    "Areas": [
        {
            "Group": 0,
            "Leds": [ ]
        },
        {
            "Group": 1,
            "Leds": [ 0, 12 ],
            "UseRange": true
        }
    ]
}
```

# Full setup Example
```json
{
  "IP": "127.0.0.1",
  "Port": 6742,
  "FPS": 60,
  "Entries": [
    {
      "DeviceID": 1,
      "Areas": [
        {
          "DarkOnStart": true,
          "Group": 0,
          "Leds": []
        },
        {
          "DarkOnStart": true,
          "Group": 0,
          "Leds": [ 0, 7 ],
          "UseRange": true,
          "FlashOnMidnight": true
        }
      ]
    },
    {
      "DeviceID": 2,
      "Areas": [
        {
          "FlashOnMidnight": true,
          "Group": 1,
          "Leds": [
            0,
            1
          ]
        }
      ]
    }
  ]
}
```

# Troubleshoot
- If your device is turned off and doesn't change, you might have forgotten to set the device `Mode` to something other than **Off** on the **OpenRGB** devices tab.