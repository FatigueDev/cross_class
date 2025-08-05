// Thank you, More Level Content
// https://github.com/dakkhuza/MoreLevelContent/blob/main/Mod%20Files/CSharp/Shared/Utils/Singleton.cs
// <3

using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using HarmonyLib;
using static Barotrauma.TalentTree;
// using Newtonsoft.Json;

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
	public Dictionary<string, CharacterConfigData> CharacterConfig = [];
	public CampaignMode? Campaign;
	public bool Initialized = false;
	public bool ShouldSave = false;

	public JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions{
		AllowTrailingCommas = true,
		WriteIndented = true,
		PropertyNameCaseInsensitive = true
	};

	public override void Setup()
	{
#if SERVER
		SetupServer();
#elif CLIENT
		SetupClient();
#endif
	}

	private void ReadNetCharacterConfig(ref IReadMessage inMsg)
	{
		try
		{
			var characterJson = inMsg.ReadString();
			var characterData = JsonSerializer.Deserialize<Dictionary<string, CharacterConfigData>>(characterJson, JsonSerializerOptions);
			// var characterData = INetSerializableStruct.Read<CharacterConfigData>(inMsg);
			CharacterConfig = characterData; //?? new Dictionary<string, CharacterConfigData>();
			
		}
		catch (Exception err)
		{
			LuaCsLogger.Log(err.ToString());
		}
	}

	private void WriteCharacterConfig(ref IWriteMessage outMsg)
	{
		// foreach(var (id, config) in CharacterConfig)
		// {
			// var serialized = (INetSerializableStruct)config.CharacterData;
			// outMsg.WriteNetSerializableStruct(config.CharacterData)
			
			var characterJson = JsonSerializer.Serialize(CharacterConfig, JsonSerializerOptions);
			outMsg.WriteString(characterJson);

			// (Instance.CharacterConfig[config.OriginalName] as INetSerializableStruct).Write(outMsg);
		// }
	}

	public partial void UpdateConfig();	

}