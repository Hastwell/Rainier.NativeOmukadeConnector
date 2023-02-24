#if FALSE
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch]
    public static class GameManagerLoadingTextPatches
    {
        internal static List<string> loadingManagerNames = new List<string>(0);

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> PatchMethod(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo setTotalStepsMethod = SymbolExtensions.GetMethodInfo(() => ((StartupScreenText)null).SetTotalSteps(47));
            int gameManagerCapturePhase = 0;
            int fieldNumberOfGameManger = -1;

            foreach(CodeInstruction instruction in instructions)
            {
                if(gameManagerCapturePhase == 0 && instruction.opcode == OpCodes.Ldfld && ((FieldInfo) instruction.operand).FieldType == typeof(GameManager))
                {
                    gameManagerCapturePhase = 1;
                    yield return instruction;
                }
                else if(gameManagerCapturePhase == 1 && instruction.IsStloc())
                {
                    if (instruction.opcode == OpCodes.Stloc_0) fieldNumberOfGameManger = 0;
                    else if (instruction.opcode == OpCodes.Stloc_1) fieldNumberOfGameManger = 1;
                    else if (instruction.opcode == OpCodes.Stloc_2) fieldNumberOfGameManger = 2;
                    else if (instruction.opcode == OpCodes.Stloc_3) fieldNumberOfGameManger = 3;
                    else if (instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S) fieldNumberOfGameManger = (int)instruction.operand;
                    else throw new InvalidOperationException("Unmatched STLOC while patching");

                    gameManagerCapturePhase = 2;

                    yield return instruction;
                }
                /*else if(gameManagerCapturePhase == 2 && instruction.Calls(setTotalStepsMethod))
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloc_S, (short)fieldNumberOfGameManger);

                    FieldInfo loadingManagersListFi = typeof(GameManager).GetField("loadingManagersList", BindingFlags.Instance | BindingFlags.NonPublic);
                    yield return new CodeInstruction(OpCodes.Ldfld, loadingManagersListFi);
                    yield return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => UpdateInitializedControllers));
                }*/
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void UpdateInitializedControllers(List<Tuple<int, ManagerBase>> loadingManagersList)
        {
            loadingManagerNames = loadingManagersList.Select(m => m.Item2.name).ToList();
        }
    }
}
#endif