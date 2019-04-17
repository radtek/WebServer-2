using System;

namespace MetasiteWebServer_2
{
   public class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer();
            ws.Listner();

           
            Console.ReadLine();
           
        }
    }
}
