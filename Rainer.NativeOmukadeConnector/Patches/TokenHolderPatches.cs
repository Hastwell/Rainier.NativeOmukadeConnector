using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch]
    internal static class TokenHolderPatches
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return Enumerable.Repeat(WswCommon.platformSdkAssembly.GetType("Platform.Sdk.TokenHolder").GetMethod("CheckIfSessionChanged", BindingFlags.Instance | BindingFlags.Public), 1);
        }

        // Since we're abusing SessionStart as a cue to send an initial SDM message, we need to make sure it always fires.
        [HarmonyTranspiler]
        [HarmonyPatch]
        static IEnumerable<CodeInstruction> SessionAlwaysChanged()
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}
