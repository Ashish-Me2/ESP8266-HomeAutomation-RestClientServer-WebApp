#include <ESP8266WiFi.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>
#include <ESP8266HTTPClient.h>

//const char *ssid = "ASHISH-MATHUR"; 
//const char *password = "sdl123456";

const char *ssid = "ZOMBIE"; 
const char *password = "CHANTI1-BANTI2";

int reconnectDelay = 1000; //AM: Use this to implement a circuit breaker for no connectivity to Wifi or the HTTP endpoint
int noDataCounter = 0;
bool stopRetries = false;
String AzureAPIUri = "http://homeautomationapi.azurewebsites.net/";
uint8 const TUBELIGHT = 0;
uint8 const BULB = 2;


void ControlGPIO(String stateString) {
	//Segments of device's state data, tokenized on COMMA
	int device1 = stateString.indexOf(",");
	int device2 = stateString.indexOf(",", device1+1);

	//Segments of device's name & state data, tokenized on =
	String dev1Segment = stateString.substring(0, device1);
	
	String dev2Segment = stateString.substring(device1 + 1, device2);
	//Serial.println(dev2Segment);
	String dev3Segment = stateString.substring(device2+1, stateString.length());

	//Now pick device name and state
	String dev1Name = dev1Segment.substring(0, dev1Segment.indexOf("="));
	String dev1State = dev1Segment.substring(dev1Segment.indexOf("=")+1, dev1Segment.length());

	String dev2Name = dev2Segment.substring(0, dev2Segment.indexOf("=") );
	String dev2State = dev2Segment.substring(dev2Segment.indexOf("=") + 1, dev2Segment.length());
	
	String dev3Name = dev3Segment.substring(0, dev3Segment.indexOf("="));
	String dev3State = dev3Segment.substring(dev3Segment.indexOf("=") + 1, dev3Segment.length());

	digitalWrite(TUBELIGHT, dev1State == "ON" ? HIGH : LOW);
	digitalWrite(BULB, dev3State == "ON" ? HIGH : LOW);
}

void setup() {
	delay(1000);
	pinMode(0, OUTPUT); //TUBE CONTROL
	pinMode(2, OUTPUT); //BULB CONTROL
	
	Serial.begin(115200);
	WiFi.mode(WIFI_OFF);        //Prevents reconnection issue (taking too long to connect)
	delay(1000);
	WiFi.mode(WIFI_STA);        
	//Serial.print("Device MAC address: ");
	//Serial.println(WiFi.macAddress());  //MAC address of ESP
	WiFi.begin(ssid, password);
	//Serial.println("");
	//Serial.print("Connecting");
	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	//If connection successful show IP address in serial monitor
	//Serial.println("");
	//Serial.print("Connected to ");
	//Serial.println(ssid);
	//Serial.print("IP address: ");
	//Serial.println(WiFi.localIP());  //IP address assigned to ESP
}

void loop() {
	HTTPClient http;
	String Link;
	String roomName = "Living%20Room";
	String apiPath = "api/home/GetRoomDeviceStatus?RoomName=";
	
	Link = AzureAPIUri + apiPath + roomName;
	//Serial.println(Link);

	//----------------------------------------------------------------------------------------------------
	if (noDataCounter > 15) {
		if (!stopRetries) {
			Serial.println("**************************************");
			Serial.println("I'm tired of retrying. Please fix the server side and reset the processor (ESP) before trying again. No more automatic retries. Zzzzz");
			Serial.println("**************************************");
			stopRetries = true;
		}
		return;
	}
	//----------------------------------------------------------------------------------------------------
	http.begin(Link);     
	int httpCode = http.GET();    
	
	if (httpCode > 0) {
		noDataCounter = 0;
		reconnectDelay = 3000;
		String payload = http.getString();
		//Serial.print("HTTP Status Code: ");
		//Serial.println(httpCode);
		//Serial.print("HTTP Response Payload: ");
		Serial.println(payload);
		http.end();
		String trimmedPayload = payload.substring(1,payload.length()-1);
		ControlGPIO(trimmedPayload);
		delay(reconnectDelay);
	}
	else {
		Serial.println("-----------------------------------");
		Serial.print("No connection to server. Waiting for: ");
		Serial.print(reconnectDelay/1000);
		Serial.println(" seconds before re-trying...");
		noDataCounter++;
		reconnectDelay = reconnectDelay * 2;
		delay(reconnectDelay);
	}
}