using System;
using System.IO;
using System.Net;
using System.Threading;


/*  HTTP/2 in .NET Core:
     - HttpSysServer
     - Http Server API (http.sys)
     - supports HTTP/2 over TLS
 */ 



namespace MetasiteWebServer_1
{
    class WebServer
    {
        private readonly HttpListener listener = new HttpListener();

        public WebServer(string prefixes)
        {
            listener.Prefixes.Add(prefixes);
            listener.Start();
        }


        public void Start()
        {
            ThreadPool.QueueUserWorkItem((x) =>
            {  
                Console.WriteLine("Webserver running...");
                try
                {
                    while (listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((h) =>
                        {
                            var context = h as HttpListenerContext;
                            try
                            {
                               DownloadFile(context);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            } 
                            finally
                            {
                                context.Response.OutputStream.Close();
                            }
                        }, listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                } 
            });
        }

        public void DownloadFile(HttpListenerContext context)
        {

            string path;
            string filename = context.Request.Url.AbsolutePath;

            Console.WriteLine(filename);
            filename = filename.Substring(1);

            path = Path.Combine(@"C:\temp\test\", filename);
            if (File.Exists(path))
            {

                try
                {
                    // context.Response.AddHeader("Connection", "upgrade");
                    //  context.Response.AddHeader("Upgrade", "h2c");

                    Stream input = new FileStream(path, FileMode.Open);
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString());
                    context.Response.AppendHeader("Content-Disposition", $"attachment; filename={filename}");

                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = 0;

                    while ((bytesRead = input.Read(buffer, 0, bufferSize)) > 0)
                    {
                        context.Response.OutputStream.Write(buffer, 0, bytesRead);
                    }

                    input.Close();
                    context.Response.OutputStream.Flush();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }
    }
}

