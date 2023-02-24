using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(AvatarCustomizationPlayerInventory))]
    internal static class OwnAllAvatarCustomizations
    {
        static bool Prepare(MethodBase original) => Plugin.Settings.EnableAllCosmetics;

        [HarmonyPatch(nameof(AvatarCustomizationPlayerInventory.IsOwned))]
        [HarmonyPrefix]
        static bool YouNowHaveAllTheOutfits(ref bool __result)
        {
            if (!Plugin.Settings.EnableAllCosmetics) return true;

            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(HUBDeckCustomizationController))]
    internal static class OwnAllDeckCustomizations
    {
        static bool Prepare(MethodBase original) => Plugin.Settings.EnableAllCosmetics;

        [HarmonyPatch("IsOwned")]
        [HarmonyPrefix]
        static bool IsOwned(ref bool __result)
        {
            if (!Plugin.Settings.EnableAllCosmetics) return true;

            __result = true;
            return false;
        }
    }
}