using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTApp.Models
{
    public class CommandPacket
    {
        [JsonProperty("nodeID")]
        public uint NodeID;

        [JsonProperty("command")]
        public string Command;

        [JsonProperty("params")]
        public object Param1;
    }
}
