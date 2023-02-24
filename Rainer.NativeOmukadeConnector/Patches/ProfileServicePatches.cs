using HarmonyLib;
using Rainer.NativeOmukadeConnector.Messages;
using RainierClientSDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(ProfileService), nameof(ProfileService.SetPlayerName))]
    static class OnPlayerNameUpdated
    {
        static void Prefix(string playerName)
        {
            //Plugin.SharedLogger.LogMessage("Player Display Name updated to " + playerName);
            var upsock = WswCommon.wargServer?.connections.FirstOrDefault();
            if (upsock == null)
            {
                Plugin.SharedLogger.LogError("Can't update Player Display Name - dead upsock");
            }
            else
            {
                upsock.TransmitQueue.Enqueue(new SupplementalDataMessage { PlayerDisplayName = playerName });
            }
        }
    }
}
