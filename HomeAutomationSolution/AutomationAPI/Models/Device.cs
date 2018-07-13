using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAPI.Models
{
    public class Device
    {
        public Device() { }
        public Device(string DeviceID, int DeviceState)
        {
            this.DeviceID = DeviceID;
            this.DeviceState = DeviceState;
        }
        public string DeviceID { get; set; }
        public int DeviceState { get; set; }
    }
}
