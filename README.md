# ESP8266-HomeAutomation-RestClientServer-WebApp
This is a fully functional and complete Wifi Home Automation system designed around the ESP8266 (and an optional Arduino Uno). The current system is modeled as an extensible design with an object heirarchy of:
Home -(HomeID)
  |Rooms -(RoomID)
      |Devices -(DeviceID, DeviceState)
      
The server-side is a simple WebAPI deployed on an Azure service on HTTP. Need to add Authentication and other security aspects to the plain vanilla implementation.
The client is a simple HTML page relying on jQuery and simple Javascript for all AJAX calls to the ReST API on the server and manipulation of the UI elements.
The device state is persisted on the server using a simple JSON file which can be maintained using the ReST API with Postman if required.
