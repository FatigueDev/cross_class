using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barotrauma;
using HarmonyLib;
using CrossClass;

namespace CrossClass;

[HarmonyPatch]
public class CampaignMode_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CampaignMode), "HandleSaveAndQuit")]
    static void HandleSaveAndQuit()
    {
#if SERVER
        if(CrossClassSync.Instance.ShouldSave)
        {
            CrossClassSync.Instance.SaveCharacterConfigs();
        }
#endif
    }
}
