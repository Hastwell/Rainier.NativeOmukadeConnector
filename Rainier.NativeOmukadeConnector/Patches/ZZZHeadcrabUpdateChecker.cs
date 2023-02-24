using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(GameSettingsEndpoint), "Initialize")]
    internal static class ZZZHeadcrabUpdateChecker
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            using StreamWriter writer = new StreamWriter("rnoc-game-settings-endpoints.log");
            writer.WriteLine("GSES.GetConfigPath: " + GameSettingsEndpointSettings.instance.GetConfigPath());

            writer.Close();
        }
    }
}
