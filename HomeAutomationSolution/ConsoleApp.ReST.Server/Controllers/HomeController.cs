using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

// Add these usings:
using System.Web.Http;
using System.Net.Http;
using ConsoleApp.ReST.Server.Models;
using System.IO;
using System.Net;

namespace ConsoleApp.ReST.Server.Controllers
{
    /// <summary>
    /// Author: ASHISH MATHUR
    /// </summary>
    public class HomeController : ApiController
    {
        private static Home myHome = null;
        private static string ControlFileName = "AMHomeController.json";
        public HomeController()
        {
            string controlData = File.ReadAllText(ControlFileName);
            myHome = JsonConvert.DeserializeObject<Home>(controlData);
        }

        /// <summary>
        /// Gets current state of all devices for refreshing the UI
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAllDeviceStatus()
        {
            string controlData = String.Empty;
            HttpResponseMessage response;
            try
            {
                controlData = File.ReadAllText(ControlFileName);
                myHome = JsonConvert.DeserializeObject<Home>(controlData);
                response = Request.CreateResponse(HttpStatusCode.OK, controlData);
            }
            catch (Exception exp)
            {
                controlData = "Could not fetch current device data due to an error.<BR/>Please try after some time.";
                response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, controlData);
            }
            return response;
        }

        /// <summary>
        /// Get specific room's device statuses
        /// </summary>
        /// <param name="RoomName"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRoomDeviceStatus([FromUri] string RoomName)
        {
            try
            {
                Room _room = myHome.Rooms.Find(r => r.RoomID.Equals(RoomName, StringComparison.CurrentCultureIgnoreCase));
                string deviceStatusString = String.Empty;
                _room.Devices.ForEach(d => {
                    deviceStatusString += d.DeviceID + "=" + ((d.DeviceState == 1) ? "ON" : "OFF") + ",";
                });
                deviceStatusString = deviceStatusString.TrimEnd(',');
                return Request.CreateResponse(HttpStatusCode.OK, deviceStatusString);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
        
        [HttpPost]
        public HttpResponseMessage SetDeviceState([FromUriAttribute] string DeviceName, [FromUriAttribute] string RoomName, [FromUriAttribute] int NewState)
        {
            HttpResponseMessage response = null;
            try
            {
                //Implicit refresh states
                GetAllDeviceStatus();
                Room _room = myHome.Rooms.Find(r => r.RoomID.Equals(RoomName, StringComparison.CurrentCultureIgnoreCase));
                Device _device = _room.Devices.Find(d => d.DeviceID.Equals(DeviceName, StringComparison.CurrentCultureIgnoreCase));
                _device.DeviceState = NewState;
                SaveState();
                response = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception exp)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotModified);
            }
            return response;
        }

        /// <summary>
        /// Saves the current devices state to JSON
        /// </summary>
        private void SaveState()
        {
            object _lock = new object();
            lock (_lock)
            {
                string controlData = JsonConvert.SerializeObject(myHome);
                StreamWriter fs = null;
                try
                {
                    if (File.Exists(ControlFileName))
                    {
                        //Do something if needed.
                    }
                    fs = new StreamWriter(ControlFileName);
                    fs.Write(controlData);

                }
                catch (Exception exp)
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OriginalRoomName"></param>
        /// <param name="NewRoomName"></param>
        /// <returns></returns>
        private Room AddorUpdateRoom(string OriginalRoomName, string NewRoomName = null)
        {
            Room _room;
            _room = myHome.Rooms.Find(r => r.RoomID.Equals(OriginalRoomName, StringComparison.CurrentCultureIgnoreCase));
            if (_room != null)
            {
                //Exists
                if (!String.IsNullOrEmpty(NewRoomName))
                {
                    _room.RoomID = NewRoomName;
                }
                else
                {
                    throw new ApplicationException("New room name not specified.");
                }
            }
            else
            {
                //Does not exist
                _room = (new Room());
                _room.RoomID = OriginalRoomName;
            }
            return _room;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceName"></param>
        /// <param name="RoomName"></param>
        /// <param name="NewDeviceName"></param>
        /// <param name="DeviceStatus"></param>
        /// <returns></returns>
        private Device AddorUpdateDevice(string DeviceName, string RoomName, string NewDeviceName, int DeviceStatus = 0)
        {
            Room _room = myHome.Rooms.Find(r => r.RoomID.Equals(RoomName, StringComparison.CurrentCultureIgnoreCase));

            //Check if room exists
            if (_room == null)
            {
                throw new ApplicationException("Specified room does not exist in the system.");
            }

            //All OK
            Device _device = _room.Devices.Find(d => d.DeviceID.Equals(DeviceName, StringComparison.CurrentCultureIgnoreCase));
            if (_device != null)
            {
                //Exists
                if (!String.IsNullOrEmpty(NewDeviceName))
                _device.DeviceID = NewDeviceName;
            }
            else
            {
                //Does not exist
                _device = (new Device(DeviceName, DeviceStatus));
                _room.Devices.Add(_device);
            }
            return _device;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myHome"></param>
        /// <returns></returns>
        private Home SetupMyHome(Home myHome)
        {
            myHome.HomeID = "Ashish Mathur's Home";
            if (myHome.Rooms == null)
            {
                myHome.Rooms = new List<Room>();
            }
            List<string> roomNames = new List<string> { "Drawing Room", "Living Room", "Kids' Room", "Master BedRoom", "Study Room", "Kitchen" };
            List<string> deviceNames = new List<string> { "Tubelight", "Fan", "Bulb" };

            List<Device> _devices = new List<Device>();
            deviceNames.ForEach(d =>
            {
                _devices.Add(new Device(d, 0));
            });

            roomNames.ForEach(r =>
            {
                myHome.Rooms.Add(new Room(r, _devices));
            });

            return myHome;
        }
    }
}
