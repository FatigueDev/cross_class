// Big thanks to https://github.com/dakkhuza/MoreLevelContent/ for being public
// :)

using System.Collections.Immutable;
using Barotrauma;
using Barotrauma.Networking;

namespace CrossClass;

[NetworkSerialize]
public struct CharacterConfig_Packet : INetSerializableStruct
{
	// public Int32 CharacterInfoID;
	public string PrimaryClass;
	// public string SelectedClass;
	public string[] CrossClasses;
	public Int32 TotalCrossClassPoints;
	public Int32 SpentCrossClassPoints;

	public static CharacterConfig_Packet GetDefault()
	{
		CharacterConfig_Packet config = new()
		{
			// CharacterInfoID = 0,
			PrimaryClass = string.Empty,
			// SelectedClass = string.Empty,
			CrossClasses = [],
			TotalCrossClassPoints = 0,
			SpentCrossClassPoints = 0,
		};
		return config;
	}

	// public override string ToString() => PirateConfig.ToString() + GeneralConfig.ToString();
}