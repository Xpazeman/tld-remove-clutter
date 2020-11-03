using System;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using MelonLoader;
using MelonLoader.TinyJSON;

namespace RemoveClutter
{
    internal class RemoveClutter : MelonMod
    {
        private static readonly string MODS_FOLDER_PATH = Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods\remove-clutter");

        public static List<GameObject> itemList = new List<GameObject>();
        public static List<BreakDownDefinition> objList = null;

        public static string sceneBreakDownData = null;

        public static bool verbose = false;

        public static List<string> notReallyOutdoors = new List<string>
        {
            "DamTransitionZone"
        };

        public override void OnApplicationStart()
        {
            Debug.Log("[remove-clutter] Version " + Assembly.GetExecutingAssembly().GetName().Version);

            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<ChangeLayer>();

            Settings.OnLoad();

            LoadBreakDownDefinitions();
        }

        internal static void LoadBreakDownDefinitions()
        {
            String defsPath = Path.Combine(MODS_FOLDER_PATH, "definitions");
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
                        fileObjs = JSON.Load(data).Make<List<BreakDownDefinition>>();
                        

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
                            if (child != null && !child.name.Contains("xpzclutter") && child.GetComponent("RepairableContainer") == null)
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

        internal static void PatchSceneDecals()
        {
            GameObject[] rObjs = RCUtils.GetRootObjects().ToArray();

            foreach (GameObject rootObj in rObjs)
            {
                MeshRenderer childRenderer = rootObj.GetComponent<MeshRenderer>();
                MeshRenderer[] allRenderers = rootObj.GetComponentsInChildren<MeshRenderer>();
                allRenderers.Add(childRenderer);

                foreach (MeshRenderer renderer in allRenderers)
                {
                    if (renderer.gameObject.name.ToLower().Contains("decal"))
                    {
                        renderer.receiveShadows = true;
                        qd_Decal decal = renderer.GetComponent<qd_Decal>();
                        //Debug.Log("Decal " + renderer.gameObject.name + " layer" + decal.m_Layer + " texture:"+ decal.texture.name);
                        if (decal != null && (decal.texture.name.StartsWith("FX_DebrisPaper") || decal.texture.name.StartsWith("FX_DebrisMail") || decal.texture.name.StartsWith("FX_DebriPaper")))
                        {
                            
                            BreakDownDefinition bdDef = new BreakDownDefinition
                            {
                                filter = "Paper",
                                minutesToHarvest = 1f,
                                sound = "Paper"
                            };

                            PrepareGameObject(renderer.gameObject, bdDef);
                        }

                        continue;
                    }
                }
            }
        }

        internal static void PrepareGameObject(GameObject gameObject, BreakDownDefinition objDef)
        {
            //Object preparer adapted from WulfMarius' Home-Improvement
            //github.com/WulfMarius/Home-Improvement/blob/master/VisualStudio/src/preparer/BreakDownPapers.cs

            LODGroup lodObject = gameObject.GetComponent<LODGroup>();
            
            if (lodObject == null)
            {
                lodObject = gameObject.GetComponentInChildren<LODGroup>();
            }

            if (lodObject != null)
            {
                gameObject = lodObject.gameObject;
            }

            Renderer renderer = Utils.GetLargestBoundsRenderer(gameObject);
            
            if (renderer == null)
            {
                return;
            }
            
            //Check if it has collider, add one if it doesn't
            Collider collider = gameObject.GetComponent<Collider>();
            
            if (collider == null)
            {
                collider = gameObject.GetComponentInChildren<Collider>();
            }

            if (gameObject.name.StartsWith("Decal-"))
            {
                gameObject.transform.localRotation = Quaternion.identity;
                GameObject collisionObject = new GameObject("PaperDecalRemover-" + gameObject.name);
                collisionObject.transform.parent = gameObject.transform.parent;
                collisionObject.transform.position = gameObject.transform.position;

                gameObject.transform.parent = collisionObject.transform;

                gameObject = collisionObject;
            }

            if (collider == null)
            {
                //AddBreakDownComponent(gameObject, objDef);
                Bounds bounds = renderer.bounds;

                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = bounds.size;
                boxCollider.center = bounds.center - gameObject.transform.position;

                /*if (gameObject.name.StartsWith("Decal-"))
                {
                    boxCollider.size = new Vector3(boxCollider.size.x, 0.01f, boxCollider.size.z);

                    GameObject hint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    hint.transform.parent = gameObject.transform;
                    hint.transform.localScale = boxCollider.size;
                    hint.transform.position = new Vector3(0,0,0);
                }*/
            }

            AddBreakDownComponent(gameObject, objDef);

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
            if (objDef.yield != null && objDef.yield.Length > 0 && Settings.options.objectYields)
            {
                List<GameObject> itemYields = new List<GameObject>();
                List<int> numYield = new List<int>();

                foreach (BreakDownYield yield in objDef.yield)
                {
                    if (yield.item.Trim() != "")
                    {
                        //GameObject yieldItem = Resources.Load("GEAR_" + yield.item).Cast<GameObject>();

                        GameObject yieldItem = null;
                        UnityEngine.Object yieldItemObj = Resources.Load("GEAR_" + yield.item);

                        if (yieldItemObj != null)
                        {
                            yieldItem = yieldItemObj.Cast<GameObject>();
                            itemYields.Add(yieldItem);
                            numYield.Add(yield.num);
                        }
                        else
                        {
                            Debug.Log("[remove-clutter] Yield  GEAR_" + yield.item + " couldn't be loaded.");
                        }
                    }
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
            if (objDef.minutesToHarvest > 0 && !Settings.options.fastBreakDown)
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

            if (Settings.options.showObjectNames)
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
            if (objDef.requireTool == true && Settings.options.toolsNeeded)
            {
                breakDown.m_RequiresTool = true;
            }

            if (objDef.tools != null && objDef.tools.Length > 0 && Settings.options.toolsNeeded)
            {
                Il2CppSystem.Collections.Generic.List<GameObject> itemTools = new Il2CppSystem.Collections.Generic.List<GameObject>();

                foreach (String tool in objDef.tools)
                {
                    GameObject selectedTool = null;

                    if (tool.ToLower() == "knife")
                    {
                        selectedTool = Resources.Load("GEAR_Knife").Cast<GameObject>();
                    }
                    else if (tool.ToLower() == "hacksaw")
                    {
                        selectedTool = Resources.Load("GEAR_Hacksaw").Cast<GameObject>();
                    }
                    else if (tool.ToLower() == "hatchet")
                    {
                        selectedTool = Resources.Load("GEAR_Hatchet").Cast<GameObject>();
                    }
                    else if (tool.ToLower() == "hammer")
                    {
                        selectedTool = Resources.Load("GEAR_Hammer").Cast<GameObject>();
                    }

                    if (selectedTool != null)
                    {
                        itemTools.Add(selectedTool);
                    }
                    else
                    {
                        Debug.Log("[remove-clutter] Tool " + tool + " couldn't be loaded or doesn't exist.");
                    }
                    
                }

                UnhollowerBaseLib.Il2CppReferenceArray<GameObject> toolsArray = new UnhollowerBaseLib.Il2CppReferenceArray<GameObject>(itemTools.ToArray());

                if(toolsArray.Length > 0)
                {
                    breakDown.m_UsableTools = toolsArray;
                }
                else
                {
                    Debug.Log("[remove-clutter] Tools array is empty.");
                    breakDown.m_RequiresTool = false;
                    breakDown.m_UsableTools = new GameObject[0];
                }
            }
            else
            {
                breakDown.m_UsableTools = new GameObject[0];
            }

            
        }
    }
}

	
