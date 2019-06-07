using System;
using Harmony;
using UnityEngine;
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
                if (InterfaceManager.IsMainMenuActive() || GameManager.IsOutDoorsScene(GameManager.m_ActiveScene))
                    return;

                string text = SaveGameSlots.LoadDataFromSlot(name, sceneSaveName);
                SceneSaveGameFormat saveGameFormat = Utils.DeserializeObject<SceneSaveGameFormat>(text);

                RemoveClutter.sceneBreakDownData = saveGameFormat.m_BreakDownObjectsSerialized;

                RemoveClutter.PatchSceneObjects();
            }
        }

    }
}
