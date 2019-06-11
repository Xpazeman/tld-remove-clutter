using System;
using Harmony;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace RemoveClutter
{
    internal class Patches
    {
        [HarmonyPatch(typeof(TimeOfDay), "Update")]
        internal class TimeOfDay_Update
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
                if (InterfaceManager.IsMainMenuActive() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !RemoveClutter.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                {
                    Debug.Log("[remove-clutter] " + GameManager.m_ActiveScene + " is outdoor scene, mod disabled.");
                    return;
                }
                    

                string text = SaveGameSlots.LoadDataFromSlot(name, sceneSaveName);
                SceneSaveGameFormat saveGameFormat = Utils.DeserializeObject<SceneSaveGameFormat>(text);

                RemoveClutter.sceneBreakDownData = saveGameFormat.m_BreakDownObjectsSerialized;

                RemoveClutter.PatchSceneObjects();
            }
        }

        //Clear missing objects before serializing
        [HarmonyPatch(typeof(BreakDown), "SerializeAll")]
        internal class BreakDown_SerializeAll
        {
            public static void Prefix(BreakDown __instance)
            {
                List<BreakDown> okItems = new List<BreakDown>();

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
    }
}
