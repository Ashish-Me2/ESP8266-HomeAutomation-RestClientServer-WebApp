#include <ESP8266WiFi.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>
#include <ESP8266HTTPClient.h>


const char *ssid = "ZOMBIE"; 
const char *password = "CHANTI1-BANTI2";

const char *host = "http://chanti-banti:8080/api/home/GetRoomDeviceStatus?RoomName=Living%20Room";

void setup() {
	delay(1000);
	Serial.begin(115200);
	WiFi.mode(WIFI_OFF);        //Prevents reconnection issue (taking too long to connect)
	delay(1000);
	WiFi.mode(WIFI_STA);        

	WiFi.begin(ssid, password);
	Serial.println("");
	Serial.print("Connecting");
	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	//If connection successful show IP address in serial monitor
	Serial.println("");
	Serial.print("Connected to ");
	Serial.println(ssid);
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());  //IP address assigned to your ESP
}

void loop() {
	HTTPClient http;
	String Link;
	//GET Data
	Link = "http://chanti-banti:8080/api/home/GetRoomDeviceStatus?RoomName=Living%20Room";
	http.begin(Link);     
	int httpCode = http.GET();           
	String payload = http.getString();   
	Serial.print("HTTP Status Code: ");   
	Serial.println(httpCode);
	Serial.print("HTTP Response Payload: ");
	Serial.println(payload);    
	http.end();
	delay(1000);
}