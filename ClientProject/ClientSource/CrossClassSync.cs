using Barotrauma;
using Barotrauma.Networking;

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
		// if (GameMain.IsSingleplayer) return; // We don't need to do any of this if we're in singleplayer

		// NetUtil.Register(NetEvent.CAMPAIGN_WRITE_CLIENT, ClientCampaignRead);
		NetUtil.Register(NetEvent.CHARACTER_WRITE_CLIENT, ClientCharacterRead);

		LuaCsLogger.Log("[CL] CrossClass sync initialized");

		// CrossClass.Hook("roundStart", "cross_class.client.roundStart", (args) =>
		// {
		// 	Campaign = GameMain.GameSession.Campaign;

		// 	var campaignHashCode = StringSha256Hash(Campaign.Name.ToString());//.CampaignSettings.Name.ToIdentifier();
		// 	characterSaveLocation = Path.Join(configFolder, campaignHashCode, GameMain.Client.Character.ID.ToString(), characterFile);
		// 	// LoadConfig();
		// 	LuaCsLogger.Log("[CL] Setting up local player on the client for Cross Class");
		// 	if (!GameMain.Client.IsServerOwner)
		// 	{
		// 		RequestCampaignConfig();
		// 		RequestCharacterConfig();
		// 	}
		// 	// else
		// 	// {
		// 	// 	// LoadConfig();
		// 	// 	// UpdateConfig();
		// 	// 	RequestCampaignConfig();
		// 	// 	RequestCharacterConfig();
		// 	// 	// CrossClass.Hook("client.connected", "cross_class.shared.client_connected", (args) =>
		// 	// 	// {
		// 	// 	// 	if (args.ElementAt(0) is Client client)
		// 	// 	// 	{
		// 	// 	// 		ClientWrite();
		// 	// 	// 	}
		// 	// 	// 	return null;
		// 	// 	// });
		// 	// }
		// 	return null;
		// });

		// if(CrossClass.IsRunning)
		// {
		// 	Campaign = GameMain.GameSession.Campaign;

		// 	var campaignHashCode = StringSha256Hash(Campaign.Name.ToString());//.CampaignSettings.Name.ToIdentifier();
		// 	characterSaveLocation = Path.Join(configFolder, campaignHashCode, GameMain.Client.Character.ID.ToString(), characterFile);

		// 	// LoadConfig();
		// 	LuaCsLogger.Log("Setting up local player on the client for Cross Class");
		// 	if (!GameMain.Client.IsServerOwner)
		// 	{
		// 		RequestCampaignConfig();
		// 		RequestCharacterConfig();
		// 	}
		// }
	}

	// public void SetConfig(CampaignConfig campaignConfig)
	// {
	// 	// CampaignConfig = campaignConfig;
	// 	LuaCsLogger.Log("[CLIENT] Config Updated");
	// 	// LuaCsLogger.Log(Config.ToString());

	// 	// this.CampaignConfigSaveLocation = GameMain.GameSession.Campaign!.ToIdentifier().ToString();

	// 	// if (!GameMain.IsSingleplayer)
	// 	// {
	// 		// UpdateConfig();
	// 	// }
	// 	// else
	// 	// {
	// 	// 	SaveCampaign();
	// 	// 	SaveCharacter();
	// 	// }
	// }

	// public void RequestCampaignConfig()
	// {
	// 	IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CAMPAIGN_REQUEST);
	// 	// outMsg.WriteString(Main.Version);
	// 	GameMain.LuaCs.Networking.Send(outMsg);
	// 	LuaCsLogger.Log("Requested campaign from server...");
	// }

	public void RequestCharacterConfig()
	{
		IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_REQUEST);
		// outMsg.WriteString(Main.Version);
		GameMain.LuaCs.Networking.Send(outMsg);
		LuaCsLogger.Log("Requested character from server...");
	}

	// private void ClientCampaignWrite()
	// {
	// 	// Always allow the server owner to write
	// 	// if (!GameMain.Client.HasPermission(ClientPermissions.ManageSettings) && !GameMain.Client.IsServerOwner)
	// 	// {
	// 	// 	LuaCsLogger.Log("No Perms!");
	// 	// 	return;
	// 	// }
	// 	IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CAMPAIGN_WRITE_SERVER);
	// 	// outMsg.WriteString(Main.Version);
	// 	WriteCampaignConfig(ref outMsg);
	// 	GameMain.LuaCs.Networking.Send(outMsg);
	// 	LuaCsLogger.Log("Sent campaign packet to server!");
	// }

	private void ClientCharacterWrite()
	{
		// Always allow the server owner to write
		// if (!GameMain.Client.HasPermission(ClientPermissions.ManageSettings) && !GameMain.Client.IsServerOwner)
		// {
		// 	LuaCsLogger.Log("No Perms!");
		// 	return;
		// }
		IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_SERVER);
		// outMsg.WriteString(Main.Version);
		WriteCharacterConfig(ref outMsg);
		GameMain.LuaCs.Networking.Send(outMsg);
		LuaCsLogger.Log("Sent character packet to server!");
	}

	// private void ClientCampaignRead(object[] args)
	// {
	// 	LuaCsLogger.Log("Got campaign packet!");
	// 	IReadMessage inMsg = (IReadMessage)args[0];
	// 	ReadNetCampaignConfig(ref inMsg);
	// 	// SaveCampaign();
	// 	// SaveCharacter();
	// }

	private void ClientCharacterRead(object[] args)
	{
		LuaCsLogger.Log("Got character packet!");
		IReadMessage inMsg = (IReadMessage)args[0];
		ReadNetCharacterConfig(ref inMsg);
		// SaveCampaign();
		// SaveCharacter();
	}

	public void UpdateConfig()
	{
		// if (!GameMain.Client.HasPermission(ClientPermissions.ManageSettings)) return;
		ClientCharacterWrite();
	}

	// private void DisplayPatchNotes(bool force = false)
	// {
	// 	// REMEMBER TO CHANGE THIS BACK
	// 	if (Config.Version != Main.Version || force || Main.IsNightly)
	// 	{
	// 		ShouldDisplayPatchNotes = false;
	// 	}
	// }

	// public bool SettingsOpen
	// {
	// 	get => _settingsOpen;
	// 	set
	// 	{
	// 		if (value == _settingsOpen) { return; }

	// 		if (value)
	// 		{

	// 			_settingsMenu = new GUIFrame(new RectTransform(Vector2.One, Screen.Selected.Frame.RectTransform, Anchor.Center), style: null);
	// 			_ = new GUIFrame(new RectTransform(GUI.Canvas.RelativeSize, _settingsMenu.RectTransform, Anchor.Center), style: "GUIBackgroundBlocker");

	// 			var settingsMenuInner = new GUIFrame(new RectTransform(new Vector2(1.0f, 0.8f), _settingsMenu.RectTransform, Anchor.Center, scaleBasis: ScaleBasis.Smallest) { MinSize = new Point(640, 480) });
	// 			_ = ConfigMenu.Create(settingsMenuInner.RectTransform);
	// 			Log.Verbose("Opened Settings");
	// 		}
	// 		else
	// 		{
	// 			ConfigMenu.Instance?.Close();
	// 			_settingsMenu.Parent.RemoveChild(_settingsMenu);
	// 			_settingsMenu = null;
	// 			Log.Verbose("Closed Settings");
	// 		}
	// 		_settingsOpen = value;
	// 	}
	// }
	// private static bool _settingsOpen;
	// private static GUIFrame _settingsMenu;
	// private void ToggleGUI(object[] args) => SettingsOpen = !SettingsOpen;
}