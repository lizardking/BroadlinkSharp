using BroadlinkSharp;
using System;
using System.Linq;

namespace BroadlinkSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime s = DateTime.Now;
            Dooya D = BroadLinkDiscovery.Discover(0, null).First() as Dooya; ;
            Console.WriteLine($"Discover: {(DateTime.Now - s).TotalMilliseconds:0}ms");

            s = DateTime.Now;
            D.Authorize();
            Console.WriteLine($"Auth: {(DateTime.Now - s).TotalMilliseconds:0}ms");

            s = DateTime.Now;
            D.Close();
            Console.WriteLine($"Close command: {(DateTime.Now - s).TotalMilliseconds:0}ms");

            //DateTime s = DateTime.Now;
            //while ((DateTime.Now-s).TotalSeconds<20 && !Console.KeyAvailable)
            //{
            //    Console.WriteLine(D.GetPercentage());
            //    System.Threading.Thread.Sleep(500);
            //}

            Console.WriteLine("");
            bool exitloop = false;
            while (!exitloop)
            {
                if(Console.KeyAvailable)
                {
                    char key = Console.ReadKey().KeyChar;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    switch (key)
                    {
                        case 'o':
                            D.Open();
                            Console.WriteLine("Open");
                            Console.WriteLine("");

                            break;
                        case 'c':
                            D.Close();
                            Console.WriteLine("Close");
                            Console.WriteLine("");

                            break;
                        case 's':
                            D.Stop();
                            Console.WriteLine("Stop");
                            Console.WriteLine("");
                            break;
                        case 'x':
                            exitloop = true;
                            break;
                        default:
                            Console.WriteLine("Unsupported Key");
                            Console.WriteLine("");
                            break;
                    }
                } else
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine($"Curtain {D.GetPercentage()}% closed        ");
                }

            }


          
        }
    }
}
