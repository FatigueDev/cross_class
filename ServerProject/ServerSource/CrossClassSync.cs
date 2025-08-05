using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Barotrauma;
using Barotrauma.Networking;
// using Newtonsoft.Json;


namespace CrossClass;

/// <summary>
/// Server
/// </summary>
partial class CrossClassSync : Singleton<CrossClassSync>
{
    public readonly string ConfigFolder = $"{ACsMod.GetStoreFolder<CrossClass>()}";
	public string CampaignRoot = string.Empty;
	public string CharacterSavePath = string.Empty;

    private void SetupServer()
    {
        NetUtil.Register(NetEvent.CHARACTER_WRITE_SERVER, ServerRead_Character);
        NetUtil.Register(NetEvent.CHARACTER_REQUEST_SERVER, ServerUpdate_Character);

        // NetUtil.Register(NetEvent.CHARACTER_REQUEST, CharacterRequest);
        
        // CrossClass.Hook("think", "cross_class.server.think", (args) => {

        //     if(CrossClass.IsRunning == false && Initialized == true)
        //     {
        //         Initialized = false;
        //     }
        //     else if(CrossClass.IsRunning && Initialized == false)
        //     {
        //         if(CrossClass.IsCampaign == false)
        //         {
        //             LuaCsLogger.LogError("CrossClass doesn't work in games that aren't campaigns.");
        //         }
        //         else
        //         {
        //             LuaCsLogger.Log("[SV] CrossClass sync initialized");
        //             LoadConfig();
        //         }
        //     }
        //     return null;
        // });

        // LuaCsPatch luaCsPatch = new LuaCsPatch((instance, args) =>
        // {
        //     Instance.SaveCharacterConfigs();
        //     return null;
        // });

        // GameMain.LuaCs.Hook.HookMethod("MultiPlayerCampaign", "SavePlayers", luaCsPatch, LuaCsHook.HookMethodType.After);

        // LoadConfig();

        // CrossClass.Hook("think", "cross_class.server.think", (args) => {
        //     if(CrossClass.IsCampaign == false)
        //     {
        //         // LuaCsLogger.LogError("CrossClass doesn't work in games that aren't campaigns.");
        //         GameMain.LuaCs.Hook.Remove("think", "cross_class.server.think");
        //         return null;
        //     }

        //     if(!CrossClass.IsRunning && CrossClassSync.Instance.Initialized)
        //     {
        //         CrossClassSync.Instance.Initialized = false;
        //         return null;
        //     }

        //     if(CrossClass.IsRunning && !CrossClassSync.Instance.Initialized)
        //     {
        //         LuaCsLogger.Log("[SV] CrossClass sync initialized");
        //         LoadConfig();
        //         return null;
        //     }
            
        //     // GameMain.LuaCs.Hook.Remove("think", "cross_class.server.think");
        //     return null;
        // });

        CrossClass.Hook("roundStart", "cross_class.server.roundStart", (args) =>
        {
            // LuaCsLogger.LogMessage("Server round start");

            // if(ShouldSave)
            // {
            //     SaveCharacterConfigs();
            // }

            LoadConfig();
            LoadCharacterConfigs();

            LuaCsLogger.Log("[SV] CrossClass sync initialized");

            if(CharacterConfig.Any())
            {
                LuaCsLogger.Log("[SV] CrossClass sync sending configurations to clients");
                ServerWrite_Character();
            }

            return null;
        });

        CrossClass.Hook("roundEnd", "cross_class.server.roundEnd", (args) => {
            if(Campaign?.GetAvailableTransition() == CampaignMode.TransitionType.None)
            {
                return null;
            }
            
            bool success = GameMain.Server.ConnectedClients.Any(c => c.InGame && c.Character != null && !c.Character.IsDead);
            
            if(success && ShouldSave)
            {
                SaveCharacterConfigs();
            }
            return null;
        });

        CrossClass.Hook("client.connected", "cross_class.server.client_connected", (args) => 
        {
            Client c = (Client)args[0];
            IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
            WriteCharacterConfig(ref outMsg);
            NetUtil.SendClient(outMsg, c.Connection);
            return null;
        });

        // CrossClass.Hook("roundEnd")
    }

