// Big thanks to https://github.com/dakkhuza/MoreLevelContent/ for being public
// :)

using Barotrauma;
using Barotrauma.Networking;

namespace CrossClass;

public struct CharacterConfig
{
	public CharacterConfig_Packet CharacterData;

	public static CharacterConfig GetDefault()
	{
		CharacterConfig config = new(){
			CharacterData = CharacterConfig_Packet.GetDefault()
		};
		return config;
	}

	// public override string ToString() => "PirateConfig.ToString() + GeneralConfig.ToString()";
}