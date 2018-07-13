using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAPI.Models
{
    public class Room
    {
        public Room(){}
        public Room(string RoomID, List<Device> Devices)
        {
            this.RoomID = RoomID;
            this.Devices = Devices;
        }
        public string RoomID { get; set; }
        public List<Device> Devices { get; set; }
    }
}
