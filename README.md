# Remove Clutter
This mods allows you to remove most of the clutter in TLD. In reality it's a system to adds the break down functionality to almost any object through the config files. It's possible to edit the added items properties (which tool can be used, if tools are required, how much time it takes, what sound it plays and what objects it yields), and also you can add your own.

This mod is compatible with [Home-Improvement](https://github.com/WulfMarius/Home-Improvement) by Wulfmarius, and in fact, having both installed is extremely recommended. Also works on it's own.

## Important Notice
Before you can use this, or any other mod, make sure of the following:
* You have The Long Dark **version 1.49+** installed.
* You have downloaded the latest version of **ModLoader** (https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases/tag/v1.5) and patched your TLD game.

Also, in particular to this mod, you'll need **ModSettings 1.5** and **ModComponent 3.2.1** installed in order for the mod to work. You can get it in two ways:
* Using Wulfmarius' Mod Installer, it will download automatically as they are dependencies
* Installed manually from https://github.com/zeobviouslyfakeacc/ModSettings/releases/tag/v1.5 and https://github.com/WulfMarius/ModComponent/releases/tag/3.2.1 respectively

#### If there's red text on the screen when the game loads, most probable cause it's that you don't have these dependencies installed

## Mod Options
In the Mod Options menu, you have three options:
* **Tools Not Required**: Removes tool requirement for all objects added through the mod.
* **Fast Breakdown**: All objects added through the mod will take 1 minute to break down.
* **No Objects Yield**: Object added through the mod will not yield anything when broken down.

The effects aren't immediate and require a scene load to be applied, going out and back inside will apply them.

## Editting/Adding clutter
The config files are located in TLD_FOLDER/mods/remove-clutter/definitions.
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
Break down won't be applied to objects that act as container, bed or if it contains a buffer memory screen. So if you find something that you can't remove but it is defined, that might be the reason.

## Known Issues
Some objects don't drop down to the closest surface when something underneath is harvested. This is partly due to all objects not being created equal, so will be reviewed case by case. Best fix for this is... starting with the topmost objects.