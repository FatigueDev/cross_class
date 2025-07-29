

using Barotrauma;
using Barotrauma.Networking;
using HarmonyLib;

namespace CrossClass;

[HarmonyPatch]
public class CrewManager_Patches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(CrewManager), "AddCharacter")]
	public static void AddCharacter(Character character, ref CrewManager __instance)
	{

		LuaCsLogger.Log("Called LoadTalents in Character");
// #if CLIENT
#if CLIENT
			// if(GameMain == character)
			// {
				LuaCsLogger.Log("Character is client, requesting config");
				CrossClassSync.Instance.RequestCharacterConfig();
#endif
			// }
			// else
			// {
			// 	LuaCsLogger.Log("Character is NOT client. Skip.");
			// }
// #endif
		// if(GameMain.IsSingleplayer)
		// {
		// 	if(__instance.IsLocalPlayer)
		// 	{
		// 		LuaCsLogger.Log("RequestCharacterConfig in Character.LoadTalents");
		// 		CrossClassSync.Instance.RequestCharacterConfig();
		// 	}
		// }
		
	}
}