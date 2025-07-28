using System;
// using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace CrossClass;

#pragma warning disable CS0618 // Type or member is obsolete
public partial class CrossClass : ACsMod, IAssemblyPlugin
#pragma warning restore CS0618 // Type or member is obsolete
{
    Harmony harmony = new Harmony("com.cross_class.plugin");
    public static CrossClass Instance;
    public static bool IsCampaign => GameMain.GameSession?.Campaign != null || GameMain.IsSingleplayer;
    public static bool IsRunning => GameMain.GameSession?.IsRunning ?? false;

    public static bool IsClient => GameMain.NetworkMember != null && GameMain.NetworkMember.IsClient;

    public override void Initialize()
    {
        LuaCsLogger.LogMessage("Trying to patch for Cross Class...");

        Instance = this;
        // GameMain.LuaCs.Hook.Add("roundStart", (args)=>
        // {
        //     CrossClassSync.Instance.Setup();
        //     return null;
        // });
        CrossClassSync.Instance.Setup();

        LuaCsLogger.LogMessage("Sync instance initialized");
        harmony.PatchAll();
        LuaCsLogger.LogMessage("Done.");
    }

     public static void Hook(string name, string hookName, LuaCsFunc hook) => 
            GameMain.LuaCs.Hook.Add(name, hookName, hook, Instance);
        // public static void HookMethod(string identifier, MethodInfo method, LuaCsPatch patch, LuaCsHook.HookMethodType hookType) =>
        //     GameMain.LuaCs.Hook.HookMethod(identifier, method, patch, hookType, Instance);

    public override void OnLoadCompleted()
    {
        // After all plugins have loaded
        // Put code that interacts with other plugins here.
    }

    void IAssemblyPlugin.PreInitPatching()
    {
        // Not yet supported: Called during the Barotrauma startup phase before vanilla content is loaded.

    }

	public override void Stop()
	{
        // We do not care.
		// throw new NotImplementedException();
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
