using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

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

        public static string sceneBreakDownData = null;

        public static bool verbose = false;

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

            int setupObjects = 0;

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

                                setupObjects++;

                                if (verbose)
                                    Debug.Log("[remove-clutter] Added: " + child.name);
                            }

                        }
                    }
                }
            }

            Debug.Log("[remove-clutter] " + setupObjects + " objects setup for removal.");

            BreakDown.DeserializeAllAdditive(sceneBreakDownData);
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

            AddBreakDownComponent(gameObject, objDef);

            //Check if it has collider, add one if it doesn't
            Collider collider = gameObject.GetComponentInChildren<Collider>();
            if (collider == null)
            {
                //AddBreakDownComponent(gameObject, objDef);
                Bounds bounds = renderer.bounds;

                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = bounds.size;
                boxCollider.center = bounds.center - gameObject.transform.position;
            }

            //Set children to interactive layer
            if (gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    RCUtils.SetLayer(gameObject.transform.GetChild(i).gameObject, vp_Layer.InteractiveProp);
                }
            }
        }

        internal static void AddBreakDownComponent(GameObject gameObject, BreakDownDefinition objDef)
        {
            BreakDown breakDown = gameObject.AddComponent<BreakDown>();
            BreakDown.m_BreakDownObjects.Add(breakDown);

            RCUtils.SetLayer(gameObject, vp_Layer.InteractiveProp);

            //Object yields
            if (objDef.yield != null && objDef.yield.Length > 0 && !options.noYield)
            {
                List<GameObject> itemYields = new List<GameObject>();
                List<int> numYield = new List<int>();

                foreach (BreakDownYield yield in objDef.yield)
                {
                    if (yield.item.Trim() != "")
                    {
                        GameObject yieldItem = Resources.Load("GEAR_" + yield.item) as GameObject;
                        if (yieldItem != null)
                        {
                            itemYields.Add(yieldItem);
                        }
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

            if (options.showObjectNames)
            {
                String rawName = objDef.filter.Replace("_", string.Empty);
                String[] objWords = Regex.Split(rawName, @"(?<!^)(?=[A-Z])");
                String objName = String.Join(" ", objWords);

                breakDown.m_LocalizedDisplayName = new LocalizedString() { m_LocalizationID = objName };
            }
            else
            {
                breakDown.m_LocalizedDisplayName = new LocalizedString() { m_LocalizationID = "GAMEPLAY_BreakDown" };
            }
            

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

	
