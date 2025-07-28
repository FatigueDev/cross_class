// Big thanks to https://github.com/dakkhuza/MoreLevelContent/ for being public
// :)

using Barotrauma;
using Barotrauma.Networking;

namespace CrossClass;

public struct CampaignConfig
{
	public CampaignConfig_Packet CampaignData;
	// public CrossClass_CharacterData_Packet CharacterData;

	public static CampaignConfig GetDefault()
	{
		CampaignConfig config = new(){
			CampaignData = CampaignConfig_Packet.GetDefault()
		};
		return config;
	}

	// public override string ToString() => "PirateConfig.ToString() + GeneralConfig.ToString()";
}