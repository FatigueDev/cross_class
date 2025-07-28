using System;
using Barotrauma;
using Barotrauma.Networking;

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
