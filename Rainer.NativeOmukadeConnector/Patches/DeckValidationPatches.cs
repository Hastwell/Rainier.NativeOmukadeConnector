using CardDatabase.DataAccess;
using HarmonyLib;
using SharedLogicUtils.Config;
using SharedSDKUtils.DeckValidation;
using SharedSDKUtils;
using System;
using System.Collections.Generic;
using System.Text;
using static SharedSDKUtils.DeckValidation.DeckValidationService;

namespace Rainer.NativeOmukadeConnector.Patches
{
    [HarmonyPatch(typeof(DeckValidationService), nameof(DeckValidationService.ValidateDeck), typeof(RulesFormat), typeof(Dictionary<string, int>), typeof(DeckInfo), typeof(IQueryableCardDatabase), typeof(IConfigLoader))]
    static class DeckValidationAlwaysIgnoreUnowned
    {
        static bool Prefix(ref DeckValidPackage __result, RulesFormat format, DeckInfo deck, IQueryableCardDatabase cardDatabase, IConfigLoader configLoader)
        {
            if (Plugin.Settings.ForceAllLegalityChecksToSucceed)
            {
                __result = new DeckValidPackage { entries = new DeckErrorEntry[0] };
                return false;
            }

            __result = DeckValidationService.ValidateDeckIgnoreUnowned(format, deck, cardDatabase, configLoader);
            return false;
        }
    }
}
