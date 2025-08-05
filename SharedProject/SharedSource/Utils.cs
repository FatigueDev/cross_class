using System;
using System.Collections.Immutable;
using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using HarmonyLib;
using static Barotrauma.TalentTree;

namespace CrossClass;

/// <summary>
/// Shared
/// </summary>
public static partial class NetUtil
{
	internal static IWriteMessage CreateNetMsg(NetEvent target) => GameMain.LuaCs.Networking.Start(Enum.GetName(typeof(NetEvent), target));

	/// <summary>
	/// Register a method to run when the specified NetEvent happens
	/// </summary>
	/// <param name="target"></param>
	/// <param name="netEvent"></param>
	public static void Register(NetEvent target, LuaCsAction netEvent)
	{
		if (GameMain.IsSingleplayer) return;
		GameMain.LuaCs.Networking.Receive(Enum.GetName(typeof(NetEvent), target), netEvent);
	}
}

/// <summary>
/// Events that are sent over the network
/// </summary>
public enum NetEvent
{
	/// <summary>
	/// Send a config message to the server
	/// </summary>
	CAMPAIGN_WRITE_SERVER,

	/// <summary>
	/// Send a config message to the clients
	/// </summary>
	CAMPAIGN_WRITE_CLIENT,

	/// <summary>
	/// Request the current config from the server
	/// </summary>
	CAMPAIGN_REQUEST,

		/// <summary>
	/// Send a config message to the server
	/// </summary>
	CHARACTER_WRITE_SERVER,

	/// <summary>
	/// Send a config message to the clients
	/// </summary>
	CHARACTER_WRITE_CLIENT,

	/// <summary>
	/// Request the current config from the server
	/// </summary>
	CHARACTER_REQUEST_SERVER
}

public static class CrossClassHelpers
{
	public static CharacterConfigData GetCharacterConfigData(Character character)
	{
		if(CrossClassSync.Instance.CharacterConfig.ContainsKey(character.Info.OriginalName))
		{
			return CrossClassSync.Instance.CharacterConfig[character.Info.OriginalName];
		}
		else
		{
			return new CharacterConfigData
			{
				OriginalName = character.Info.OriginalName,
				PrimaryClass = character.Info.Job.Prefab.Identifier.ToString(),
				SelectedClass = character.Info.Job.Prefab.Identifier.ToString(),
				SpentCrossClassPoints = 0,
				TotalCrossClassPoints = 0,
				CrossClasses = []
			};
		}
	}

	public static void ApplyChangeset(Character character, CharacterConfigData configData)
	{
		Dictionary<string, CharacterConfigData> changeset = new Dictionary<string, CharacterConfigData>(CrossClassSync.Instance.CharacterConfig);
		changeset[character.Info.OriginalName] = configData;
		CrossClassSync.Instance.CharacterConfig = changeset;
		CrossClassSync.Instance.UpdateConfig();
	}

	public static void UnlockCrossClassTalentTree(TalentTree talentTree, Character character)
	{
		CharacterConfigData d = GetCharacterConfigData(character);

		List<string> crossClasses = [.. d.CrossClasses];
		crossClasses.Add(talentTree.Identifier.ToString());
		d.CrossClasses = crossClasses.ToArray();

		ApplyChangeset(character, d);

		SetSpentCrossClassPoints(GetSpentCrossClassPoints(character) + 1, character);
	}
	
	public static void SetTotalCrossClassTalentPoints(int value, Character character)
	{
		var configData = GetCharacterConfigData(character);
		configData.TotalCrossClassPoints = value;
		ApplyChangeset(character, configData);
	}

	public static void SetSpentCrossClassPoints(int value, Character character)
	{
		var configData = GetCharacterConfigData(character);
		configData.SpentCrossClassPoints = value;
		ApplyChangeset(character, configData);
	}

	public static int GetTotalCrossClassPoints(Character character)
	{
		return GetCharacterConfigData(character).TotalCrossClassPoints;
	}

	public static void UpdateCharacterTotalTalentPoints(Character character)
	{
		var currentLevel = character.Info.GetCurrentLevel();
		int crossClassPoints = 0;
		// if (currentLevel >= 3)
		// {
		for (int i = 0; i < currentLevel; i++)
		{
			if (i != 0 && (i % 3 == 0))
			{
				crossClassPoints++;
			}
		}
		SetTotalCrossClassTalentPoints(crossClassPoints, character);
		// }
	}

	public static int GetSpentCrossClassPoints(Character character)
	{
		return GetCharacterConfigData(character).SpentCrossClassPoints;
		// return CrossClassSync.Instance.CharacterConfig.CharacterData.SpentCrossClassPoints;
		//return GetSavedStatValue(crossClassSpentPointsString, info);
	}

	public static int GetAvailableCrossClassPoints(Character character)
	{
		return GetTotalCrossClassPoints(character) - GetSpentCrossClassPoints(character);
	}

