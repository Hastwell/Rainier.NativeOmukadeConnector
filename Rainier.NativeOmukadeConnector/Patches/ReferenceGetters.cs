using HarmonyLib;
using RainierClientSDK.Inventory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch]
    internal static class ReferenceGetters
    {
        internal static PlatformCollectionService collectionServiceReference;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformCollectionService), nameof(PlatformCollectionService.GetCollectionsAsync))]
        static void Prefix(PlatformCollectionService __instance)
        {
            //Plugin.SharedLogger.LogInfo("Obtained the PlatformCollectionService reference!");
            collectionServiceReference = __instance;
        }
    }
}
