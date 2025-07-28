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
        // NetUtil.Register(NetEvent.CAMPAIGN_WRITE_SERVER, ServerRead_Campaign);
        // NetUtil.Register(NetEvent.CAMPAIGN_REQUEST, CampaignRequest);
        NetUtil.Register(NetEvent.CHARACTER_WRITE_SERVER, ServerRead_Character);
        NetUtil.Register(NetEvent.CHARACTER_REQUEST, CharacterRequest);
        
        CrossClass.Hook("roundStart", "cross_class.server.roundStart", (args) => {
            LoadConfig();
            LuaCsLogger.Log("[SV] CrossClass sync initialized");
            return null;
        });

        // Only setup default config on non-dedicated servers, on servers hosted through
        // the in game menu, the owner of the server will send the config to use
        // where as on dedicated servers they will never get a config sent and thus will
        // always have the default config loaded
        // if (!CrossClass.IsDedicatedServer)
        // {
        //     DefaultConfig();
        // } else
        // {
        // CrossClass.Hook("roundStart", "cross_class.server.roundStart", (args) =>
        // {
        //     return null;
        // });

        // }

    }

    // private readonly List<ulong> correctInstalls = new List<ulong>();

    #region Campaign Networking
    // private void ServerRead_Campaign(object[] args)
    // {
    //     try
    //     {
    //         IReadMessage inMsg = (IReadMessage)args[0];
    //         Client c = (Client)args[1];
    //         LuaCsLogger.Log($"Got campaign from client {c.Name}");
    //         // if (!c.HasPermission(ClientPermissions.ManageSettings))
    //         // {
    //         //     LuaCsLogger.Log("No Perms!");
    //         //     return;
    //         // }
    //         // if (!CheckClientVersion(c, inMsg.ReadString()))
    //         // {
    //         //     LuaCsLogger.Log($"Ignored config from {c.Name} due to them using the wrong version!");
    //         //     return;
    //         // }
    //         ReadNetCampaignConfig(ref inMsg);
    //         ServerWrite_Campaign();
    //         SaveCampaign();
    //     }
    //     catch (Exception err)
    //     {
    //         LuaCsLogger.Log(err.ToString());
    //     }
    // }

    // private void ServerWrite_Campaign()
    // {
    //     LuaCsLogger.Log("Propagating config to all clients...");
    //     IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CAMPAIGN_WRITE_CLIENT);
    //     WriteCampaignConfig(ref outMsg);
    //     NetUtil.SendAll(outMsg);
    // }

    // private void CampaignRequest(object[] args)
    // {
    //     // IReadMessage inMsg = (IReadMessage)args[0];
    //     Client c = (Client)args[1];
    //     // string version = inMsg.ReadString();
    //     // if (!CheckClientVersion(c, version)) return; // Exit if the client doesn't have the right version
    //     IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CAMPAIGN_WRITE_CLIENT);
    //     WriteCampaignConfig(ref outMsg);
    //     NetUtil.SendClient(outMsg, c.Connection);
    //     LuaCsLogger.Log($"Sent campaign to client {c.Name}");
    // }

    #endregion

    #region Character Networking
    private void ServerRead_Character(object[] args)
    {
        try
        {
            IReadMessage inMsg = (IReadMessage)args[0];
            Client c = (Client)args[1];
            LuaCsLogger.Log($"Got character from client {c.Name}");
            // if (!c.HasPermission(ClientPermissions.ManageSettings))
            // {
            //     LuaCsLogger.Log("No Perms!");
            //     return;
            // }
            // if (!CheckClientVersion(c, inMsg.ReadString()))
            // {
            //     LuaCsLogger.Log($"Ignored config from {c.Name} due to them using the wrong version!");
            //     return;
            // }
            ReadNetCharacterConfig(ref inMsg);
            ServerWrite_Character();
            if(c?.AccountId != null && c.AccountId.hasValue)
            {
                SaveCharacter(c.AccountId.value!.ToString());
            }
            else
            {
                SaveCharacter("local_player");
            }
            
        }
        catch (Exception err)
        {
            LuaCsLogger.Log(err.ToString());
        }
    }

    private void ServerWrite_Character()
    {
        LuaCsLogger.Log("Propagating character to all clients...");
        IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
        WriteCharacterConfig(ref outMsg);
        NetUtil.SendAll(outMsg);
    }

    private void CharacterRequest(object[] args)
    {
        // IReadMessage inMsg = (IReadMessage)args[0];
        Client c = (Client)args[1];
        // string version = inMsg.ReadString();
        // if (!CheckClientVersion(c, version)) return; // Exit if the client doesn't have the right version
        IWriteMessage outMsg = NetUtil.CreateNetMsg(NetEvent.CHARACTER_WRITE_CLIENT);
        WriteCharacterConfig(ref outMsg);
        NetUtil.SendClient(outMsg, c.Connection);
        LuaCsLogger.Log($"Sent character to client {c.Name}");
    }

    #endregion

}