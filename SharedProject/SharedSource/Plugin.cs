using System;
// using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Barotrauma;
using Microsoft.Xna.Framework;
using HarmonyLib;
using System.Runtime.CompilerServices;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace CrossClass
{
    public partial class CrossClass : IAssemblyPlugin
    {
        Harmony harmony = new Harmony("com.cross_class.plugin");

        public void Initialize()
        {
            // When your plugin is loading, use this instead of the constructor
            // Put any code here that does not rely on other plugins.
            LuaCsLogger.LogMessage("Trying to patch for Cross Class...");
            harmony.PatchAll();
            LuaCsLogger.LogMessage("Done.");
        }

        public void OnLoadCompleted()
        {
            // After all plugins have loaded
            // Put code that interacts with other plugins here.
        }

        public void PreInitPatching()
        {
            // Not yet supported: Called during the Barotrauma startup phase before vanilla content is loaded.
            
        }

        void IDisposable.Dispose()
        {
            // Cleanup your plugin!
            // Honestly do nothing rn
            LuaCsLogger.LogMessage("Trying to unpatch Cross Class...");
            harmony.UnpatchSelf();
            LuaCsLogger.LogMessage("Done.");
        }
    }
}