    public void LoadConfig()
	{
		if (!CrossClass.IsCampaign)
		{
			// LuaCsLogger.LogMessage("Tried to load config, but the game mode was not Campaign so we're skipping.");
			return;
		}

		try
		{
			// LuaCsLogger.LogMessage("Setting campaign...");
			Campaign = GameMain.GameSession.Campaign;

			// Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath);

			var campaignName = Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath).Replace(" ", "_").Trim();

			CampaignRoot = Path.Join(ConfigFolder, campaignName);

			// LuaCsLogger.LogMessage("Creating campaign directory...");
			if(!LuaCsFile.DirectoryExists(CampaignRoot))
			{
				LuaCsFile.CreateDirectory(CampaignRoot);
			}

			// LuaCsLogger.LogMessage("Directory is fine.");

			CharacterSavePath = Path.Join(CampaignRoot, "character_data.json");
			// LuaCsLogger.LogMessage($"Path join is fine. Path: {CharacterSavePath}");

			// if(LuaCsFile.Exists(CharacterSavePath))
            // {
                // LuaCsLogger.Log("Has file at path.");
                // var characterAsJson = LuaCsFile.Read(CharacterSavePath);
                // LuaCsLogger.Log("Character read as json");
                // JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions{
                //     AllowTrailingCommas = true,
                //     WriteIndented = true,
                //     PropertyNameCaseInsensitive = true
                // };
                // CharacterConfig = JsonSerializer.Deserialize<Dictionary<string, CharacterConfigData>>(characterAsJson, JsonSerializerOptions) ?? [];
                // LuaCsLogger.Log("CharacterConfig is fine");

            // }
            // else
            // {
            //     LuaCsLogger.Log("No file at path.");
            //     CharacterConfig = new Dictionary<string, CharacterConfigData>();
            //     LuaCsLogger.Log("CharacterConfig is a new Dictionary<string, CharacterConfig>");
            // }

            Initialized = true;
			LuaCsLogger.Log("Loaded config successfully.");
		}
		catch
		{
			LuaCsLogger.Log("Failed to load config!");
		}
	}

    public void SaveCharacterConfigs()
	{
		LuaCsLogger.LogMessage("Saving character configs...");
		var characterAsJson = JsonSerializer.Serialize(CharacterConfig, JsonSerializerOptions);
		LuaCsFile.Write(CharacterSavePath, characterAsJson);
        // LuaCsLogger.Log($"Config JSON:\n\n{characterAsJson}\n");
	}

	public void LoadCharacterConfigs()
	{
		LuaCsLogger.LogMessage("Loading character configs...");
		if(LuaCsFile.Exists(CharacterSavePath))
		{
			var characterAsJson = LuaCsFile.Read(CharacterSavePath);
			CharacterConfig = JsonSerializer.Deserialize<Dictionary<string, CharacterConfigData>>(characterAsJson, JsonSerializerOptions) ?? [];
		}
		else
		{
			CharacterConfig = new Dictionary<string, CharacterConfigData>();
		}
	}


    private void ServerRead_Character(object[] args)
    {
        try
        {
            IReadMessage inMsg = (IReadMessage)args[0];
            Client c = (Client)args[1];
            ReadNetCharacterConfig(ref inMsg);
            ServerWrite_Character();
            ShouldSave = true;
        }
        catch (Exception err)
        {
            LuaCsLogger.Log(err.ToString());
        }
    }

    public void ServerWrite_Character()
    {
        // LuaCsLogger.Log("Propagating character to all clients...");
        IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
        WriteCharacterConfig(ref outMsg);
        NetUtil.SendAll(outMsg);
    }

    public void ServerUpdate_Character(object[] args)
    {
        Client c = (Client)args[1];
        IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
        NetUtil.SendClient(outMsg, c.Connection);
    }

    // private void CharacterRequest(object[] args)
    // {
    //     // IReadMessage inMsg = (IReadMessage)args[0];
    //     Client c = (Client)args[1];
    //     // string version = inMsg.ReadString();
    //     // if (!CheckClientVersion(c, version)) return; // Exit if the client doesn't have the right version
    //     IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);

    //     LuaCsLogger.Log("Server has received a character request.");

    //     WriteCharacterConfig(ref outMsg);
    //     NetUtil.SendClient(outMsg, c.Connection);
    // }

    public partial void UpdateConfig(){}

    // public partial void RequestCharacterConfig(){}

}