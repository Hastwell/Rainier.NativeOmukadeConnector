using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(RainierLogHandler))]
    internal class BetterExceptionLogger
    {
        [HarmonyPatch(nameof(RainierLogHandler.LogException))]
        [HarmonyPrefix]
        internal static bool LogException(Exception exception)
        {
            Plugin.SharedLogger.LogError(PrepareExceptionString(exception));

            return false;
        }

        static internal string PrepareExceptionString(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(exception.GetType().FullName);
            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                sb.Append(" - ");
                sb.Append(exception.Message);
            }
            sb.AppendLine();
            sb.AppendLine("Stack Trace:");

            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(exception);
            PrepareStacktraceString(sb, st);

            return sb.ToString();
        }

        static internal void PrepareStacktraceString(StringBuilder sb, StackTrace st)
        {
            foreach (System.Diagnostics.StackFrame frame in st.GetFrames())
            {
                try
                {
                    MethodBase frameMethod = frame.GetMethod();
                    if (frameMethod == null)
                    {
                        sb.Append("[null method] - at IL_");
                    }
                    else
                    {
                        string frameClass = frameMethod.DeclaringType.FullName;
                        string frameMethodDisplayName = frameMethod.Name;
                        int frameMetadataToken = frameMethod.MetadataToken;
                        sb.Append($"{frameClass}::{frameMethodDisplayName} @{frameMetadataToken:X8} - at IL_");
                    }
                    sb.Append(frame.GetILOffset().ToString("X4"));
                    sb.AppendLine();
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"[error on this frame - {ex.GetType().FullName} - {ex.Message}]");
                }
            }
        }
    }
}
