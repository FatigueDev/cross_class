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

		// NetUtil.Register(NetEvent.CAMPAIGN_WRITE_CLIENT, ClientCampaignRead);
		NetUtil.Register(NetEvent.CHARACTER_WRITE_CLIENT, ClientCharacterRead);
		// RequestCharacterConfig();

		bool requestedConfig = false;

		CrossClass.Hook("think", "cross_class.client.think", (args) =>
		{
			if(CrossClass.IsRunning && CrossClass.IsCampaign == false)
            {
                LuaCsLogger.LogError("CrossClass doesn't work in games that aren't campaigns.");
                GameMain.LuaCs.Hook.Remove("think", "cross_class.client.think");    
                return null;
            }

			if(CrossClass.IsCampaign && GameMain.Client.GameStarted && !requestedConfig)
			{
				Instance.RequestCharacterConfig();
				requestedConfig = true;
				return null;
			}

			if(Instance.Initialized)
			{
				LuaCsLogger.Log("Client has received their character configuration.");
				GameMain.LuaCs.Hook.Remove("think", "cross_class.client.think");
				return null;
			}

			return null;
		});
	}

	public partial void RequestCharacterConfig()
	{
		IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_REQUEST);
		// outMsg.WriteString(Main.Version);
		// outMsg.WriteUInt16(GameMain.Client.MyClient.CharacterID);
		GameMain.LuaCs.Networking.Send(outMsg);
		// LuaCsLogger.Log("Requested character from server...");
	}

	public void ClientCharacterWrite()
	{
		// Always allow the server owner to write
		// if (!GameMain.Client.HasPermission(ClientPermissions.ManageSettings) && !GameMain.Client.IsServerOwner)
		// {
		// 	LuaCsLogger.Log("No Perms!");
		// 	return;
		// }
		IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_SERVER);
		// outMsg.WriteString(Main.Version);
		// outMsg.WriteUInt16(GameMain.Client.MyClient.CharacterID);
		WriteCharacterConfig(ref outMsg);
		GameMain.LuaCs.Networking.Send(outMsg);
		// LuaCsLogger.Log("Sent character packet to server!");
	}

	private void ClientCharacterRead(object[] args)
	{
		// LuaCsLogger.Log("Got character packet!");
		IReadMessage inMsg = (IReadMessage)args[0];
		ReadNetCharacterConfig(ref inMsg);
		Instance.Initialized = true;
		// SaveCampaign();
		// SaveCharacter();
	}

	public partial void UpdateConfig()
	{
		ClientCharacterWrite();
	}

	



}