using HarmonyLib;
using Platform.Sdk;
using RainierClientSDK.source.Friend.Implementations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Platform.Sdk.Models.Friend;
using System.Collections.Concurrent;
using Rainer.NativeOmukadeConnector.Messages;
using System.Linq;
using Newtonsoft.Json;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(Client))]
    internal static class FriendStatusPatches
    {
        static readonly TimeSpan TIMEOUT_FOR_FRIEND_MESSAGES = new TimeSpan(hours: 0, minutes: 0, seconds: 10);
        

        [HarmonyPatch(nameof(Client.GetFriendOnlineStatusAsync))]
        [HarmonyPrefix]
        static bool GetFriendOnlineStatusAsync(Client __instance, ref Task __result, string friendPtcsGuid, ResponseHandler<GetFriendOnlineStatusResponse> success, ErrorHandler failure)
        {
            // The FriendPTCS guid is useless, and distinctly different from the signedAccountId.Id used elsewhere.
            string realFriendId = ((PlatformFriendService)RainierClientSDK.source.Friend.Friend.helper).friends[friendPtcsGuid].signedAccountId.accountId;

            if (Plugin.Settings.ForceFriendsToBeOnline)
            {
                // Plugin.SharedLogger.LogInfo($"{nameof(Client.GetFriendOnlineStatusAsync)} - Injecting IsOnline = true for Friend {friendPtcsGuid}/GUID {realFriendId}");
                success?.Invoke(__instance, new GetFriendOnlineStatusResponse(isOnline: true, realFriendId));
                __result = Task.CompletedTask;
            }
            else
            {
                // Plugin.SharedLogger.LogInfo($"{nameof(Client.GetFriendOnlineStatusAsync)} - Checking Omukade for specific friend {friendPtcsGuid}/GUID {realFriendId}");
                __result = GetSingleFriendStatusFromOmukadeAsync(__instance, realFriendId, success, failure);
            }

            return false;
        }

        static async Task GetSingleFriendStatusFromOmukadeAsync(Client instance, string friendPtcsGuid, ResponseHandler<GetFriendOnlineStatusResponse> success, ErrorHandler failure)
        {
            List<string> friendFound = GetOnlineFriendsFromOmukade(instance, new List<string> { friendPtcsGuid });
            success?.Invoke(instance, new GetFriendOnlineStatusResponse(isOnline: friendFound.Contains(friendPtcsGuid), friendPtcsGuid));
        }

        internal static List<string> GetOnlineFriendsFromOmukade(Client instance, List<string> concernedFriends)
        {
            using ManualResetEvent getFriendsEvent = new ManualResetEvent(initialState: false);
            OnlineFriendsResponse? ofr = null;
            uint txId = unchecked((uint)DateTime.UtcNow.Ticks);
            // Plugin.SharedLogger.LogInfo($"Preparing to get friend data (TXID {txId})...");
            Action<OnlineFriendsResponse> respondToGetFriends = (OnlineFriendsResponse ofrReceived) =>
            {
                Plugin.SharedLogger.LogInfo($"{nameof(GetOnlineFriendsFromOmukade)} - For TxID {txId}, receiving OFR {JsonConvert.SerializeObject(ofrReceived)}");
                if (ofrReceived.TransactionId == txId)
                {
                    ofr = ofrReceived;
                    getFriendsEvent.Set();
                }
            };

            ClientPatches.ReceivedOnlineFriendsResponse += respondToGetFriends;
            WswCommon.InjectUpsockMessage(instance, new GetOnlineFriends { FriendIds = concernedFriends, TransactionId = txId });

            bool didGetSignalInTime = getFriendsEvent.WaitOne(TIMEOUT_FOR_FRIEND_MESSAGES);

            if(didGetSignalInTime)
            {
                Plugin.SharedLogger.LogInfo($"GetFriends event was received; returning control");
            }
            else
            {
                Plugin.SharedLogger.LogError($"GetFriends event timed out; returning control");
            }

            ClientPatches.ReceivedOnlineFriendsResponse -= respondToGetFriends;

            // Plugin.SharedLogger.LogInfo($"Online Friends Response: {JsonConvert.SerializeObject(ofr.CurrentlyOnlineFriends)}");
            return ofr?.CurrentlyOnlineFriends;
        }
    }

    [HarmonyPatch]
    internal static class FriendServicePatches
    {
        // RainierClientSDK.source.Friend.Implementations.PlatformFriendService.<>c__DisplayClass26_0
        // This patches one of the async method fragments for PlatformFriendService.GetFriendsAsync.
        // I didn't want to hard-code it to a specific method name, as the name may be unstable from build to build.
        static IEnumerable<MethodBase> TargetMethods() => typeof(PlatformFriendService)
            .GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic)
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            .Where(m => m.Name.StartsWith($"<{nameof(PlatformFriendService.GetFriendsAsync)}>"))
            .Where(m => { ParameterInfo[] mParams = m.GetParameters(); return mParams.Length == 2 && mParams[0].ParameterType == typeof(Client) && mParams[1].ParameterType == typeof(GetAllFriendsResponse); })
            .Take(1);

        static void Prefix(Client sdk, GetAllFriendsResponse message)
        {
            // Plugin.SharedLogger.LogInfo($"{nameof(PlatformFriendService.GetFriendsAsync)} - Checking Omukade for all online friends");
            if (Plugin.Settings.ForceFriendsToBeOnline)
            {
                foreach (FriendInfo friend in message.friendInfos)
                {
                    friend.isOnline = true;
                }
            }
            else
            {
                List<string> friendIds = message.friendInfos.Select(f => f.signedAccountId.accountId).ToList();
                List<string> onlineFriendsFound = FriendStatusPatches.GetOnlineFriendsFromOmukade(sdk, friendIds);
                HashSet<string> actuallyOnlineFriends = new HashSet<string>(onlineFriendsFound);

                foreach (FriendInfo friend in message.friendInfos)
                {
                    friend.isOnline = actuallyOnlineFriends.Contains(friend.signedAccountId.accountId);
                    // Plugin.SharedLogger.LogInfo($"|- Friend {friend.friendScreenName} - {friend.signedAccountId.accountId} - IsOnline {friend.isOnline}");
                }
            }
        }
    }
}
