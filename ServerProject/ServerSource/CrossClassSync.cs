using System;
using System.Collections.Generic;
using System.Linq;
using Barotrauma;
using Barotrauma.Networking;


namespace CrossClass;

/// <summary>
/// Server
/// </summary>
partial class CrossClassSync : Singleton<CrossClassSync>
{
    private void SetupServer()
    {
        NetUtil.Register(NetEvent.CHARACTER_WRITE_SERVER, ServerRead_Character);
        NetUtil.Register(NetEvent.CHARACTER_REQUEST, CharacterRequest);
        
        CrossClass.Hook("think", "cross_class.server.think", (args) => {
            if(CrossClass.IsRunning && CrossClass.IsCampaign == false)
            {
                LuaCsLogger.LogError("CrossClass doesn't work in games that aren't campaigns.");
                GameMain.LuaCs.Hook.Remove("think", "cross_class.server.think");    
                return null;
            }

            if(CrossClass.IsCampaign == false)
            {
                return null;
            }

            LuaCsLogger.Log("[SV] CrossClass sync initialized");
            Instance.LoadConfig();
            GameMain.LuaCs.Hook.Remove("think", "cross_class.server.think");
            return null;
        });
    }

    #region Character Networking
    private void ServerRead_Character(object[] args)
    {
        try
        {
            IReadMessage inMsg = (IReadMessage)args[0];
            Client c = (Client)args[1];
            ReadNetCharacterConfig(ref inMsg);
            // ServerWrite_Character();
            SaveCharacter(Math.Abs(c.CharacterInfo.GetIdentifierUsingOriginalName()).ToString());            
        }
        catch (Exception err)
        {
            LuaCsLogger.Log(err.ToString());
        }
    }

    // private void ServerWrite_Character()
    // {
    //     // LuaCsLogger.Log("Propagating character to all clients...");
    //     IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
    //     WriteCharacterConfig(ref outMsg);
    //     NetUtil.SendAll(outMsg);
    // }

    private void CharacterRequest(object[] args)
    {
        // IReadMessage inMsg = (IReadMessage)args[0];
        Client c = (Client)args[1];
        // string version = inMsg.ReadString();
        // if (!CheckClientVersion(c, version)) return; // Exit if the client doesn't have the right version
        IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);

        LuaCsLogger.Log("Server has received a character request.");

        try
        {
            LuaCsLogger.Log("Trying to load character...");
            LuaCsLogger.Log($"Save ID: {Math.Abs(c.CharacterInfo.GetIdentifierUsingOriginalName())}");
            CharacterConfig.CharacterData = LoadCharacter(Math.Abs(c.CharacterInfo.GetIdentifierUsingOriginalName()).ToString());
            LuaCsLogger.Log("Success!");
        }
        catch (Exception)
		{
            LuaCsLogger.Log("Failed; getting default.");
            CharacterConfig = new CharacterConfig();
        }

        WriteCharacterConfig(ref outMsg);
        NetUtil.SendClient(outMsg, c.Connection);
    }

    #endregion

    public partial void UpdateConfig(){}

    public partial void RequestCharacterConfig(){}

}