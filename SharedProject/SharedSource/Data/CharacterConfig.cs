// Big thanks to https://github.com/dakkhuza/MoreLevelContent/ for being public
// :)

using Barotrauma;
using Barotrauma.Networking;

namespace CrossClass;

public struct CharacterConfigData : INetSerializableStruct
{
	[NetworkSerialize]
	public string OriginalName {get; set;}
	[NetworkSerialize]
	public string PrimaryClass { get; set; }

	[NetworkSerialize]
	public string SelectedClass { get; set; }

	[NetworkSerialize(ArrayMaxSize = 64)]
	public string[] CrossClasses { get; set; }

	[NetworkSerialize]
	public int TotalCrossClassPoints { get; set; }

	[NetworkSerialize]
	public int SpentCrossClassPoints { get; set; }
}

// public struct CharacterConfig
// {
// 	[NetworkSerialize]
// 	public CharacterConfigData CharacterData;

// 	public CharacterConfig()
// 	{
// 		CharacterData = new CharacterConfigData{
// 			PrimaryClass = "",
// 			SelectedClass = "",
// 			TotalCrossClassPoints = 0,
// 			SpentCrossClassPoints = 0,
// 			CrossClasses = []
// 		};
// 	}

// 	// public static CharacterConfig GetDefault()
// 	// {
// 	// 	CharacterConfig config = new(){
// 	// 		CharacterData = CharacterConfig_Packet.GetDefault()
// 	// 	};
// 	// 	return config;
// 	// }

// 	// public override string ToString() => "PirateConfig.ToString() + GeneralConfig.ToString()";
// }