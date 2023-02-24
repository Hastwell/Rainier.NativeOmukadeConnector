using HarmonyLib;
using RainierClientSDK.Inventory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(PlatformCollectionService), nameof(PlatformCollectionService.GetCollectionsAsync))]
    internal static class PlatformCollectionService_ReferenceGetter
    {
        internal static PlatformCollectionService INSTANCE;

        static void Prefix(PlatformCollectionService __instance)
        {
            //Plugin.SharedLogger.LogInfo("Obtained the PlatformCollectionService reference!");
            INSTANCE = __instance;
        }
    }
}
