using System;
using System.Collections.Generic;
using System.Text;

namespace IOTApp
{
    public static class NodeTypedef
    {
        public const int LED_CONTROLLER          = (1 << 0);
        public const int TEMPERATURE_SENSOR_LM35 = (1 << 1);
        public const int LIGHT_LEVEL_SENSOR_LDR  = (1 << 2);
    }
}
