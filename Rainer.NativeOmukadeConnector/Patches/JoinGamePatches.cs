using HarmonyLib;
using Platform.Sdk;
using Platform.Sdk.Models.GameServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(Client), nameof(Client.JoinGameAsync), typeof(JoinGameRequest), typeof(ResponseHandler<JoinGameResponse>), typeof(ErrorHandler))]
    static class Client_JoinGameAsync_GIGO
    {
        static bool Prefix(Client __instance, ref Task __result, JoinGameRequest request, ResponseHandler<JoinGameResponse> success, ErrorHandler failure)
        {
            //Plugin.SharedLogger.LogInfo("JoinGame is now returning ticket value to avoid HTTP call");
            success?.Invoke(__instance, new JoinGameResponse { gameId = request.ticket });

            __result = Task.CompletedTask;
            return false;
        }
    }
}
