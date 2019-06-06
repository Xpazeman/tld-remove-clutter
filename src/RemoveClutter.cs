using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ModComponentMapper;

namespace RemoveClutter
{
    internal class RemoveClutter
    {
        public static string modsFolder;
        public static string modDataFolder;
        public static string settingsFile = "config.json";

        public static RemoveClutterOptions options;

        public static List<GameObject> itemList = new List<GameObject>();
        public static List<BreakDownDefinition> objList = null;

        public static bool scenePatched = false;

        public static bool verbose = true;

        public static void OnLoad()
        {
            Debug.Log("[remove-clutter] Version " + Assembly.GetExecutingAssembly().GetName().Version);

            modsFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            modDataFolder = Path.Combine(modsFolder, "remove-clutter");

            RemoveClutterSettings settings = new RemoveClutterSettings();
            settings.AddToModSettings("Remove Clutter Settings");
            options = settings.setOptions;

            LoadBreakDownDefinitions();
        }

        internal static void LoadBreakDownDefinitions()
        {
            String defsPath = Path.Combine(modDataFolder, "definitions");
            string[] defFiles = Directory.GetFiles(defsPath, "*.json");

            if (defFiles.Length > 0)
            {
                objList = new List<BreakDownDefinition>();

                for (int i = 0; i < defFiles.Length; i++)
                {
                    string data = File.ReadAllText(defFiles[i]);
                    List<BreakDownDefinition> fileObjs = null;

                    try
                    {
                        fileObjs = FastJson.Deserialize<List<BreakDownDefinition>>(data);

                        objList.AddRange(fileObjs);

                        Debug.Log("[remove-clutter] " + Path.GetFileName(defFiles[i]) + " definitions loaded ");
                    }
                    catch(FormatException e)
                    {
                        Debug.Log("[remove-clutter] ERROR: " + Path.GetFileName(defFiles[i]) + " incorrectly formatted.");
                    }
                }
            }
        }

        //Item preparation - searches every object that appears on the config file
        internal static void PatchSceneObjects()
        {
            if (objList == null)
            {
                return;
            }
            
            //Get list of all root objects
            List<GameObject> rObjs = RCUtils.GetRootObjects();

            //Results container
            List<GameObject> result = new List<GameObject>();
            
            //Clear object list
            itemList.Clear();

            //Iterate over obj config list
            foreach (BreakDownDefinition obj in objList)
            {
                if (obj.filter.Trim() == "" || obj.filter == null)
                {
                    continue;
                }

                foreach (GameObject rootObj in rObjs)
                {
                    RCUtils.GetChildrenWithName(rootObj, "OBJ_" + obj.filter, result);

                    if (result.Count > 0)
                    {
                        foreach (GameObject child in result)
                        {
                            if (child != null && !child.name.Contains("xpzclutter"))
                            {
                                child.name += "_xpzclutter";

                                PrepareGameObject(child, obj);

                                if (verbose)
                                    Debug.Log("[remove-clutter] Added: " + child.name);
                            }

                        }
                    }
                }
            }
        }

        internal static void PrepareGameObject(GameObject gameObject, BreakDownDefinition objDef)
        {
            //Object preparer adapted from WulfMarius' Home-Improvement
            //github.com/WulfMarius/Home-Improvement/blob/master/VisualStudio/src/preparer/BreakDownPapers.cs

            Renderer renderer = Utils.GetLargestBoundsRenderer(gameObject);
            if (renderer == null)
            {
                return;
            }

            
            RCUtils.SetLayer(gameObject);

            //Get Children with the collider
            Collider collider = gameObject.GetComponentInChildren<Collider>();
            if (collider != null)
            {
                AddBreakDownComponent(gameObject, objDef);
                gameObject = collider.gameObject;
            }
            else
            {
                GameObject trigger = new GameObject("XPZClutterHolder-" + gameObject.name);
                trigger.transform.parent = gameObject.transform.parent;
                trigger.transform.position = gameObject.transform.position;

                gameObject.transform.parent = trigger.transform;

                gameObject = trigger;

                AddBreakDownComponent(gameObject, objDef);

                //If no collider found, add one to the trigger
                Bounds bounds = renderer.bounds;

                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = bounds.size;
                boxCollider.center = bounds.center - gameObject.transform.position;
            }

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderGO in renderers)
            {
                AddBreakDownComponent(renderGO.gameObject, objDef);
                //RCUtils.SetLayer(renderGO.gameObject);
            }
        }

