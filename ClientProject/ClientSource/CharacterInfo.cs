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
	public class CharacterInfo_Utils
	{
		public static IEnumerable<Identifier> GetUnlockedTalentsInCrossClassTree(TalentTree? crossClassTalentTree, CharacterInfo info)
		{
			if (crossClassTalentTree is null)
			{
				return Enumerable.Empty<Identifier>();
			}

			return info.UnlockedTalents.Where((Identifier t) => crossClassTalentTree.TalentIsInTree(t));
		}

		public static IEnumerable<Identifier> GetUnlockedTalentsOutsideTree(TalentTree? crossClassTalentTree, CharacterInfo info)
		{
			if (crossClassTalentTree is null)
			{
				return Enumerable.Empty<Identifier>();
			}

			return info.UnlockedTalents.Where((Identifier t) => !crossClassTalentTree.TalentIsInTree(t));
		}
	}
}