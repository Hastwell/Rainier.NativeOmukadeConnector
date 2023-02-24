using System;
using System.Collections.Generic;
using System.Text;

namespace Rainer.NativeOmukadeConnector.Messages
{
    public class OnlineFriendsResponse
    {
        /// <summary>
        /// All requested players that are currently online.
        /// </summary>
        public List<string> CurrentlyOnlineFriends { get; set; }
        public uint TransactionId { get; set; }
    }
}
