﻿/*************************************************************************
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

using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Rainier.NativeOmukadeConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string VERSION_STRING = "Rainier Redirector 2.0.0 \"NOC\"";
        internal const string OMUKADE_VERSION = "Omukade Cheyenne-EX";

        internal static ManualLogSource SharedLogger;
        internal static ConfigurationSettings Settings;

        private void Awake()
        {
            SharedLogger = Logger;

            if (!Environment.GetCommandLineArgs().Contains("--enable-omukade"))
            {
                SharedLogger.LogWarning("Omukade not enabled by command-line; goodbye");
                return;
            }
            SharedLogger.LogWarning($"CMD Line Args is: {string.Join(" ", Environment.GetCommandLineArgs())}");
            SharedLogger.LogWarning($"CMD Line is: {Environment.CommandLine}");

            if (File.Exists("config.json"))
            {
                SharedLogger.LogMessage("Found config file");
                Settings = JsonConvert.DeserializeObject<ConfigurationSettings>(File.ReadAllText("config.json"));
            }
            else
            {
                SharedLogger.LogWarning("No Config File Found! Using Dev defaults...");
                Settings = new ConfigurationSettings();
            }

            SharedLogger.LogMessage($"Omukade endpoint set to {Settings.OmukadeEndpoint}");


            // Plugin startup logic
            SharedLogger.LogInfo($"Loading Rainier PWR...");

            new HarmonyLib.Harmony("RainierPrimativeWebsocketRedirector").PatchAll();

            SharedLogger.LogInfo($"Applied Patches");
        }
    }
}
