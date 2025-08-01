// Thank you, More Level Content
// https://github.com/dakkhuza/MoreLevelContent/blob/main/Mod%20Files/CSharp/Shared/Utils/Singleton.cs
// <3

using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using HarmonyLib;
using static Barotrauma.TalentTree;

namespace CrossClass;

public abstract class Singleton<T> where T : class
{
	/// <summary>
	/// Static instance. Needs to use lambda expression
	/// to construct an instance (since constructor is private).
	/// </summary>
	private static readonly Lazy<T> sInstance = new Lazy<T>(() => CreateInstanceOfT());

	/// <summary>
	/// Gets the instance of this singleton.
	/// </summary>
	public static T Instance => sInstance.Value;

	/// <summary>
	/// Creates an instance of T via reflection since T's constructor is expected to be private.
	/// </summary>
	/// <returns></returns>
	private static T CreateInstanceOfT() => Activator.CreateInstance(typeof(T), true) as T;

	public abstract void Setup();
}

partial class CrossClassSync : Singleton<CrossClassSync>
{
	public bool Initialized = false;
	public CharacterConfig CharacterConfig = new CharacterConfig();
	public CampaignMode? Campaign;
	static readonly string configFolder = $"{ACsMod.GetStoreFolder<CrossClass>()}";
	static string campaignRoot = string.Empty;

	public override void Setup()
	{
#if CLIENT
		SetupClient();
#elif SERVER
		SetupServer();
#endif
	}

	private void LoadConfig()
	{
		if (!CrossClass.IsCampaign)
		{
			LuaCsLogger.Log("Tried to load config, but the game mode was not Campaign so we're skipping.");
			return;
		}

		try
		{
			Campaign = GameMain.GameSession.Campaign;

			// Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath);

			var campaignName = Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath).Replace(" ", "_").Trim();

			campaignRoot = Path.Join(configFolder, campaignName);

			LuaCsFile.CreateDirectory(campaignRoot);

			// if(GameMain.IsSingleplayer)
			// {
			// 	var localFilePath = Path.Join(campaignRoot, "local_player.xml");
			// 	if(LuaCsFile.Exists(localFilePath))
			// 	{
			// 		CharacterConfig = LuaCsConfig.Load<CharacterConfig>(localFilePath);
			// 	}
			// 	else
			// 	{
			// 		CharacterConfig = CharacterConfig.GetDefault();
			// 		// SaveCharacter("local_player");
			// 	}
			// }
		}
		catch
		{
			LuaCsLogger.Log("Failed to load config!");
		}
	}

	public void SaveCharacter(string id)
	{
		// if(GameMain.IsSingleplayer)
		// {
		// 	joinedConfig = Path.Join(configFolder, "Singleplayer", Config.CampaignData.CampaignName, configFile);
		// }
		// else
		// {
		// 	joinedConfig = Path.Join(configFolder, "Multiplayer", Config.CampaignData.CampaignID.ToString(), configFile);
		// }
		// joinedConfig = Path.Join(configFolder, campaign.CampaignID.ToString(), configFile);
		// LuaCsLogger.Log($"Character Saved. Character Config: id={CharacterConfig.CharacterData.CharacterInfoID}");
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			WriteIndented = true
		};
		var characterAsJson = JsonSerializer.Serialize(CharacterConfig.CharacterData, options);
		LuaCsFile.Write(Path.Join(campaignRoot, $"{id}.json"), characterAsJson);
		// LuaCsConfig.Save(Path.Join(campaignRoot, $"{id}.xml"), CharacterConfig);
		// LuaCsLogger.Log("Character update saved to disk!");
	}

	public CharacterConfigData LoadCharacter(string id)
	{
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			WriteIndented = true
		};

		var characterAsJson = LuaCsFile.Read(Path.Join(campaignRoot, $"{id}.json"));
		var config = JsonSerializer.Deserialize<CharacterConfigData>(characterAsJson, options);
		return config;
	}

	// public void SaveCharacter(CharacterInfo info)
	// {
	// 	LuaCsConfig.Save(Path.Join(campaignRoot, $"{info.GetIdentifierUsingOriginalName()}.xml"), CharacterConfig);
	// 	// LuaCsLogger.Log("Character update saved to disk!");
	// }

	private void ReadNetCharacterConfig(ref IReadMessage inMsg)
	{
		try
		{
			CharacterConfig.CharacterData = INetSerializableStruct.Read<CharacterConfigData>(inMsg);
		}
		catch (Exception err)
		{
			LuaCsLogger.Log(err.ToString());
		}
	}

	private void WriteCharacterConfig(ref IWriteMessage outMsg) =>
		(CharacterConfig.CharacterData as INetSerializableStruct).Write(outMsg);

	public partial void UpdateConfig();

	public partial void RequestCharacterConfig();

	

}