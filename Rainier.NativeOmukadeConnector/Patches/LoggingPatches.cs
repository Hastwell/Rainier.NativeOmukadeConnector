﻿using HarmonyLib;
using Platform.Sdk;
using SharedLogicUtils.source.Logging;
using SharedLogicUtils.source.Logging.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using TPCI.Networking;

namespace Rainier.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(TPCI.Networking.DefaultNetworkController), nameof(DefaultNetworkController.ClientSetupTask))]
    internal static class NetworkController_UseRealLoggers
    {
        [HarmonyPatch]
        [HarmonyPrefix]
        static void UseRealLoggers(ref ILogger iLogger, ref IClientLogger platformLogger, ref Action<string> rainierSdkLog)
        {
            ClientLoggerImpl clientLoggerImpl = new();
            iLogger = clientLoggerImpl;
            platformLogger = clientLoggerImpl;
            rainierSdkLog = Plugin.SharedLogger.LogInfo;
            Plugin.SharedLogger.LogInfo("UseRealLoggers armed all client loggers");
        }
    }

    internal class ClientLoggerImpl : IClientLogger, ILogger
    {
        public void Error(string message) => Plugin.SharedLogger.LogError(message);

        public void Log(string message) => Plugin.SharedLogger.LogInfo(message);

        public void Log(string message, LogFlag flags = LogFlag.Log)
        {
            if(flags != LogFlag.Error)
            {
                Plugin.SharedLogger.LogInfo(message);
            }
            else
            {
                Plugin.SharedLogger.LogError(message);
            }
        }
    }
}
