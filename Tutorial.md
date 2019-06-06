# Tutorial: Adding new objects

## Creating a new definition
If you open custom.json inside the definitions folder you can add your own objects in it. 
** I recommend saving this file with a different name, so you work isn't overwritten when the mod updates. **

Always make sure that all objects are separated by commas, and that the whole list is inside [] brackets, otherwise, the file will not load.

So, for a blank file, you should start with this:
```javascript
[
	{
		"filter": "",
		"sound": "",
		"minutesToHarvest": 1,
		"requireTool":true,	
		"tools": [],
		"yield": []
	}
]
```
## Filter/Name
First thing you need to find is the name of the object you want to target, for this I've added **F3** as a debug key, when you press it, it will print on screen and on the console the name of the object under the crosshair.
Only valid objects are those that start with OBJ, for example, targeting the laptop you'd get something like **OBJ_ComputerLaptop_Prefab**. If you see something that starts with INTERACTIVE_, GEAR_ or STR_, it won't work on those objects.
Sometimes it might show something like **WoldView**, move around or crouch and try again until it outputs the object. Also, there are some objects that can't be targeted no matter what you do.

For the filter, we only need the name, without prefixes or suffixes, so in this case it would be __ComputerLaptop__ what we would put in the filter field.

So objects, like paintings or rugs have a letter in the name like: **OBJ_PictureFrameA_LOD0** or **OBJ_RugF_Prefab** (the suffix after the name doesn't matter). In this case we have some options:
* We can target only that specific rug or painting by using __PictureFrameA__ or __RugF__ as filter.
* We can target all painting or rugs, simply using __PictureFrame__ and __Rug__

Basically what filter does is search for every object which name starts with __OBJ_[filter]__, not caring what comes after it.

So, in our example, we would now have this:
```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "",
		"minutesToHarvest": 1,
		"requireTool":true,	
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

Now, on minutesToHarvest we can set how long it will take to break down, lets say 45 minutes, so now we would have:
```javascript
[
	{
		"filter": "ComputerLaptop",
		"sound": "Generic",
		"minutesToHarvest": 45,
		"requireTool":true,	
		"tools": [],
		"yield": []
	}
]
```

## Tools
The field requireTool can be set to false if we want to be able to harvest by hand, and to true if a tool is needed.

In the tools field we can add which tools we want to allow on the object. These are the possible values:
* knife
* hacksaw
* hatchet
* hammer

For our example, let's say we can break the laptop down with a hacksaw, but also with a hammer:

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

Possible values for yields are:
* cloth
* scrap
* tinder
* stick
* wood - (this is reclaimed wood)
* stone
* line

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
				"item":"scrap",
				"num": 2
			},{
				"item":"cloth",
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
				"item":"scrap",
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
				"item":"scrap",
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