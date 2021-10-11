using System;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace RemoveClutter
{
    internal class Patches
    {
        //[HarmonyPatch(typeof(TimeOfDay), "Update")]
        [HarmonyPatch(typeof(TimeOfDay), "UpdateUniStormDayLength")]
        internal class TimeOfDay_UpdateUniStormDayLength
        {
            public static void Postfix(TimeOfDay __instance)
            {
                RCUtils.TimeUpdate();
            }
        }

        [HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData", new Type[] { typeof(string), typeof(string) })]
        internal class SaveGameSystem_LoadSceneData
        {
            public static void Postfix(SaveGameSystem __instance, string name, string sceneSaveName)
            {
                if (InterfaceManager.IsMainMenuEnabled() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !RemoveClutter.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                {
                    Debug.Log("[remove-clutter] " + GameManager.m_ActiveScene + " is outdoor scene, mod disabled.");
                    return;
                }
                    

                string text = SaveGameSlots.LoadDataFromSlot(name, sceneSaveName);
                SceneSaveGameFormat saveGameFormat = Utils.DeserializeObject<SceneSaveGameFormat>(text);

                RemoveClutter.sceneBreakDownData = saveGameFormat.m_BreakDownObjectsSerialized;

                //Debug.Log(RemoveClutter.sceneBreakDownData);

                RemoveClutter.PatchSceneObjects();
                RemoveClutter.PatchSceneDecals();

                BreakDown.DeserializeAllAdditive(RemoveClutter.sceneBreakDownData);
            }
        }

        //Clear missing objects before serializing
        [HarmonyPatch(typeof(BreakDown), "SerializeAll")]
        internal class BreakDown_SerializeAll
        {
            public static void Prefix(BreakDown __instance)
            {
                Il2CppSystem.Collections.Generic.List<BreakDown> okItems = new Il2CppSystem.Collections.Generic.List<BreakDown>();

                for (int i = 0; i < BreakDown.m_BreakDownObjects.Count; i++)
                {
                    BreakDown breakDown = BreakDown.m_BreakDownObjects[i];
                    if (breakDown != null)
                    {
                        okItems.Add(breakDown);
                    }
                }

                BreakDown.m_BreakDownObjects = okItems;
            }
        }

        /*[HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessAltFire")]
        internal class PlayerManager_InteractiveObjectsProcessAltFire
        {
            public static bool Prefix(PlayerManager __instance)
            {
                var gameObject = __instance.m_InteractiveObjectUnderCrosshair;

                if (gameObject.name.Contains("xpzclutter"))
                {
                    //BetterPlacing.PreparePlacableFurniture(gameObject);

                    if (gameObject.GetComponent<BreakDown>() != null)
                    {
                        __instance.StartPlaceMesh(gameObject, 5f, PlaceMeshFlags.None);
                    }else if (gameObject.GetComponentInChildren<BreakDown>() != null)
                    {
                        GameObject lod = gameObject.GetComponentInChildren<BreakDown>().gameObject;
                        __instance.StartPlaceMesh(lod, 5f, PlaceMeshFlags.None);
                    }
                    

                    return false;
                }

                return true;
            }
        }*/
    }
}
