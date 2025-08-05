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
using static CrossClass.CrossClassHelpers;

namespace CrossClass
{
	[HarmonyPatch]
	public class TalentTree_Patches
	{
		// [HarmonyPrefix]
		// [HarmonyPatch(typeof(TalentTree), "IsViableTalentForCharacter", [typeof(Character), typeof(Identifier), typeof(IReadOnlyCollection<Identifier>)])]
		// static bool IsViableTalentForCharacter(Character character, Identifier talentIdentifier, IReadOnlyCollection<Identifier> selectedTalents, TalentTree __instance, ref bool __result)
		// {
		// 	if (character?.Info?.Job.Prefab == null)
		// 	{
		// 		__result = false;
		// 		return false;
		// 	}

		// 	if (character.Info.GetTotalTalentPoints() - selectedTalents.Count <= 0)
		// 	{
		// 		__result = false;
		// 		return false;
		// 	}

		// 	var primaryTalentTree = GetPrimaryTalentTree(character.Info);
		// 	var selectedTalentTree = GetSelectedTalentTree(character.Info);

		// 	if(primaryTalentTree != selectedTalentTree)
		// 	{
		// 		if(!HasCrossClassTalentTree(selectedTalentTree))
		// 		{
		// 			__result = false;
		// 			return false;
		// 		}
		// 	}

		// 	if (IsTalentLocked(talentIdentifier, Character.GetFriendlyCrew(character)))
		// 	{
		// 		__result = false;
		// 		return false;
		// 	}

		// 	if(primaryTalentTree.Identifier == selectedTalentTree.Identifier)
		// 	{
		// 		if (character.Info.GetUnlockedTalentsInTree().Contains(talentIdentifier))
		// 		{
		// 			__result = true;
		// 			return false;
		// 		}
		// 	}
		// 	else
		// 	{
		// 		if(CharacterInfo_Utils.GetUnlockedTalentsInCrossClassTree(selectedTalentTree, character.Info).Contains(talentIdentifier))
		// 		{
		// 			__result = true;
		// 			return false;
		// 		}
		// 		if(CharacterInfo_Utils.GetUnlockedTalentsOutsideTree(selectedTalentTree, character.Info).Contains(talentIdentifier))
		// 		{
		// 			__result = true;
		// 			return false;
		// 		}
		// 	}

		// 	ImmutableArray<TalentSubTree>.Enumerator enumerator = selectedTalentTree.TalentSubTrees.GetEnumerator();
		// 	while (enumerator.MoveNext())
		// 	{
		// 		TalentSubTree current = enumerator.Current;
		// 		if (current.AllTalentIdentifiers.Contains(talentIdentifier) && current.HasMaxTalents(selectedTalents))
		// 		{
		// 			__result = false;
		// 			return false;
		// 		}

		// 		ImmutableArray<TalentOption>.Enumerator enumerator2 = current.TalentOptionStages.GetEnumerator();
		// 		while (enumerator2.MoveNext())
		// 		{
		// 			TalentOption current2 = enumerator2.Current;
		// 			if (current2.TalentIdentifiers.Contains(talentIdentifier))
		// 			{
		// 				if (!current2.HasMaxTalents(selectedTalents))
		// 				{
		// 					__result = TalentTreeMeetsRequirements(GetSelectedTalentTree(character.Info), current, selectedTalents);
		// 					return false;
		// 				}

		// 				__result = false;
		// 				return false;
		// 			}

		// 			if (!current2.HasEnoughTalents(selectedTalents))
		// 			{
		// 				break;
		// 			}
		// 		}
		// 	}

		// 	__result = false;
		// 	return false;
		// }

