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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Omukade.Cheyenne.CustomMessages
{
    public class ImplementedExpandedCardsV1
    {
        /// <summary>
        /// A list of of valid card IDs, seperated by pipe symbols ("|"). If this value's checksum matched the one provided in <see cref="GetImplementedExpandedCardsV1.Checksum"/>, this value will be null.
        /// </summary>
        public string? RawImplementedCardNames;

        [JsonIgnore]
        public IEnumerable<string>? ImplementedCardNames
        {
            get
            {
                return RawImplementedCardNames?.Split('|');
            }
        }

        /// <summary>
        /// Checksum of valid card IDs. Always returned.
        /// </summary>
        public string Checksum;
    }
}
