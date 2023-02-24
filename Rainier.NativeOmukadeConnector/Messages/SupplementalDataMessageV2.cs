using Newtonsoft.Json;
using Platform.Sdk.Models.Inventory;
using Platform.Sdk.Models.User;
using SharedLogicUtils.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rainier.NativeOmukadeConnector.Messages
{
    public class SupplementalDataMessageV2
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PlayerId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CurrentRegion { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CollectionData DeckInformation { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Outfit OutfitInformation { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PlayerDisplayName { get; set; }
    }
}