		[HarmonyPrefix]
		[HarmonyPatch(typeof(TalentTree), "IsViableTalentForCharacter", [typeof(Character), typeof(Identifier), typeof(IReadOnlyCollection<Identifier>)])]
		static bool IsViableTalentForCharacter(Character character, Identifier talentIdentifier, IReadOnlyCollection<Identifier> selectedTalents, TalentTree __instance, ref bool __result)
		{
			// if(!CrossClassSync.Instance.Initialized) return true;

			if (character?.Info?.Job.Prefab == null) { __result = false; return false; }
			if (character.Info.GetTotalTalentPoints() - selectedTalents.Count <= 0) { __result = false; return false; }
			
			var primaryTalentTree = GetPrimaryTalentTree(character);
			if (primaryTalentTree is null) { __result = false; return false; }

			var selectedTalentTree = GetSelectedTalentTree(character);
			if(selectedTalentTree is null) { selectedTalentTree = primaryTalentTree; }

			if(primaryTalentTree != selectedTalentTree)
			{
				if(!HasCrossClassTalentTree(selectedTalentTree, character))
				{
					__result = false;
					return false;
				}
			}

			// Additions
			// if()
			// </ Additions

			if (IsTalentLocked(talentIdentifier, Character.GetFriendlyCrew(character))) { __result = false; return false; }

			if (character.Info.GetUnlockedTalentsInTree().Contains(talentIdentifier))
			{
				//if the character already has the talent, it must be viable?
				//needed for backwards compatibility, otherwise if we remove e.g. a tier 1 or tier 2 talent,
				//all the already-unlocked higher-tier talents will be considered invalid which'll break the talent selection
				__result = true;
				return false;
			}

			// if(character.Info.GetUnlockedTalentsOutsideTree().Contains(talentIdentifier))
			// {
			// 	__result = true;
			// 	return false;
			// }

			// foreach(TalentTree crossClassTree in GetCrossClassTalentTrees())
			// {
			// 	if(character.Info.UnlockedTalents.Where(crossClassTree.TalentIsInTree).Contains(talentIdentifier))
			// 	{
			// 		__result = true;
			// 		return false;
			// 	}
			// 	else if(character.Info.UnlockedTalents.Where(tt => !crossClassTree.TalentIsInTree(tt)).Contains(talentIdentifier))
			// 	{
			// 		__result = true;
			// 		return false;
			// 	}
			// }

			foreach (var subTree in selectedTalentTree!.TalentSubTrees)
			{
				if (subTree.AllTalentIdentifiers.Contains(talentIdentifier) && subTree.HasMaxTalents(selectedTalents)) { return false; }

				foreach (var talentOptionStage in subTree.TalentOptionStages)
				{
					if (talentOptionStage.TalentIdentifiers.Contains(talentIdentifier))
					{
						__result = !talentOptionStage.HasMaxTalents(selectedTalents) && TalentTreeMeetsRequirements(selectedTalentTree, subTree, selectedTalents);
						return false;
					}
					//if a previous stage hasn't been completed, this talent can't be selected yet
					bool optionStageCompleted = talentOptionStage.HasEnoughTalents(selectedTalents);
					if (!optionStageCompleted)
					{
						break;
					}                    
				}
			}

			__result = false;
			return false;
		}

		/*

		if (character?.Info?.Job.Prefab == null) { return false; }
		if (character.Info.GetTotalTalentPoints() - selectedTalents.Count <= 0) { return false; }
		if (!JobTalentTrees.TryGet(character.Info.Job.Prefab.Identifier, out TalentTree talentTree)) { return false; }

		if (IsTalentLocked(talentIdentifier, Character.GetFriendlyCrew(character))) { return false; }

		if (character.Info.GetUnlockedTalentsInTree().Contains(talentIdentifier))
		{
			//if the character already has the talent, it must be viable?
			//needed for backwards compatibility, otherwise if we remove e.g. a tier 1 or tier 2 talent,
			//all the already-unlocked higher-tier talents will be considered invalid which'll break the talent selection
			return true;
		}

		foreach (var subTree in talentTree!.TalentSubTrees)
		{
			if (subTree.AllTalentIdentifiers.Contains(talentIdentifier) && subTree.HasMaxTalents(selectedTalents)) { return false; }

			foreach (var talentOptionStage in subTree.TalentOptionStages)
			{
				if (talentOptionStage.TalentIdentifiers.Contains(talentIdentifier))
				{
					return !talentOptionStage.HasMaxTalents(selectedTalents) && TalentTreeMeetsRequirements(talentTree, subTree, selectedTalents);
				}
				//if a previous stage hasn't been completed, this talent can't be selected yet
				bool optionStageCompleted = talentOptionStage.HasEnoughTalents(selectedTalents);
				if (!optionStageCompleted)
				{
					break;
				}                    
			}
		}

		return false;

		*/

		[HarmonyPrefix]
        [HarmonyPatch(typeof(TalentTree), "CheckTalentSelection")]
        public static bool CheckTalentSelection(Character controlledCharacter, IEnumerable<Identifier> selectedTalents, TalentTree __instance, ref List<Identifier> __result)
        {
			// if(!CrossClassSync.Instance.Initialized) return true;

            List<Identifier> viableTalents = new List<Identifier>();
            bool canStillUnlock = true;
            // keep trying to unlock talents until none of the talents are unlockable
            while (canStillUnlock && selectedTalents.Any())
            {
                canStillUnlock = false;
                foreach (Identifier talent in selectedTalents)
                {
					var viableForCharacter = false;
					IsViableTalentForCharacter(controlledCharacter, talent, viableTalents, __instance, ref viableForCharacter);
                    if (!viableTalents.Contains(talent) && viableForCharacter)
                    {
                        viableTalents.Add(talent);
                        canStillUnlock = true;
                    }
                }
            }

            __result = viableTalents;
			return false;
        }

	// #endif
	}
}