/*************************************************************************
* Rainier Native Omukade Connector
* (c) 2022 Hastwell/Electrosheep Networks 
* 
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published
* by the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Affero General Public License for more details.
* 
* You should have received a copy of the GNU Affero General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
**************************************************************************/

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

namespace Rainier.NativeOmukadeConnector.Patches
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
