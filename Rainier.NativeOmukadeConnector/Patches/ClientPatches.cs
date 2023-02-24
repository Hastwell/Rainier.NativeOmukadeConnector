// #define DEBUG_LOG_RX_MESSAGES
using HarmonyLib;
using Newtonsoft.Json;
using Platform.Sdk;
using Platform.Sdk.Stomp;
using Platform.Sdk.Util;
using Rainier.NativeOmukadeConnector.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(Client))]
    internal static class ClientPatches
    {
        internal static event Action<OnlineFriendsResponse>? ReceivedOnlineFriendsResponse;
        internal static MethodInfo forwardMessageForOFR = typeof(Client)
            .GetMethod("ForwardMessage", BindingFlags.Instance | BindingFlags.NonPublic)
            .MakeGenericMethod(typeof(OnlineFriendsResponse));

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(ClientBuilder), typeof(string) })]
        static void AdjustClientSettings(ref bool ____enableMessageReceipts)
        {
            Plugin.SharedLogger.LogInfo(nameof(ClientPatches) + ": Enable Message Receipts patched to: false");
#warning TODO: Implement receipts in Omukade
            ____enableMessageReceipts = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateMessageHandlerMapping")]
        static void AddHanderForResponses(Client __instance, Dictionary<string, Action<string, ReusableBuffer>> __result)
        {
            //Plugin.SharedLogger.LogInfo($"Adding OFR frame to list of message handlers...");
            __result.Add(nameof(OnlineFriendsResponse), (contentType, buffer) =>
            {
                try
                {
                    forwardMessageForOFR.Invoke(__instance, new object[] { contentType, buffer, new MessageHandler<OnlineFriendsResponse>((c, e) => ReceivedOnlineFriendsResponse?.Invoke(e)) });
                }
                catch(Exception e)
                {
                    BetterExceptionLogger.LogException(e);
                    throw;
                }
            });
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Client.EnterRegionAsync))]
        static void WarnOfRegionChange()
        {
            StringBuilder sb = new StringBuilder($"{nameof(Client.EnterRegionAsync)} - Reconnecting due to change-of-region, which shouldn't be occuring under Omukade.\n");
            StackTrace st = new System.Diagnostics.StackTrace(true);
            BetterExceptionLogger.PrepareStacktraceString(sb, st);

            Plugin.SharedLogger.LogWarning(sb.ToString());
        }

#if DEBUG_LOG_RX_MESSAGES
        [HarmonyPrefix]
        [HarmonyPatch("Dispatch")]
        static void LogRxMessage(StompFrame frame, ReusableBuffer buffer)
        {
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
            string isJsonString = frame.ContentType == "application/json" ? "JSON" : "BINARY";
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

            Plugin.SharedLogger.LogInfo($"RX {isJsonString} {frame.Payload} (len={buffer.Length}, cslen={buffer.ContentSegment.Count})");
        }
#endif
    }
}
