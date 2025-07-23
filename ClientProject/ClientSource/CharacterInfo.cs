using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Barotrauma;
using Barotrauma.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using static Barotrauma.TalentTree;
using static Barotrauma.TalentTree.TalentStages;

namespace CrossClass
{
	[HarmonyPatch]
	public class CharacterInfo_Patches
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CharacterInfo), "GetUnlockedTalentsInTree")]
		static bool GetUnlockedTalentsInTree(CharacterInfo __instance, ref IEnumerable<Identifier> __result)
		{
			// List<Identifier> resultList = new List<Identifier>();
			// List<TalentTree?> talentTrees = new();
			// foreach(TalentTree tree in JobTalentTrees)
			// {
			// 	if(__instance.GetSavedStatValue(StatTypes.None, $"cross_class.{tree.Identifier}") == 1.0f)
			// 	{
					// resultList.AddRange(__instance.UnlockedTalents.Where(tree.TalentIsInTree));
			// 		talentTrees.Add(tree);
			// 	}
			// }

			// if (!TalentTree.JobTalentTrees.TryGet(__instance.Job.Prefab.Identifier, out TalentTree? talentTree))
			// {
			// 	__result = Enumerable.Empty<Identifier>();
			// 	return false;
			// }

			// __result = resultList; //__instance.UnlockedTalents.Where((Identifier t) => talentTree.TalentIsInTree(t));

			List<Identifier> resultList = new List<Identifier>();

			resultList.AddRange(__instance.UnlockedTalents.Where(t => TalentMenu_Patches.primaryTalentTree!.TalentIsInTree(t)));
			var crossClassTalentTrees = JobTalentTrees.Where(tt => TalentMenu_Patches.HasCrossClassTalentTree(__instance, tt));
			crossClassTalentTrees.ForEach(ct =>
			{
				resultList.AddRange(__instance.UnlockedTalents.Where(t => ct.TalentIsInTree(t)));
			});

			__result = resultList.Distinct(); //__instance.UnlockedTalents.Where((Identifier t) => TalentMenu_Patches.selectedTalentTree!.TalentIsInTree(t));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(CharacterInfo), "GetUnlockedTalentsOutsideTree")]
		static bool GetUnlockedTalentsOutsideTree(CharacterInfo __instance, ref IEnumerable<Identifier> __result)
		{
			// List<Identifier> resultList = new List<Identifier>();
			// foreach(TalentTree tree in JobTalentTrees)
			// {
			// 	if(__instance.GetSavedStatValue(StatTypes.None, $"cross_class.{tree.Identifier}") == 1.0f)
			// 	{
			// 		LuaCsLogger.LogMessage($"Cross class for cross_class.{tree.Identifier} found!");
			// 		resultList.AddRange(__instance.UnlockedTalents.Where((Identifier t) => !tree.TalentIsInTree(t)));
			// 	}
			// }
			// __result = resultList;

			// if (!TalentTree.JobTalentTrees.TryGet(__instance.Job.Prefab.Identifier, out TalentTree? talentTree))
			// {
			// 	__result = Enumerable.Empty<Identifier>();
			// 	return false;
			// }

			List<Identifier> resultList = new List<Identifier>();

			resultList.AddRange(__instance.UnlockedTalents.Where(t => !TalentMenu_Patches.primaryTalentTree!.TalentIsInTree(t)));
			
			var crossClassTalentTrees = JobTalentTrees.Where(tt => TalentMenu_Patches.HasCrossClassTalentTree(__instance, tt));
			crossClassTalentTrees.ForEach(ct =>
			{
				resultList.AddRange(__instance.UnlockedTalents.Where(t => !ct.TalentIsInTree(t)));
			});

			__result = resultList.Distinct();
			return false;
		}
	}
}