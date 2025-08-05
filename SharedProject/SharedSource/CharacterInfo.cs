using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Barotrauma;
using Barotrauma.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using static Barotrauma.TalentTree;
using static Barotrauma.TalentTree.TalentStages;
using static CrossClass.CrossClassHelpers;


namespace CrossClass;

[HarmonyPatch]
public class CharacterInfo_Shared_Patches
{

	// [HarmonyPostfix]
	// [HarmonyPatch(typeof(CharacterInfo), nameof(CharacterInfo.Save))]
	// static void Save(XElement parentElement, ref XElement __result)
	// {
// #if SERVER
// 		if(CrossClassSync.Instance.ShouldSave)
// 		{
// 			LuaCsLogger.Log("Save character info has been called from the harmony monkey patch.");
// 			CrossClassSync.Instance.SaveCharacterConfigs();
// 			LuaCsLogger.Log("Ran SaveCharacterConfigs.");
// 			CrossClassSync.Instance.ShouldSave = false;
// 		}
// 		else
// 		{
// 			// LuaCsLogger.Log("CrossClass instance config shouldn't save; either because of a fresh game or because nobody used it during the round. Skipping save.");
// 			return;
// 		}
// #endif
		// CrossClassSync.Instance.Setup();
	// }

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterInfo), "GetUnlockedTalentsOutsideTree")]
	public static bool GetUnlockedTalentsOutsideTree(ref IEnumerable<Identifier> __result, ref CharacterInfo __instance)
	{
		// if(!CrossClassSync.Instance.Initialized) return true;

		var allUnlockedTalentTrees = GetCrossClassTalentTrees(__instance.Character).Prepend(GetPrimaryTalentTree(__instance.Character));
		__result = Enumerable.Empty<Identifier>();

		foreach(var talentTree in allUnlockedTalentTrees)
		{
			__result = __result.Concat(__instance.UnlockedTalents.Where(t => !talentTree.TalentIsInTree(t))).Distinct();
		}

		return false;

		// if(GetSelectedTalentTree() is TalentTree selectedTree)
		// {
		// 	__result = __instance.UnlockedTalents.Where(t => !selectedTree.TalentIsInTree(t));
		// 	return false;
		// }
		// else
		// {
		// 	__result = Enumerable.Empty<Identifier>();
		// 	return false;
		// }
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterInfo), "GetUnlockedTalentsInTree")]
	public static bool GetUnlockedTalentsInTree(ref IEnumerable<Identifier> __result, ref CharacterInfo __instance)
	{
		// if(!CrossClassSync.Instance.Initialized) return true;

		var allUnlockedTalentTrees = GetCrossClassTalentTrees(__instance.Character).Prepend(GetPrimaryTalentTree(__instance.Character));
		__result = Enumerable.Empty<Identifier>();

		foreach(var talentTree in allUnlockedTalentTrees)
		{
			__result = __result.Concat(__instance.UnlockedTalents.Where(talentTree.TalentIsInTree)).Distinct();
		}
		
		return false;
		// if(GetSelectedTalentTree() is TalentTree selectedTree)
		// {
		// 	__result = __instance.UnlockedTalents.Where(selectedTree.TalentIsInTree);
		// 	return false;
		// }
		// else
		// {
		// 	__result = Enumerable.Empty<Identifier>();
		// 	return false;
		// }
	}
}