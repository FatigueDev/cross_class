using Barotrauma;
using Barotrauma.Networking;
using HarmonyLib;
using static Barotrauma.TalentTree;

namespace CrossClass;

/// <summary>
/// Client
/// </summary>
partial class CrossClassSync : Singleton<CrossClassSync>
{
	private void SetupClient()
	{
		// CommandUtils.AddCommand(
		// 	"mlc_config",
		// 	"Toggle the display of the config editor",
		// 	ToggleGUI);
		// Exit if we're in an editor
		if (Screen.Selected.IsEditor) return;
		if (GameMain.IsSingleplayer) return;

		NetUtil.Register(NetEvent.CHARACTER_WRITE_SERVER, ClientCharacterWrite);
		NetUtil.Register(NetEvent.CHARACTER_WRITE_CLIENT, ClientCharacterRead);
		// RequestCharacterConfig();

		// CrossClass.Hook("roundStart", "cross_class.client.roundStart", (args) => {
		// 	IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_REQUEST_SERVER);
		// 	NetUtil.SendServer(outMsg);
		// 	return null;
		// });


// 		CrossClass.Hook("roundStart", "cross_class.client.roundStart", (args) =>
// 		{
// #if CLIENT
// 			// IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_REQUEST);
// 			// GameMain.LuaCs.Networking.Send(outMsg);
// 			return null;
// #else
// 			return null;
// #endif
// 		});
	}

	// public partial void RequestCharacterConfig()
	// {
	// 	IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_REQUEST);
	// 	GameMain.LuaCs.Networking.Send(outMsg);
	// }

	public void ClientCharacterWrite(object[] args)
	{
		IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_SERVER);
		WriteCharacterConfig(ref outMsg);
		GameMain.LuaCs.Networking.Send(outMsg);
	}

	private void ClientCharacterRead(object[] args)
	{
		IReadMessage inMsg = (IReadMessage)args[0];
		ReadNetCharacterConfig(ref inMsg);
		// Initialized = true;
	}

	public partial void UpdateConfig()
	{
		ClientCharacterWrite([]);
	}
}