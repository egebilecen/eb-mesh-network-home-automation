#include "lm35.h"

float get_temperature_lm35(int pin)
{
    float out    = analogRead(pin);
    float temp_c = out * 3300 / 1024 / 10; //Celcius

    return temp_c;
}
