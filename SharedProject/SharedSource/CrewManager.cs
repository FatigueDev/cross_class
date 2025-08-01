

using Barotrauma;
using Barotrauma.Networking;
using HarmonyLib;

namespace CrossClass;

// [HarmonyPatch]
// public class CrewManager_Patches
// {
	// [HarmonyPostfix]
	// [HarmonyPatch(typeof(CrewManager), "AddCharacter")]
	// static void InitializeCharacter(Character character, WayPoint mainSubWaypoint, WayPoint spawnWaypoint)
	// {
// #if CLIENT
		// LuaCsLogger.Log("Called AddCharacter");
// #endif
// #if CLIENT
		// CrossClassSync.Instance.RequestCharacterConfig();
// #endif
	// }
// }