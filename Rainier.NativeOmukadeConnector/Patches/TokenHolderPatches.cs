using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch]
    internal static class TokenHolder_DontSendWebsocketAuth
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return Enumerable.Repeat(WswCommon.platformSdkAssembly.GetType("Platform.Sdk.TokenHolder").GetMethod("AddWebsocketUpgradeHeaders", BindingFlags.Instance | BindingFlags.Public), 1);
        }

        [HarmonyTranspiler]
        [HarmonyPatch]
        static IEnumerable<CodeInstruction> DontAddWebsocketAuthHeaders()
        {
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}
