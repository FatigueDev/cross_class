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