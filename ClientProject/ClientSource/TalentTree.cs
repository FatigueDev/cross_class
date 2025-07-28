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
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TalentTree), "IsViableTalentForCharacter", [typeof(Character), typeof(Identifier), typeof(IReadOnlyCollection<Identifier>)])]
		static bool IsViableTalentForCharacter(Character character, Identifier talentIdentifier, IReadOnlyCollection<Identifier> selectedTalents, TalentTree __instance, ref bool __result)
		{
			if (character?.Info?.Job.Prefab == null)
			{
				__result = false;
				return false;
			}

			if (character.Info.GetTotalTalentPoints() - selectedTalents.Count <= 0)
			{
				__result = false;
				return false;
			}

			var primaryTalentTree = TalentMenu_Patches.GetPrimaryTalentTree(character.Info);
			var selectedTalentTree = TalentMenu_Patches.GetSelectedTalentTree(character.Info);

			// if(!JobTalentTrees.Where(tt =>
			// 		TalentMenu_Patches.HasCrossClassTalentTree(character.Info, tt)
			// 	)
			// 	.Contains(
			// 		TalentMenu_Patches.GetSelectedTalentTree(character.Info)
			// 	))
			// {
			// 	JobTalentTrees.TryGet(character.Info.Job.Prefab.Identifier, out TalentTree? defaultTree);
			// 	if(selectedTalentTree != defaultTree)
			// 	{
			// 		__result = false;
			// 		return false;
			// 	}
			// }

			if (IsTalentLocked(talentIdentifier, Character.GetFriendlyCrew(character)))
			{
				__result = false;
				return false;
			}

			if(primaryTalentTree.Identifier == selectedTalentTree.Identifier)
			{
				if (character.Info.GetUnlockedTalentsInTree().Contains(talentIdentifier))
				{
					__result = true;
					return false;
				}
			}
			else
			{
				if(CharacterInfo_Utils.GetUnlockedTalentsInCrossClassTree(selectedTalentTree, character.Info).Contains(talentIdentifier))
				{
					__result = true;
					return false;
				}
				if(CharacterInfo_Utils.GetUnlockedTalentsOutsideTree(selectedTalentTree, character.Info).Contains(talentIdentifier))
				{
					__result = true;
					return false;
				}
			}

			ImmutableArray<TalentSubTree>.Enumerator enumerator = selectedTalentTree.TalentSubTrees.GetEnumerator();
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
							__result = TalentTreeMeetsRequirements(selectedTalentTree, current, selectedTalents);
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
	}
}