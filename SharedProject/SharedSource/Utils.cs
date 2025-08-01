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
	CHARACTER_REQUEST
}

public static class CrossClassHelpers
{
	public static void UnlockCrossClassTalentTree(TalentTree talentTree)
	{
		List<string> crossClasses = [.. CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses];
		crossClasses.Add(talentTree.Identifier.ToString());
		CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses = crossClasses.ToArray(); //new CrossClass_Packet{Identifiers = crossClasses.ToImmutableArray()};
		// CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses =
		// 	CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses.AddItem(talentTree.Identifier.ToString());
		// CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses = CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses.AddToArray(talentTree.Identifier.ToString());
		SetSpentCrossClassPoints(GetSpentCrossClassPoints() + 1);
// #if CLIENT
// 		if(CrossClassSync.Instance.CharacterConfig.Initialized)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
		// SetSavedStatValue($"{crossClassUnlockedTreesString}{talentTree.Identifier}", 1f, info);
		// int currentSpent = GetSpentCrossClassPoints(info);
		// SetSavedStatValue(crossClassSpentPointsString, currentSpent, info);
	}
	
	public static void SetTotalCrossClassTalentPoints(int value)
	{
		CrossClassSync.Instance.CharacterConfig.CharacterData.TotalCrossClassPoints = value;
// #if CLIENT
// 		if(!GameMain.IsSingleplayer)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
	}

	public static void SetSpentCrossClassPoints(int value)
	{
		CrossClassSync.Instance.CharacterConfig.CharacterData.SpentCrossClassPoints = value;
// #if CLIENT
// 		if(!GameMain.IsSingleplayer)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
	}

	public static int GetTotalCrossClassPoints()
	{
		return CrossClassSync.Instance.CharacterConfig.CharacterData.TotalCrossClassPoints;
	}

	public static void UpdateCharacterTotalTalentPoints(CharacterInfo info)
	{
		var currentLevel = info.GetCurrentLevel();
		int crossClassPoints = 0;
		if (currentLevel >= 3)
		{
			for (int i = 3; i < currentLevel; i++)
			{
				if (i == 3 || i % 3 == 0)
				{
					crossClassPoints++;
				}
			}
			SetTotalCrossClassTalentPoints(crossClassPoints);
// #if CLIENT
// 		if(CrossClassSync.Instance.CharacterConfig.Initialized)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
		}
	}

	public static int GetSpentCrossClassPoints()
	{
		return CrossClassSync.Instance.CharacterConfig.CharacterData.SpentCrossClassPoints;
		//return GetSavedStatValue(crossClassSpentPointsString, info);
	}

	public static int GetAvailableCrossClassPoints()
	{
		return GetTotalCrossClassPoints() - GetSpentCrossClassPoints();
	}

	public static bool CanCrossClass()
	{
		// var availablePoints = GetAvailableCrossClassPoints(characterInfo);
		// var totalPoints = GetTotalCrossClassPoints(characterInfo);
		// var spentPoints = GetSpentCrossClassPoints(characterInfo);
		// LuaCsLogger.LogError($"\nAvailable points: {availablePoints}\nTotal points: {totalPoints}\nSpent points: {spentPoints}\n");
		return GetAvailableCrossClassPoints() > 0;
	}

	public static bool HasCrossClassTalentTree(TalentTree talentTree)
	{
		return CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses.Contains(talentTree.Identifier.ToString());
		// return GetSavedStatValue($"{crossClassUnlockedTreesString}{talentTree.Identifier}", info) != 0;
	}

	public static TalentTree? GetPrimaryTalentTree(CharacterInfo info)
	{
		if(string.IsNullOrEmpty(CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass))
		{
			CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass = info.Job.Prefab.Identifier.ToString();
// #if CLIENT
// 			if(!GameMain.IsSingleplayer)
// 				CrossClassSync.Instance.UpdateConfig();
// #endif
		}
		return GetTalentTreeForJobIdentifier(info.Job.Prefab.Identifier);

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

	public static TalentTree? GetSelectedTalentTree()
	{
		return GetTalentTreeForJobIdentifier(CrossClassSync.Instance.CharacterConfig.CharacterData.SelectedClass);
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

	public static IEnumerable<TalentTree> GetCrossClassTalentTrees()
	{
		IEnumerable<TalentTree> crossClassTalentTrees = [];
		foreach(string talentIdentifierString in CrossClassSync.Instance.CharacterConfig.CharacterData.CrossClasses)
		{
			if(GetTalentTreeForJobIdentifier(talentIdentifierString) is TalentTree talentTree)
			{
				crossClassTalentTrees = crossClassTalentTrees.Append(talentTree);
			}
		}
		return crossClassTalentTrees;
	}

	public static void SetPrimaryTalentTree(Identifier talentTreeIdentifier)
	{
		CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass = talentTreeIdentifier.ToString();
// #if CLIENT
// 		if(!GameMain.IsSingleplayer)
// 			CrossClassSync.Instance.UpdateConfig();
// #endif
		// SelectedTalentTree = GetTalentTreeForJobIdentifier(selectedTalentTreeJobPrefabIdentifier);
		// TalentTree talentTree = GetSelectedTalentTree(info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{talentTree.Identifier}", 0, info);
		// SetSavedStatValue($"{crossClassSelectedTreeString}{selectedTalentTreeJobPrefabIdentifier}", 1, info);
	}

	public static void SetSelectedTalentTree(Identifier talentTreeIdentifier)
	{
		CrossClassSync.Instance.CharacterConfig.CharacterData.SelectedClass = talentTreeIdentifier.ToString();
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