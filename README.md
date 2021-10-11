# Remove Clutter
This mods allows you to remove most of the clutter in TLD. In reality it's a system to add the break down functionality to almost any object through config files. It's possible to edit the added items properties (which tool can be used, if tools are required, how much time it takes, what sound it plays and what objects it yields), and also you can add your own.

## Updates
### v2.3
* Updated to work with TLD 1.95 - Fury, then silence.

### v2.2
* FIX: Fixed issue with paper decals not saved after being broken down.

## Installing the mod
* If you haven't done so already, install MelonLoader by downloading and running [MelonLoader.Installer.exe](https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.Installer.exe)
* You'll need to download and install [ModSettings](https://github.com/zeobviouslyfakeacc/ModSettings/releases/download/v1.7/ModSettings.dll) by Zeo. **MOD WON'T WORK WITHOUT IT**
* Download the latest version of RemoveClutter.zip from the [releases page](https://github.com/Xpazeman/tld-remove-clutter/releases/latest).
* Extract RemoveClutter.zip into the Mods folder in your TLD install directory, you should have a folder name remove-clutter and RemoveClutter.dll in there
* **IMPORTANT: DO NOT leave the .zip folder in the Mods folder as this will make Melon Loader error.**
* **UPDATING**: Simply unzip the file in your mods folder and overwrite the old files. Remember to remove the .zip file from the folder.

## Mod Options
In the Mod Options menu, you have four options:
* **Tools Required**: If disabled, removes tool requirement for all objects added through the mod.
* **Fast Breakdown**: All objects added through the mod will only take 1 minute to break down.
* **Objects Yield**: If disabled, object added through the mod will not yield anything when broken down.
* **Show object names**: Shows the object name instead of a generic 'Break Down' text when hovering. These names aren't translated.

The effects aren't immediate and require a scene load to be applied, going out and back inside will apply them.

## Editting/Adding clutter
The config files are located in TLD_FOLDER/Mods/remove-clutter/definitions.
Inside you will find .json files, divided by categories.

When you open one, you can see a json array of objects, each defining one object to apply to, for example, a laptop is defined like this:

```javascript
{
	"filter": "ComputerLaptop",		// This will be the search used to find the object in the scene.
	"sound": "Generic",				// Sound it will make when broken down. 
	"minutesToHarvest": 45.0,		// Minutes it takes to break the object down.
	"requireTool":true,				// If true, removes the option to break down by hand.
	"tools": [						// Array of tools that can be used. 
		"hacksaw",
		"hammer"
	],
	"yield": [{						// Array of objects that will be yield. 
		"item":"scrap",
		"num":1
	}]
}
```

Or the podium from Milton's church:
```javascript
{
	"filter": "Podium",
	"sound": "WoodReclaimedNoAxe",
	"minutesToHarvest": 45.0,
	"requireTool":true,
	"tools": [
		"hatchet"
	],
	"yield": [{
		"item":"wood",
		"num":3
	}]
}
```

You can edit these values and save the file, and next time you start the game, the new values will be used.
Definition files are loaded in alphabetical order, so adding for example a 0 or something like that at the start of the custom.json file will make your custom definitions load before the mod ones. Also, since definitions don't overwrite, if you redefine something in your custom file, it will override the mod's default. This is helpful so when the mod updates your edits are kept (remember to change the custom.json file name)

You can take a look at the definiton files [here](./src/remove-clutter/definitions)

You can read a simple tutorial on how to create new item definitions [here](./Tutorial.md)

## Special cases
Break down won't be applied to objects that act as container, bed or if it contains a buffer memory screen. So if you find something that you can't remove but it is defined in the JSON files, that might be the reason.
Due to performance reasons, the mod will only work with interiors.

## Known Issues
Some objects don't drop down to the closest surface when something underneath is harvested. This is partly due to all objects not being created equal, so will be reviewed case by case. Best fix for this is... starting with the topmost objects.