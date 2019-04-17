using System;

namespace MetasiteWebServer_1
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer("http://localhost:1234/");
            ws.Start();
            Console.WriteLine("Webserver Started. Press any key to quit.");
            Console.ReadKey();
            ws.Stop();
        }
    }
}
