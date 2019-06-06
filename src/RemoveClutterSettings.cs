using System.IO;
using System.Reflection;
using ModSettings;
using System;

namespace RemoveClutter
{
    internal class RemoveClutterOptions
    {
        public bool noToolsNeeded = false;
        public bool fastBreakDown = false;
        public bool noYield = false;
        //public bool preventBufferRemove = true;
    }

    internal class RemoveClutterSettings : ModSettingsBase
    {
        internal readonly RemoveClutterOptions setOptions = new RemoveClutterOptions();

        [Name("Tools not required")]
        [Description("If active, clutter won't need any tools to be removed.")]
        public bool noToolsNeeded = false;

        [Name("Fast breakdown")]
        [Description("If active, clutter will just take 1 minute to remove.")]
        public bool fastBreakDown = false;

        [Name("No objects yield")]
        [Description("If active, clutter will not yield any objects when harvested.")]
        public bool noYield = false;

        /*[Name("Prevent Buffer Memories removal")]
        [Description("If active, you won't be able to remove computers that contain Buffer Memories.")]
        public bool preventBufferRemove = true;*/

        internal RemoveClutterSettings()
        {
            //Load settings
            if (File.Exists(Path.Combine(RemoveClutter.modDataFolder, RemoveClutter.settingsFile)))
            {
                string opts = File.ReadAllText(Path.Combine(RemoveClutter.modDataFolder, RemoveClutter.settingsFile));
                setOptions = FastJson.Deserialize<RemoveClutterOptions>(opts);

                noToolsNeeded = setOptions.noToolsNeeded;
                fastBreakDown = setOptions.fastBreakDown;
                noYield = setOptions.noYield;
                //preventBufferRemove = setOptions.preventBufferRemove;

            }
        }

        protected override void OnConfirm()
        {
            setOptions.noToolsNeeded = noToolsNeeded;
            setOptions.fastBreakDown = fastBreakDown;
            setOptions.noYield = noYield;
            //setOptions.preventBufferRemove = preventBufferRemove;

            string jsonOpts = FastJson.Serialize(setOptions);

            File.WriteAllText(Path.Combine(RemoveClutter.modDataFolder, RemoveClutter.settingsFile), jsonOpts);
        }
    }
}
