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
	public class TalentTree_Patches
	{
		// [HarmonyPrefix]
		// [HarmonyPatch(typeof(TalentTree), "IsViableTalentForCharacter", [typeof(Character), typeof(Identifier)])]
		// static bool IsViableTalentForCharacter(Character character, Identifier talentIdentifier, TalentTree __instance, ref bool __result)
		// {
		// 	LuaCsLogger.LogMessage("IsViableTalentForCharacter (1)");
		// 	return true;
		// }

		[HarmonyPrefix]
		[HarmonyPatch(typeof(TalentTree), "IsViableTalentForCharacter", [typeof(Character), typeof(Identifier), typeof(IReadOnlyCollection<Identifier>)])]
		static bool IsViableTalentForCharacter(Character character, Identifier talentIdentifier, IReadOnlyCollection<Identifier> selectedTalents, TalentTree __instance, ref bool __result)
		{
			if (character?.Info?.Job.Prefab == null)
			{
				__result = false;
				return false;
			}

			// var some_stat = character.info.GetSavedStatValue(StatTypes.None, "some_stat");
			// LuaCsLogger.LogMessage($"pre some_stat: {some_stat}");
			// character.info.ChangeSavedStatValue(StatTypes.None, 1.0f, "some_stat", false, 1.0f, true);
			// some_stat = character.info.GetSavedStatValue(StatTypes.None, "some_stat");
			// LuaCsLogger.LogMessage($"post some_stat: {some_stat}");

			if (character.Info.GetTotalTalentPoints() - selectedTalents.Count <= 0)
			{
				__result = false;
				return false;
			}

			if(!JobTalentTrees.Where(tt =>
					TalentMenu_Patches.HasCrossClassTalentTree(character.Info, tt)
				)
				.Contains(
					TalentMenu_Patches.GetSelectedTalentTree(character.Info)
				))
			{
				JobTalentTrees.TryGet(character.Info.Job.Prefab.Identifier, out TalentTree? defaultTree);
				if(TalentMenu_Patches.GetSelectedTalentTree(character.Info) != defaultTree)
				{
					__result = false;
					return false;
				}
			}

			// if (!JobTalentTrees.TryGet(character.Info.Job.Prefab.Identifier, out TalentTree? result))
			// {
			// 	__result = false;
			// 	return false;
			// }

			// var otherTreeList = JobTalentTrees.Where(t => t.AllTalentIdentifiers.Contains(talentIdentifier)).ToImmutableList();
			// TalentTree? crossClassTree = null;
			// foreach(var tree in otherTreeList)
			// {
			// 	if(character.Info.GetSavedStatValue(StatTypes.None, $"cross_class.{tree.Identifier}") == 1.0f)
			// 	{
			// 		LuaCsLogger.LogMessage($"Cross class for cross_class.{tree.Identifier} found!");
			// 		result = tree;
			// 	}
			// }

			if (IsTalentLocked(talentIdentifier, Character.GetFriendlyCrew(character)))
			{
				__result = false;
				return false;
			}

			if (character.Info.GetUnlockedTalentsInTree().Contains(talentIdentifier))
			{
				__result = true;
				return false;
			}

			ImmutableArray<TalentSubTree>.Enumerator enumerator = TalentMenu_Patches.selectedTalentTree!.TalentSubTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TalentSubTree current = enumerator.Current;
				if (current.AllTalentIdentifiers.Contains(talentIdentifier) && current.HasMaxTalents(selectedTalents))
				{
					__result = false;
					return false;
				}

				ImmutableArray<TalentOption>.Enumerator enumerator2 = current.TalentOptionStages.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TalentOption current2 = enumerator2.Current;
					if (current2.TalentIdentifiers.Contains(talentIdentifier))
					{
						if (!current2.HasMaxTalents(selectedTalents))
						{
							__result = TalentTreeMeetsRequirements(TalentMenu_Patches.selectedTalentTree, current, selectedTalents);
							return false;
						}

						__result = false;
						return false;
					}

					if (!current2.HasEnoughTalents(selectedTalents))
					{
						break;
					}
				}
			}

			__result = false;
			return false;
		}
	
		[HarmonyPrefix]
        [HarmonyPatch(typeof(TalentTree), "CheckTalentSelection")]
        public static bool CheckTalentSelection(Character controlledCharacter, IEnumerable<Identifier> selectedTalents, TalentTree __instance, ref List<Identifier> __result)
        {
            List<Identifier> list = new List<Identifier>();
            bool flag = true;
            while (flag && selectedTalents.Any())
            {
                flag = false;
                foreach (Identifier selectedTalent in selectedTalents)
                {
                    if (!list.Contains(selectedTalent) && TalentTree.IsViableTalentForCharacter(controlledCharacter, selectedTalent, list))
                    {
                        list.Add(selectedTalent);
                        flag = true;
                    }
                }
            }
			__result = list;
            return false;
        }

		// [HarmonyPrefix]
        // [HarmonyPatch(typeof(TalentTree), "TalentIsInTree")]
		// public bool TalentIsInTree(Identifier talentIdentifier, TalentTree __instance, ref bool __result)
		// {
		// 	__result = true;
		// 	return false;
		// }
	}
}