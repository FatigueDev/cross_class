// Thank you, More Level Content
// https://github.com/dakkhuza/MoreLevelContent/blob/main/Mod%20Files/CSharp/Shared/Utils/Singleton.cs
// <3

using System;
using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using Microsoft.Xna.Framework;

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
	// public CampaignConfig CampaignConfig = CampaignConfig.GetDefault();
	public CharacterConfig CharacterConfig = CharacterConfig.GetDefault();
	public CampaignMode? Campaign;
	static readonly string configFolder = $"{ACsMod.GetStoreFolder<CrossClass>()}";
	static readonly string campaignFile = $"campaign.xml";
	// static readonly string characterFile = "character.xml";
	static string campaignRoot = string.Empty;
	static string campaignSaveFile = string.Empty;
	static string characterSaveLocation = string.Empty;

	// public override void Setup(string CampaignName = "", int CampaignID = -1)
	public override void Setup()
	{
		// Config.CampaignData.CampaignName = CampaignName;
		// Config.CampaignData.CampaignID = CampaignID;
		// if(CrossClass.IsCampaign)
		// {
#if CLIENT
		SetupClient();
#elif SERVER
		SetupServer();
#endif
		// }
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
			// if(GameMain.IsMultiplayer)
			// 	Campaign = Campaign as MultiPlayerCampaign;
			// else if(GameMain.IsSingleplayer)
			// 	Campaign = Campaign as SinglePlayerCampaign;

			LuaCsLogger.Log($"Campaign is: {Campaign}");
			LuaCsLogger.Log($"configFolder is: {configFolder}");
			LuaCsLogger.Log($"campaignFile is: {campaignFile}");
			LuaCsLogger.Log($"GameMain.GameSession.DataPath: {GameMain.GameSession.DataPath.SavePath}");
			LuaCsLogger.Log($"Filname SHOULD be: \"{Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath)}\"");
			Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath);

			var campaignName = Path.GetFileNameWithoutExtension(GameMain.GameSession.DataPath.SavePath).Replace(" ", "_").Trim();
			// LuaCsLogger.Log($"campaignHashCode is: {campaignHashCode}");

			campaignRoot = Path.Join(configFolder, campaignName);
			campaignSaveFile = Path.Join(campaignRoot, campaignFile);
			// characterSaveLocation = Path.Join(campaignRoot, "LocalPlayer.xml");

			LuaCsFile.CreateDirectory(campaignRoot);

			// if (LuaCsFile.Exists(campaignSaveFile))
			// {

				// CampaignConfig = LuaCsConfig.Load<CampaignConfig>(campaignSaveFile);
				// if (Config.Version != Main.Version)
				// {
				// 	MigrateConfig();
				// 	Log.Debug("Migrated Config");
				// }
#if CLIENT
				// DisplayPatchNotes();
				// characterSaveLocation = Path.Join(configFolder, campaignHashCode, GameMain.Client.nameId.ToString(), characterFile);
				
				// if(LuaCsFile.Exists(characterSaveLocation))
				// {
				// 	CharacterConfig = LuaCsConfig.Load<CharacterConfig>(characterSaveLocation);
				// }
				
				// SetConfig(CampaignConfig);
				
#endif
				// Config.Version = Main.Version;
				// SaveConfig();
				// return;

			// }
			// else
			// {
				LuaCsLogger.LogMessage("File doesn't exist");
				// SaveCampaign();
			// }
		}
		catch
		{
			LuaCsLogger.Log("Failed to load config!");
			// SaveCampaign();
			// DefaultConfig();
		}
	}


	// private void MigrateConfig()
	// {
	// Config.NetworkedConfig.GeneralConfig.EnableThalamusCaves = true;
	// Config.NetworkedConfig.GeneralConfig.DistressSpawnChance = 35;
	// Config.NetworkedConfig.GeneralConfig.MaxActiveDistressBeacons = 5;
	// Config.NetworkedConfig.PirateConfig.PeakSpawnChance = 35;
	// Config.NetworkedConfig.PirateConfig.EnablePirateBases = true;
	// Config.NetworkedConfig.GeneralConfig.EnableConstructionSites = true;
	// Config.NetworkedConfig.GeneralConfig.EnableDistressMissions = true;
	// Config.NetworkedConfig.GeneralConfig.EnableMapFeatures = true;
	// Config.NetworkedConfig.GeneralConfig.EnableRelayStations = true;
	// }

	private void DefaultConfig()
	{
		LuaCsLogger.Log("Defaulting config with campaign vals...");
		// CharacterConfig.CharacterData.CharacterInfoID = (int)Character.Controlled.ID;

		// CampaignConfig.CampaignName = Campaign.Name.ToString();
		// if(GameMain.IsSingleplayer)
		// {
		// }
		// else if(GameMain.IsMultiplayer)
		// {
		// 	Config.CampaignData.CampaignName = Campaign_MultiPlayer!.Name.ToString();
		// }

	}

	// public void SaveCampaign()
	// {
	// 	// if(GameMain.IsSingleplayer)
	// 	// {
	// 	// 	joinedConfig = Path.Join(configFolder, "Singleplayer", Config.CampaignData.CampaignName, configFile);
	// 	// }
	// 	// else
	// 	// {
	// 	// 	joinedConfig = Path.Join(configFolder, "Multiplayer", Config.CampaignData.CampaignID.ToString(), configFile);
	// 	// }
	// 	// joinedConfig = Path.Join(configFolder, campaign.CampaignID.ToString(), configFile);
	// 	// LuaCsConfig.Save(campaignSaveFile, CampaignConfig);
	// 	LuaCsLogger.Log($"Campaign update saved to disk!");
	// 	// LuaCsLogger.Log("Saved config to disk!");
	// }

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
		LuaCsConfig.Save(Path.Join(campaignRoot, $"{id}.xml"), CharacterConfig);
		LuaCsLogger.Log("Character update saved to disk!");
	}

	// private void ReadNetCampaignConfig(ref IReadMessage inMsg)
	// {
	// 	try
	// 	{
	// 		CampaignConfig.CampaignData = INetSerializableStruct.Read<CampaignConfig_Packet>(inMsg);
	// 	}
	// 	catch (Exception err)
	// 	{
	// 		LuaCsLogger.Log(err.ToString());
	// 	}
	// }

	private void ReadNetCharacterConfig(ref IReadMessage inMsg)
	{
		try
		{
			CharacterConfig.CharacterData = INetSerializableStruct.Read<CharacterConfig_Packet>(inMsg);
		}
		catch (Exception err)
		{
			LuaCsLogger.Log(err.ToString());
		}
	}

	// private void WriteCampaignConfig(ref IWriteMessage outMsg) =>
	// 	(CampaignConfig.CampaignData as INetSerializableStruct).Write(outMsg);

	private void WriteCharacterConfig(ref IWriteMessage outMsg) =>
		(CharacterConfig.CharacterData as INetSerializableStruct).Write(outMsg);

	public static string StringSha256Hash(string text) =>
		string.IsNullOrEmpty(text) ? string.Empty : BitConverter.ToString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text))).Replace("-", string.Empty);

}