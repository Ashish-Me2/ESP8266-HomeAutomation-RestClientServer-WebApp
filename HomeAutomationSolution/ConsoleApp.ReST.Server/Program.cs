using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ReST.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Specify the URI to use for the local host:
            string baseUri = "http://chanti-banti:8080";

            Console.WriteLine("Starting web Server...");
            WebApp.Start<Startup>(baseUri);
            Console.WriteLine("Server running at {0} - press Enter to quit. ", baseUri);
            Console.ReadLine();
        }
    }
}
