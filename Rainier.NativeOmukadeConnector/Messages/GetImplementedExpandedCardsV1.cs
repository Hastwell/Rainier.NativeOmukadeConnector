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

namespace Omukade.Cheyenne.CustomMessages
{
    public class GetImplementedExpandedCardsV1
    {
        /// <summary>
        /// The checksum of the client's existing data. If this matches the server's checksum, fresh data will not be returned. If not provided, or the provided checksum does not match the server's data, fresh data will be returned.
        /// </summary>
        public string? Checksum;
    }
}
