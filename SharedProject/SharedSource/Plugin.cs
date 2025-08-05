using System.Runtime.CompilerServices;
using Barotrauma;
using HarmonyLib;

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
    public static bool IsCampaign => GameMain.GameSession?.Campaign != null; //|| GameMain.IsSingleplayer;
    public static bool IsRunning => GameMain.GameSession?.IsRunning ?? false;

    public static bool IsClient => GameMain.NetworkMember != null && GameMain.NetworkMember.IsClient;

    public override void Initialize()
    {
        LuaCsLogger.LogMessage("Trying to patch for Cross Class...");
        
        if(GameMain.IsSingleplayer)
        {
            LuaCsLogger.LogError("""
            Hey mate! Developer of CrossClass here.
            Singleplayer doesn't really work with CrossClass in it's current state, as singleplayer encourages the use of bots to pad out your skillset.
            Since it'd also require a massive rewrite and I am truly cannot be bothered at the moment, it won't be enabled for this session.
            If you're still keen to use it, you should host a game instead; you can play a full solo campaign there without any problems!
            ~ Crona
            """);
            return;
        }

        Instance = this;
        LuaCsLogger.LogMessage("Sync instance initialized");
        harmony.PatchAll();
        CrossClassSync.Instance.Setup();
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
        if(GameMain.IsSingleplayer)
        {
            return;
        }
        // Honestly do nothing rn
        LuaCsLogger.LogMessage("Trying to unpatch Cross Class...");
        harmony.UnpatchSelf();
        LuaCsLogger.LogMessage("Done.");
    }
}
