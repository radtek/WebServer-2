using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MestasiteWebServerTests
{
    [TestFixture]
    public class WebServerUnitTest
    {
        [Test]
        public void HttpProtocol_test()
        {
            var expected = new Version(2, 0);

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:1234/test.txt/");
                var request1 = new HttpRequestMessage(HttpMethod.Get, "http://127.0.0.1:1234/test.txt/");
                request.Version = new Version(2, 0);

                var task1 = httpClient.SendAsync(request);
                Task.WaitAll(task1);

                var actual = task1.Result.Version;

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void FileExistsInFolder_test()
        {
            int expected = 200;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://localhost:1234/test.txt");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            var actual = (int)webResponse.StatusCode;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FileNotFound_test()
        {
            int expected = 404;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://localhost:1234/testas.txt");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            var actual = (int)webResponse.StatusCode;
            Assert.AreEqual(expected, actual);
        }



    }

}