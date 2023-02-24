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

using CardDatabase.DataAccess;
using HarmonyLib;
using SharedLogicUtils.Config;
using SharedSDKUtils.DeckValidation;
using SharedSDKUtils;
using System;
using System.Collections.Generic;
using System.Text;
using static SharedSDKUtils.DeckValidation.DeckValidationService;

namespace Rainier.NativeOmukadeConnector.Patches
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
