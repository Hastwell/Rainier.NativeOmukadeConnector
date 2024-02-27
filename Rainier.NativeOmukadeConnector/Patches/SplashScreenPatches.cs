#if FALSE
// This doens't really seem to be used, and my method equals doesn't seem to work...?
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine.Events;

namespace Rainier.NativeOmukadeConnector.Patches
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
#endif