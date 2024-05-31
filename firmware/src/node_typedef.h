#define NODE_TYPE_NONE                    0
#define NODE_TYPE_LED_CONTROLLER          (1 << 0) // Has LED connected to it.
#define NODE_TYPE_TEMPERATURE_SENSOR_LM35 (1 << 1) // Gets temperature readings using LM35 sensor.
#define NODE_TYPE_LIGHT_LEVEL_SENSOR_LDR  (1 << 2) // Gets the light level using LDR sensor.
