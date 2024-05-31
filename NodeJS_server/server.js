const WebSocketServer = require("websocket").server;
const http   = require("http");
const ip     = require("ip");
const ros    = require("rosnodejs");
const crypto = require("crypto");
const std_msgs = ros.require('std_msgs').msg;

var connectionList = {};
var appDataPub;

function GenerateID()
{
    var id = crypto.randomBytes(16).toString("hex");

    if(id in connectionList)
        return GenerateID();

    return id;
}

var server = http.createServer(function(request, response) {
    console.log((new Date()) + " Received request for " + request.url);
    response.writeHead(404);
    response.end();
});

server.listen(8080, function() {
    console.log((new Date()) + " Server is listening on "+ip.address()+":8080");
});

wsServer = new WebSocketServer({
    httpServer: server,
    autoAcceptConnections: false
});

function originIsAllowed(origin) 
{
    // console.log("Origin: "+origin);
    return true;
}

// Websocket Server
wsServer.on("request", function(request) 
{
    if (!originIsAllowed(request.origin)) 
    {
      request.reject();
      console.log((new Date()) + " Connection from origin " + request.origin + " rejected.");
      return;
    }
    
    var connection = request.accept(null, request.origin);
    connection.clientID = GenerateID();

    connectionList[connection.clientID] = connection;
    console.log((new Date()) + " Connection "+connection.clientID+" accepted.");

    connection.on("message", function(message) {
        if(message.type === "utf8") 
        {
            if(message.utf8Data == "ping"
            || message.utf8Data == "pong") return;

            const msg = new std_msgs.String();
            msg.data = message.utf8Data;
            appDataPub.publish(msg);
        }
    });
    
    connection.on("close", function(reasonCode, description) {
        console.log((new Date()) + " Peer "+connection.clientID+" disconnected.");
        delete connectionList[connection.clientID];
    });
});

// ROS Server
ros.initNode('/iot_app_server').then((rosNode) => {
    appDataPub = rosNode.advertise("/app_data", std_msgs.String);

    rosNode.subscribe('/sensor_data', std_msgs.String, (data) => {
        var data = data.data;

        for(const clientID in connectionList)
            connectionList[clientID].sendUTF(data);
    });

    rosNode.subscribe('/mesh_info', std_msgs.String, (data) => {
        var data = data.data;

        for(const clientID in connectionList)
        {
            connectionList[clientID].sendUTF(JSON.stringify({
                type : "mesh_info",
                data : JSON.parse(data)
            }));
        }
    });
});
