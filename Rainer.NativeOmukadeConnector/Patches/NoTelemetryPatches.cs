using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TPCI.Commands;
using TPCI.NetworkSystem;
using TPCI.NewRelic;
using TPCI.Telemetry;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(Endpoint_ProdPipeline))]
    static class NoTelemetryPatches
    {
        [HarmonyPatch(nameof(Endpoint_ProdPipeline.SendEvent))]
        [HarmonyPrefix]
        static bool SendEventNowSendsNoTelemetry(string eventName)
        {
            // Telemetry be gone!
            UnityEngine.Debug.Log($"[NoTelemetry] Swallowed telemetry event {eventName}");
            return false;
        }
    }

    [HarmonyPatch(typeof(TelemetryManager))]
    static class NoTpciTelemetry
    {
        [HarmonyPatch("WriteLog")]
        [HarmonyPatch("sendEventWithPrewrittenMetadata")]
        [HarmonyPatch(nameof(TelemetryManager.Send))]
        [HarmonyPatch(nameof(TelemetryManager.AddToQueue))]
        [HarmonyPrefix]
        static bool SilenceTpciTelemetry() => false;
    }

    [HarmonyPatch(typeof(InsightsAPI))]
    static class NoNewRelic
    {
        [HarmonyPatch("SendPostQueueYield_Impl")]
        [HarmonyPrefix]
        static bool SendPostQueueYield1(ref IEnumerator __result)
        {
            __result = FakeEnumerator.INSTANCE;
            return false;
        }

        [HarmonyPatch(nameof(InsightsAPI.SendPostQueueYield))]
        [HarmonyPrefix]
        static bool SendPostQueueYield2(ref IEnumerator __result)
        {
            __result = FakeEnumerator.INSTANCE;
            return false;
        }

        [HarmonyPatch(nameof(InsightsAPI.PostYield), typeof(IInsightsEvent), typeof(IParamCommand<RestWorkerBase>), typeof(PostOptions))]
        [HarmonyPatch(nameof(InsightsAPI.PostYield), typeof(List<IInsightsEvent>), typeof(IParamCommand<RestWorkerBase>), typeof(PostOptions))]
        [HarmonyPrefix]
        static bool SendPostQueueYield3(ref IEnumerator __result)
        {
            __result = FakeEnumerator.INSTANCE;
            return false;
        }

        [HarmonyPatch(nameof(InsightsAPI.PostYield_Impl))]
        [HarmonyPrefix]
        static bool SendPostQueueYield4(ref IEnumerator __result)
        {
            __result = FakeEnumerator.INSTANCE;
            return false;
        }

        [HarmonyPatch(nameof(InsightsAPI.Post))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DummyOutMethod1(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [HarmonyPatch("ImmediatelyDispatchIfFull")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DummyOutMethod2(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [HarmonyPatch(nameof(InsightsAPI.AddToPostQueue))]
        [HarmonyPrefix]
        static bool AddToPostQueue(InsightsAPI __instance, ref InsightsAPI __result)
        {
            __result = __instance;
            return false;
        }
    }
}
