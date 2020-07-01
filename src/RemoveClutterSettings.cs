using System.IO;
using System.Reflection;
using ModSettings;
using System;

namespace RemoveClutter
{
    internal class RemoveClutterSettings : JsonModSettings
    {
        [Name("Tools required")]
        [Description("If set to NO, clutter won't need any tools to be removed.")]
        public bool toolsNeeded = true;

        [Name("Fast breakdown")]
        [Description("If active, clutter will just take 1 minute to remove.")]
        public bool fastBreakDown = false;

        [Name("Objects yield")]
        [Description("If set to NO, clutter will not yield any objects when harvested.")]
        public bool objectYields = true;

        [Name("Show object names (no translation)")]
        [Description("If deactivated, mod will show a generic localized string, if activated, it will show the original object name without translation.")]
        public bool showObjectNames = true;
    }

    internal static class Settings
    {
        public static RemoveClutterSettings options;

        public static void OnLoad()
        {
            options = new RemoveClutterSettings();
            options.AddToModSettings("Remove Clutter Settings");
        }
    }
}
