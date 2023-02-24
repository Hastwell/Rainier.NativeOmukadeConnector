using HarmonyLib;
using Platform.Sdk;
using Platform.Sdk.Codecs;
using Rainier.NativeOmukadeConnector.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(Command<GetOnlineFriends>))]
    [HarmonyPatch(typeof(Command<SupplementalDataMessageV2>))]
    internal static class CommandPatches
    {
#warning TODO; add a custom payload for SDMs and GOFs to leverage Rainier's native encoding; this hacky patch runs on every message sent
        [HarmonyPatch(nameof(Command<SupplementalDataMessageV2>.Encode))]
        [HarmonyPrefix]
        static void ForceJsonForOmukadePayloads(ref ICodec serializer, object body)
        {
            if(body is SupplementalDataMessageV2 || body is GetOnlineFriends)
            {
                Plugin.SharedLogger.LogInfo($"Forcing {body.GetType().Name} to be processed as JSON");
                serializer = WswCommon.JsonCodec;
            }
        }
    }
}
