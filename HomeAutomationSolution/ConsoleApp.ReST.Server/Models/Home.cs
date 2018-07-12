using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ReST.Server.Models
{
    public class Home
    {
        public string HomeID { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
