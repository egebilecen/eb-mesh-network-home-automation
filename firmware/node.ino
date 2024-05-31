#include "painlessMesh.h"
#include <Arduino_JSON.h>
#include "src/node_typedef.h"

#define LED_BUILTIN 2

#include "src/sensor/lm35.h"

#define ROOT_NODE 0
#define NODE_ID   2
#define NODE_TYPE NODE_TYPE_NONE
#define NODE_NAME "TODO"

#define MESH_PREFIX   "IOT_APP"
#define MESH_PASSWORD "159753456"
#define MESH_PORT     5555

Scheduler scheduler;
painlessMesh mesh;
// nodeTask.setInterval(random(TASK_SECOND * 1, TASK_SECOND * 5));

#if ROOT_NODE
    #undef ESP8266
    #include <ros.h>
    #include <std_msgs/String.h>
    #define ESP8266

    unsigned long lastSpin = 0;

    void appDataCallback(const std_msgs::String& msg)
    {
        mesh.sendBroadcast(msg.data, true);
    }

    ros::NodeHandle  nh;
    std_msgs::String strMsg;
    ros::Publisher   sensorDataTopic("sensor_data", &strMsg);
    ros::Publisher   meshInformationTopic("mesh_info", &strMsg);
    ros::Subscriber<std_msgs::String> appData("app_data", &appDataCallback);
#endif

// TASK FUNCTION DECLARATIONS
#if ROOT_NODE 
void rootWork();
#endif
void nodeWork();

// TASK DEFINATIONS
#if ROOT_NODE
Task rootTask(TASK_SECOND * 5, TASK_FOREVER, &rootWork);
#endif
Task nodeTask(TASK_SECOND * 1, TASK_FOREVER, &nodeWork);

// TASK FUNCTION DEFINATIONS
#if ROOT_NODE
void rootWork()
{
    if(nh.connected())
    {
        String meshLayout = mesh.subConnectionJson();
        strMsg.data = meshLayout.c_str();
        meshInformationTopic.publish(&strMsg);
    }
}
#endif

void nodeWork() 
{
    StaticJsonDocument<512> json;
    json["type"] = "node_data";

    JsonObject data  = json.createNestedObject("data");
    JsonObject value = data.createNestedObject("value");

    data["nodeID"]   = NODE_ID;
    data["nodeType"] = NODE_TYPE;

    // Get data from sensor
#if NODE_ID == 1
    value[String(NODE_TYPE_TEMPERATURE_SENSOR_LM35)] = get_temperature_lm35(A0);
#endif

    String msg;
    serializeJson(json, msg);
    mesh.sendBroadcast(msg, true);
}

// Mesh Callback Functions
void receivedCallback(uint32_t from, String& msg) 
{
    StaticJsonDocument<512> json;
    DeserializationError error = deserializeJson(json, msg);

    String type = json["type"].as<String>();

    if(!error
    && type != null)
    {
    #if ROOT_NODE
        if(type != "app_data"
        && nh.connected())
        {
            strMsg.data = msg.c_str();
            sensorDataTopic.publish(&strMsg);
        }
    #endif
        
        if(type == "app_data")
        {
            auto data = json["data"].as<JsonObject>();

            if(data["nodeID"].as<uint32_t>() == NODE_ID)
            {
                int state = data["state"].as<uint32_t>();
                digitalWrite(LED_BUILTIN, !state);
            }
        }
    }
}

void setup() 
{
    pinMode(LED_BUILTIN, OUTPUT);
    digitalWrite(LED_BUILTIN, HIGH);

#if ROOT_NODE
    nh.getHardware()->setPort(&Serial);
    nh.getHardware()->setBaud(115200);
    
    nh.initNode();
    nh.advertise(sensorDataTopic);
    nh.advertise(meshInformationTopic);
    nh.subscribe(appData);
#endif

    // ERROR | MESH_STATUS | CONNECTION | SYNC | COMMUNICATION | GENERAL | MSG_TYPES | REMOTE | STARTUP
    mesh.setDebugMsgTypes(0);
    mesh.init(MESH_PREFIX, MESH_PASSWORD, &scheduler, MESH_PORT, WIFI_AP_STA, 6);

#if ROOT_NODE
    mesh.setRoot(true);
#else
    mesh.setRoot(false);
#endif

    mesh.setContainsRoot(true);
    mesh.onReceive(&receivedCallback);

    // TASK SCHEDULING
#if ROOT_NODE
    scheduler.addTask(rootTask);
    rootTask.enable();
#endif
    scheduler.addTask(nodeTask);
    nodeTask.enable();
}

void loop() 
{
    mesh.update();

#if ROOT_NODE
    unsigned long now = millis();

    if(now - lastSpin >= 1000)
    {
        lastSpin = now;
        nh.spinOnce();
    }
#endif
}
