using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Barotrauma;
using Barotrauma.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using static Barotrauma.TalentTree;
using static Barotrauma.TalentTree.TalentStages;
using static CrossClass.CrossClassHelpers;


namespace CrossClass;

[HarmonyPatch]
public class CrewManager_Shared_Patches
{

	// [HarmonyPatch(typeof(CrewManager), "AddCharacter")]
	// static void AddCharacter_Postfix(Character character)
	// {
// #if SERVER
// 		LuaCsLogger.LogMessage("Adding character, writing to all clients");
// 		CrossClassSync.Instance.ServerWrite_Character();
// #endif
	// }
}