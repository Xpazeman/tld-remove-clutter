using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemoveClutter
{
    internal class Patches
    {
        [HarmonyPatch(typeof(MissionServicesManager), "SceneLoadCompleted")]
        internal class MissionServicesManager_SceneLoadCompleted
        {
            private static void Postfix(MissionServicesManager __instance)
            {
                //Patch scene objects after scene load
                if (!InterfaceManager.IsMainMenuActive())
                    RemoveClutter.PatchSceneObjects();
            }
        }

        [HarmonyPatch(typeof(TimeOfDay), "Update")]
        internal class TimeOfDay_Update
        {
            public static void Postfix(TimeOfDay __instance)
            {
                RCUtils.TimeUpdate();
            }
        }
        [HarmonyPatch(typeof(BreakDown), "StickToGround")]
        internal class BreakDown_StickToGround
        {
            public static void Prefix(BreakDown __instance, GameObject go)
            {
                //Debug.Log("sticking " + go.name);
            }

            public static void Postfix()
            {
                
            }

            
        }

    }
}
