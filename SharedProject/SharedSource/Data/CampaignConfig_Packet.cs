// Big thanks to https://github.com/dakkhuza/MoreLevelContent/ for being public
// :)

using System.Collections.Immutable;
using Barotrauma;
using Barotrauma.Networking;

namespace CrossClass;

public struct CampaignConfig_Packet : INetSerializableStruct
{
	public string CampaignName;
	// [NetworkSerialize]
	// public CrossClassData_Packet[] CharacterData;

	public static CampaignConfig_Packet GetDefault()
	{
		CampaignConfig_Packet config = new()
		{
			CampaignName = ""//,
			// CharacterData = []
		};
		return config;
	}

	// public override string ToString() => PirateConfig.ToString() + GeneralConfig.ToString();
}