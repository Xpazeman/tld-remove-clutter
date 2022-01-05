# Tutorial: Adding new objects to be removed

## Creating a new definition
If you open custom.json inside the definitions folder you can add your own objects in it. 

**I recommend saving this file with a different name, so you work isn't overwritten when the mod updates.**

Always make sure that all objects are separated by commas, and that the whole list is inside [] brackets, otherwise, the file will not load.

So, for a blank file, you should start with this:
```javascript
[
	{
		"filter": "",
		"sound": "",
		"minutesToHarvest": 1,
		"requireTool":false,	
		"tools": [],
		"yield": []
	}
]
```
## Filter/Name
First thing you need to find is the name of the object you want to target, for this I've added **F3** as a debug key, when you press it, it will print on screen and on the console the name of the object under the crosshair.
Only valid objects are those that start with OBJ, for example, targeting the laptop you'd get something like **OBJ_ComputerLaptop_Prefab**. If you see something that starts with INTERACTIVE_, GEAR_ or STR_, discard it as it won't work on those objects.
Sometimes it might show something like **WoldView** when you try to target something. In that case, move around or crouch and try again until it outputs the correct object. Also, keep in mind that there are some objects that can't be targeted no matter what you do.

**Keep in mind that the mod is disabled outside for performance reasons, so even if you add objects, they won't be breakable.**

For the filter field, we only need the name, without prefixes or suffixes, so in this case it would be _ComputerLaptop_ what we would put in the filter field.

So objects, like paintings or rugs have a letter in the name like: **OBJ_PictureFrameA_LOD0** or **OBJ_RugF_Prefab** (the suffix after the name doesn't matter). In this case we have some options:
* We can target only that specific rug or painting by using _PictureFrameA_ or _RugF_ as filter.
* We can target all painting or rugs, simply using _PictureFrame_ and _Rug_
(See definitions in decorations.json for examples of this)

Basically what filter does is search for every object which name starts with **_OBJ_[filter]_**, not caring what comes after it.

So, in our example, we would now have this:
```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "",
		"minutesToHarvest": 1,
		"requireTool":false,	
		"tools": [],
		"yield": []
	}
]
```

## Sound & Time to harvest
These are the possible values we can set this to:
* MetalSaw				
* WoodSaw		
* Generic				
* Metal	
* Cardboard		
* NylonCloth			
* Plants				
* Paper				
* Wood					
* Wool					
* Leather				
* WoodReclaimedNoAxe	
* WoodReclaimed		
* Cloth				
* MeatLarge			
* WoodSmall		
* WoodAxe	

For our example, we will use the Generic one.

The minutesToHarvest field defines how long it takes to harvest the item by hand, if you use a tool its modifier will be applied to this value.

Now, on minutesToHarvest we can set how long it will take to break down, lets say 45 minutes, so now we would have:
```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":false,	
		"tools": [],
		"yield": []
	}
]
```

## Tools
The field requireTool can be set to false if we want to be able to harvest by hand, and to true if a tool is needed, chosen tool will affect total harvest time.

In the tools field we can add which tools we want to allow on the object. These are the possible values along with the reduction they apply):
* knife - 0.4 reduction, 60 minutes become 36
* hacksaw - No reduction
* hatchet - 0.5 reduction, 60 minutes become 30
* hammer - 0.25 reduction, 60 minutes become 45

For our example, let's say we can break the laptop down with a hacksaw (will take 45 minutes when selected), also with a hammer (will take 34 minutes when selected), but not by hand:

```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":true,	
		"tools": [
			"hacksaw",
			"hammer"
		],
		"yield": []
	}
]
```

## Yields
Each type of yield should be their own object in the array, with a field for the object, and another one for the number of items it yields.

You can make the item yield any other GEAR item you want. For this, you need the name of the object you want to spawn, **minus the GEAR_ prefix**. Some examples:
* ScrapMetal
* ReclaimedWoodB
* Stick
* Line

You can use the dev console for a list of items, or use F3 on items in game to see their name. You can get the gear names from the modding wiki: https://the-long-dark-modding.fandom.com/wiki/Gear_Name

Yes, you can make a poster yield beefjerky when harvested if you want to.

If any mod adds new GearItems, you can use them too. For example, HomeImprovement adds GEAR_CumpledPaper, so you can use CumpledPaper as a yield item type. If no object is found with that name, it simply won't spawn.

In the case of our laptop, let's make it give us 2 pieces of scrap, and why not, a piece of cloth (just for tutorial purposes)

So now our object definition would finally look like this:
```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":true,	
		"tools": [
			"hacksaw",
			"hammer"
		],
		"yield": [
			{
				"item":"ScrapMetal",
				"num": 2
			},{
				"item":"Cloth",
				"num": 1
			}	
		]
	}
]
```

## Adding more items
Now, if we wanted to add more items, we can copy the one we made and modify from there, always making sure the structure is well formatted.

```javascript
[   //------------------------------------> Start of objects array
	{
		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":true,	
		"tools": [
			"hacksaw",
			"hammer"
		],
		"yield": [
			{
				"item":"ScrapMetal",
				"num": 2
			},{
				"item":"cloth",
				"num": 1
			}	
		]

	} , {		//-----------------------------> Notice the comma between the objects

		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":true,	
		"tools": [
			"hacksaw",
			"hammer"
		],
		"yield": [
			{
				"item":"ScrapMetal",
				"num": 2
			},{
				"item":"cloth",
				"num": 1
			}	
		]
	}
]   //------------------------------------> End of objects array
```

In order to validate your file, or if it fails and you're not sure why, you can use this tool to debug it:
https://jsonlint.com/