        internal static void AddBreakDownComponent(GameObject gameObject, BreakDownDefinition objDef)
        {
            //gameObject.AddComponent<StickToGround>();
            BreakDown breakDown = gameObject.AddComponent<BreakDown>();

            //Object yields
            if (objDef.yield != null && objDef.yield.Length > 0 && !options.noYield)
            {
                List<GameObject> itemYields = new List<GameObject>();
                List<int> numYield = new List<int>();

                foreach (BreakDownYield yield in objDef.yield)
                {
                    if (yield.item == "cloth")
                    {
                        itemYields.Add(Resources.Load("GEAR_Cloth") as GameObject);
                    }
                    else if (yield.item == "scrap")
                    {
                        itemYields.Add(Resources.Load("GEAR_ScrapMetal") as GameObject);
                    }
                    else if (yield.item == "tinder")
                    {
                        itemYields.Add(Resources.Load("GEAR_Tinder") as GameObject);
                    }
                    else if (yield.item == "stick")
                    {
                        itemYields.Add(Resources.Load("GEAR_Stick") as GameObject);
                    }
                    else if (yield.item == "wood")
                    {
                        itemYields.Add(Resources.Load("GEAR_ReclaimedWoodB") as GameObject);
                    }
                    else if (yield.item == "stone")
                    {
                        itemYields.Add(Resources.Load("GEAR_Stone") as GameObject);
                    }
                    else if (yield.item == "line")
                    {
                        itemYields.Add(Resources.Load("GEAR_Line") as GameObject);
                    }

                    numYield.Add(yield.num);
                }

                breakDown.m_YieldObject = itemYields.ToArray();
                breakDown.m_YieldObjectUnits = numYield.ToArray();
            }
            else
            {
                breakDown.m_YieldObject = new GameObject[0];
                breakDown.m_YieldObjectUnits = new int[0];
            }


            //Time to harvest
            if (objDef.minutesToHarvest > 0 && !options.fastBreakDown)
                breakDown.m_TimeCostHours = objDef.minutesToHarvest / 60;
            else
                breakDown.m_TimeCostHours = 1f / 60;

            //Harvest sound
            /*MetalSaw				
              WoodSaw				
              Outerwear			
              MeatlSmall			
              Generic				
              Metal			
              MeatlMed				
              Cardboard			
              WoodCedar			
              NylonCloth			
              Plants				
              Paper				
              Wood					
              Wool					
              Leather				
              WoodReclaimedNoAxe	
              WoodReclaimed		
              Cloth				
              MeatLarge			
              WoodSmall			
              WoodFir				
              WoodAxe		*/
            if (objDef.sound.Trim() != "" && objDef.sound != null)
                breakDown.m_BreakDownAudio = "Play_Harvesting" + objDef.sound;
            else
                breakDown.m_BreakDownAudio = "Play_HarvestingGeneric";

            //Display name
            breakDown.m_LocalizedDisplayName = new LocalizedString() { m_LocalizationID = "GAMEPLAY_BreakDown" };

            //Required Tools
            if (objDef.requireTool == true && !options.noToolsNeeded)
            {
                breakDown.m_RequiresTool = true;
            }

            if (objDef.tools != null && objDef.tools.Length > 0 && !options.noToolsNeeded)
            {
                List<GameObject> itemTools = new List<GameObject>();

                foreach (String tool in objDef.tools)
                {
                    if (tool == "knife")
                    {
                        itemTools.Add(Resources.Load("GEAR_Knife") as GameObject);
                    }
                    else if (tool == "hacksaw")
                    {
                        itemTools.Add(Resources.Load("GEAR_Hacksaw") as GameObject);
                    }
                    else if (tool == "hatchet")
                    {
                        itemTools.Add(Resources.Load("GEAR_Hatchet") as GameObject);
                    }
                    else if (tool == "hammer")
                    {
                        itemTools.Add(Resources.Load("GEAR_Hammer") as GameObject);
                    }
                }

                breakDown.m_UsableTools = itemTools.ToArray();
            }
            else
            {
                breakDown.m_UsableTools = new GameObject[0];
            }
        }
    }
}

	
