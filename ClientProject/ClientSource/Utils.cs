using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using static Barotrauma.TalentTree;

namespace CrossClass;

/// <summary>
/// Client
/// </summary>
public static partial class NetUtil
{
	internal static void SendServer(IWriteMessage outMsg, DeliveryMethod deliveryMethod = DeliveryMethod.Reliable)
	{
		if (GameMain.IsSingleplayer) return;
		GameMain.LuaCs.Networking.Send(outMsg, deliveryMethod);
	}
}

public static class CharacterInfoUtil
{
		// public static IEnumerable<Identifier> GetUnlockedTalentsInCrossClassTree(IEnumerable<Identifier> existingUnlocks, TalentTree? crossClassTalentTree, CharacterInfo info)
		// {
		// 	if (crossClassTalentTree is null)
		// 	{
		// 		return existingUnlocks;
		// 	}

		// 	List<Identifier> resultList = [];
		// 	crossClassTalentTree.AllTalentIdentifiers.ForEach(ct =>
		// 	{
		// 		resultList.AddRange(info.UnlockedTalents.Where(t => ct.TalentIsInTree(t)));
		// 	});

		// 	__result = resultList.Distinct();

		// 	return existingUnlocks.Where((Identifier t) => crossClassTalentTree.TalentIsInTree(t));
		// }

		// public static IEnumerable<Identifier> GetUnlockedTalentsOutsideCrossClassTree(TalentTree? crossClassTalentTree, CharacterInfo info)
		// {
		// 	if (crossClassTalentTree is null)
		// 	{
		// 		return Enumerable.Empty<Identifier>();
		// 	}

		// 	return info.UnlockedTalents.Where((Identifier t) => !crossClassTalentTree.TalentIsInTree(t));
		// }
}