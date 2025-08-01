using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
	[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterInfo), "GetUnlockedTalentsOutsideTree")]
	public static bool GetUnlockedTalentsOutsideTree(ref IEnumerable<Identifier> __result, ref CharacterInfo __instance)
	{
		// if(!CrossClassSync.Instance.Initialized) return true;

		var allUnlockedTalentTrees = GetCrossClassTalentTrees().Prepend(GetPrimaryTalentTree(__instance));
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

		var allUnlockedTalentTrees = GetCrossClassTalentTrees().Prepend(GetPrimaryTalentTree(__instance));
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