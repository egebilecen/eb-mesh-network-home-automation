using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTApp.Models
{
    public class SensorData
    {
        public uint     NodeID   { get; set; }
        public uint     NodeType { get; set; }
        public double   Value    { get; set; }
        public DateTime Time     { get;      } = DateTime.Now;
    }
}
