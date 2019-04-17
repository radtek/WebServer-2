using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

/* The HTTP Upgrade mechanism is used to establish HTTP/2 starting from plain HTTP. The client starts an HTTP/1.1 connection and sends an Upgrade: h2c header. 
   If the server supports HTTP/2, it replies with HTTP 101 Switching Protocol status code. The HTTP Upgrade mechanism is used only for cleartext HTTP2 (h2c). 
   In the case of HTTP2 over TLS (h2), the ALPN TLS protocol extension is used instead.*/

namespace MetasiteWebServer_2
{
    public class WebServer
    {
        private TcpListener tcpListener;
     

        public WebServer()
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
        }

        public void Listner()
        {
            ThreadPool.QueueUserWorkItem((x) =>
            {
                tcpListener.Start();
                Console.WriteLine("Waiting connections");

                while (true)
                {
                    
                    TcpClient client = tcpListener.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem((h) =>
                    {
                        DownloadFile(client);

                    });
                }
            });
        }


        private void DownloadFile(object client)
        {

            TcpClient tcpClient = (TcpClient)client;
            StreamReader sr = new StreamReader(tcpClient.GetStream());
            StreamWriter sw = new StreamWriter(tcpClient.GetStream());

                try
                {
                   // string req = sr.ToString();
                    string requst = sr.ReadLine();
                    Console.WriteLine(requst);
                    

                    string[] tokens = requst.Split(' ');
                    string filename = tokens[1].Substring(1);
                    StreamReader file = new StreamReader(@"C:\temp\test\" + filename);


                   //    StringBuilder builder2 = new StringBuilder();
                   //     builder2.AppendLine(@"HTTP/1.1 101 Switching Protocols");
                   //     builder2.AppendLine(@"Connection: Upgrade");
                   //     builder2.AppendLine(@"Upgrade: h2c");

                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(@"HTTP/2.0 200 OK");
                    builder.AppendLine(@"Content-Type: text/plain");
                    builder.AppendLine(@"Date: " + DateTime.Now.ToString());
                    builder.AppendLine(@"Status: 200 OK ");
                    builder.AppendLine(@"Content-Disposition:" + $"attachment; filename={filename}");

                    sw.WriteLine(builder);

                    string data = file.ReadLine();
                    while (data != null)
                    {

                        sw.WriteLine(data);
                        sw.Flush();
                        data = file.ReadLine();

                    }
                

                }catch (Exception ex)
                {
                    sw.WriteLine("HTTP/1.1 404 OK\n");
                    sw.WriteLine("<H1> File not found </H1>");
                    sw.Flush();
                }
             
               tcpClient.Close();
         

        }
      
    }
}