	public static bool CanCrossClass(Character character)
	{
		// var availablePoints = GetAvailableCrossClassPoints(characterInfo);
		// var totalPoints = GetTotalCrossClassPoints(characterInfo);
		// var spentPoints = GetSpentCrossClassPoints(characterInfo);
		// LuaCsLogger.LogError($"\nAvailable points: {availablePoints}\nTotal points: {totalPoints}\nSpent points: {spentPoints}\n");
		return GetAvailableCrossClassPoints(character) > 0;
	}

	public static bool HasCrossClassTalentTree(TalentTree talentTree, Character character)
	{
		return GetCharacterConfigData(character).CrossClasses.Contains(talentTree.Identifier.ToString());
		// return CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses.Contains(talentTree.Identifier.ToString());
		// return GetSavedStatValue($"{crossClassUnlockedTreesString}{talentTree.Identifier}", info) != 0;
	}

	public static TalentTree? GetPrimaryTalentTree(Character character)
	{
// 		var configData = GetCharacterConfigData(character);
// 		if(string.IsNullOrEmpty(configData.PrimaryClass))
// 		{
// 			configData.PrimaryClass = character.Info.Job.prefab.Identifier.ToString();
// 			// CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass = info.Job.Prefab.Identifier.ToString();
// // #if CLIENT
// // 			if(!GameMain.IsSingleplayer)
// // 				CrossClassSync.Instance.UpdateConfig();
// // #endif
// 		}
		return GetTalentTreeForJobIdentifier(character.Info.Job.Prefab.Identifier);

		// if (JobTalentTrees.TryGet(info.Job.Prefab.Identifier, out TalentTree defaultTree))
		// {
		// 	PrimaryTalentTree = defaultTree;
		// 	return defaultTree;
		// }
		// else
		// {
		// 	return null;
		// 	//throw new NullReferenceException("Something was null in GetPrimaryTalentTree");//($"TalentTree.JobTalentTrees does not contain a TalentTree for {GameMain.NetworkMember.Character.Info.Job.Prefab.Identifier}");
		// }
	}

	public static TalentTree? GetSelectedTalentTree(Character character)
	{
		return GetTalentTreeForJobIdentifier(GetCharacterConfigData(character).SelectedClass);
		// return GetTalentTreeForJobIdentifier(CrossClassSync.Instance.CharacterConfig.CharacterData.SelectedClass);
		// return SelectedTalentTree ?? GetPrimaryTalentTree(info) ?? null;
		// foreach (TalentTree tree in TalentTree.JobTalentTrees)
		// {
		// 	if (GetSavedStatValue($"{crossClassSelectedTreeString}{tree.Identifier}", info) != 0)
		// 	{
		// 		return tree;
		// 	}
		// }
		// return GetPrimaryTalentTree(info);
	}

	public static IEnumerable<TalentTree> GetCrossClassTalentTrees(Character character)
	{
		IEnumerable<TalentTree> crossClassTalentTrees = [];
		var configData = GetCharacterConfigData(character);
		foreach(string talentIdentifierString in configData.CrossClasses)
		{
			if(GetTalentTreeForJobIdentifier(talentIdentifierString) is TalentTree talentTree)
			{
				crossClassTalentTrees = crossClassTalentTrees.Append(talentTree);
			}
		}
		return crossClassTalentTrees;
	}

	public static void SetPrimaryTalentTree(Identifier talentTreeIdentifier, Character character)
	{
		var configData = GetCharacterConfigData(character);
		configData.PrimaryClass = talentTreeIdentifier.ToString();
		ApplyChangeset(character, configData);
		// CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass = talentTreeIdentifier.ToString();
// #if CLIENT
// 		if(!GameMain.IsSingleplayer)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
		// SelectedTalentTree = GetTalentTreeForJobIdentifier(selectedTalentTreeJobPrefabIdentifier);
		// TalentTree talentTree = GetSelectedTalentTree(info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{talentTree.Identifier}", 0, info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{selectedTalentTreeJobPrefabIdentifier}", 1, info);
	}

	public static void SetSelectedTalentTree(Identifier talentTreeIdentifier, Character character)
	{
		var configData = GetCharacterConfigData(character);
		configData.SelectedClass = talentTreeIdentifier.ToString();
		ApplyChangeset(character, configData);
		// CrossClassSync.Instance.CharacterConfig.CharacterData.SelectedClass = talentTreeIdentifier.ToString();
// #if CLIENT
// 		if(!GameMain.IsSingleplayer)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
		// SelectedTalentTree = GetTalentTreeForJobIdentifier(selectedTalentTreeJobPrefabIdentifier);
		// TalentTree talentTree = GetSelectedTalentTree(info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{talentTree.Identifier}", 0, info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{selectedTalentTreeJobPrefabIdentifier}", 1, info);
	}

	public static TalentTree? GetTalentTreeForJobIdentifier(Identifier jobPrefabIdentifier)
	{
		if (JobTalentTrees.TryGet(jobPrefabIdentifier, out TalentTree? talentTree))
		{
			return talentTree;
		}
		else
		{
			return null; //GetPrimaryTalentTree(info);
		}
	}
}