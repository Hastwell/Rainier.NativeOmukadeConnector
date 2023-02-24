using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TPCI.Build;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(StartupScreenText))]
    static class VersionStringPatches
    {
        [HarmonyPatch(nameof(StartupScreenText.SetVersionTextWithUser))]
        [HarmonyPrefix]
        static bool SetVersionTextWithUser(string user, TextMeshProUGUI ___versionText)
        {
            ___versionText.SetText(user + "\n" + GameVersionInfo.instance.GetGameVersion() + "\n" + GetOmukadeString());
            return false;
        }

        [HarmonyPatch(nameof(StartupScreenText.SetVersionText))]
        [HarmonyPrefix]
        static bool SetVersionText(TextMeshProUGUI ___versionText)
        {
            ___versionText.SetText(GameVersionInfo.instance.GetGameVersion() + "\n" + GetOmukadeString());
            return false;
        }

        static string GetOmukadeString() => $"{Plugin.VERSION_STRING}\n{Plugin.OMUKADE_VERSION} - {Plugin.Settings.OmukadeEndpoint}";
    }
}
