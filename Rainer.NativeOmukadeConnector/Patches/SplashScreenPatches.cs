using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine.Events;

namespace Rainer.NativeOmukadeConnector.Patches
{
    internal static class SplashScreenPatches
    {
        static MethodInfo openNewMessageMethod = typeof(SimpleMessage).GetMethod("OpenNewMessage", new Type[] { typeof(string), typeof(string), typeof(string), typeof(UnityAction), typeof(UnityAction), typeof(bool) });
        static MethodInfo patchMethod = typeof(SplashScreenPatches).GetMethod(nameof(SplashScreenPatches.OpenNewMessageFake), BindingFlags.Static | BindingFlags.Public);

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(StartupScreenText.ShowStartupMessage))]
        static IEnumerable<CodeInstruction> ShowStartupMessageLogsToConsoleOnly(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(openNewMessageMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, patchMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void OpenNewMessageFake(SimpleMessage _smgInstance, string title, string message, string buttonName, UnityEngine.Events.UnityAction confirm, UnityEngine.Events.UnityAction closeAction, bool close)
        {
            bool isFatal = closeAction == ManagerSingleton<GameManager>.instance.RestartProject;

            if (isFatal)
            {
                Plugin.SharedLogger.LogFatal("Fatal Announcement: " + message);
            }
            else
            {
                Plugin.SharedLogger.LogFatal("Announcement: " + message);
            }

            _smgInstance.OpenNewMessage(title, message, buttonName, confirm, closeAction, close);
        }
    }
}
