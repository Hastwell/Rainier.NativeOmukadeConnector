using System;
using System.Collections.Generic;
using System.Text;

namespace Rainier.NativeOmukadeConnector
{
    internal class ConfigurationSettings
    {
        public string OmukadeEndpoint = "ws://localhost:18181";
        public bool ForceFriendsToBeOnline = false;
        public bool EnableAllCosmetics = false;
        public bool ForceAllLegalityChecksToSucceed = false;
        public bool DumpManifestFileUrl = false;
    }
}
