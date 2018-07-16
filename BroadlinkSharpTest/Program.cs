using BroadlinkSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BroadlinkSharpTest
{
    /// <summary>
    /// Test program for the BroadlinkSharp library.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {


            DateTime s = DateTime.Now;

            List<BroadlinkDevice> devices = BroadLinkDiscovery.Discover(2000, null);

            Console.WriteLine($"Discover: {(DateTime.Now - s).TotalMilliseconds:0}ms");

            Console.WriteLine($"Found {devices.Count} Broadlink devices:");
            foreach (BroadlinkDevice device in devices)
            {
                Console.WriteLine($"  Class: {device.GetType().Name}, Description: {device.DeviceTypeDescription}, Code: {device.DeviceTypeCode}, Mac: {device.MacAddress.ToString()}");
            }
            Console.WriteLine();

            Dooya dooyaCurtainMotor = devices.Select(d => d as Dooya).FirstOrDefault();
            if(dooyaCurtainMotor==null)
            {
                Console.WriteLine($"Found no {nameof(Dooya)} device in the list of discovered devices.");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Will control {nameof(Dooya)} device with mac Address {dooyaCurtainMotor.MacAddress.ToString()}");
            Console.WriteLine("Press o to open, c to close and s to stop the curtain motor. Presss x to exit.");
            Console.WriteLine();

            s = DateTime.Now;
            dooyaCurtainMotor.Authorize();
            Console.WriteLine($"Authorized: {(DateTime.Now - s).TotalMilliseconds:0}ms");
            Console.WriteLine();
            Console.WriteLine();


            bool exitloop = false;
            while (!exitloop)
            {
                if(Console.KeyAvailable)
                {
                    char key = Console.ReadKey().KeyChar;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    s = DateTime.Now;
                    switch (key)
                    {
                        case 'o':
                            dooyaCurtainMotor.Open();
                            Console.WriteLine("Open");
                            break;
                        case 'c':
                            dooyaCurtainMotor.Close();
                            Console.WriteLine("Close");
                            break;
                        case 's':
                            dooyaCurtainMotor.Stop();
                            Console.WriteLine("Stop");
                            break;
                        case 'x':
                            exitloop = true;
                            break;
                        default:
                            Console.WriteLine("Unsupported Key");
                            break;
                    }
                    Console.WriteLine($"command duration: {(DateTime.Now - s).TotalMilliseconds:0}ms");
                    Console.WriteLine();
                } else
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    s = DateTime.Now;
                    int percentage = dooyaCurtainMotor.GetPercentage();
                    Console.WriteLine($"Curtain {dooyaCurtainMotor.GetPercentage()}% open. GetPercentage command duration: {(DateTime.Now - s).TotalMilliseconds:0} ms          ");
                }

            }


          
        }
    }
}
