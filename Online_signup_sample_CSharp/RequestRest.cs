/* This file is part of Sign-up Page Sample.

The Sign-up Page Sample is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Sign-up Page Sample is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with The Sign-up Page Sample.  If not, see <http://www.gnu.org/licenses/>. */
using System.Net.Http;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Web;

namespace Online_signup_sample_CSharp.Controllers
{
    public class RequestRest
    {
        public string requestUrl { get; set; }
        public string requestBody { get; set; }
        public string apiKey { get; set; }

        public static HttpClient client = new HttpClient();

        private void Headers(WebRequest request)
        {
            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("X-Apikey", apiKey);
        }
        public string Get()
        {
            WebRequest request = WebRequest.Create(requestUrl);
            Headers(request);
            WebResponse response = request.GetResponse();
            string responseString = ReadStream(response.GetResponseStream());
            response.Close();
            return responseString;
        }

        public string Put()
        {
            WebRequest request = WebRequest.Create(requestUrl);
            request.Method = "PUT";
            Headers(request);
            WriteData(request, requestBody);
            WebResponse response = request.GetResponse();
            string responseString = ReadStream(response.GetResponseStream());
            response.Close();
            return responseString;
        }

        public string Post()
        {
            WebRequest request = WebRequest.Create(requestUrl);
            request.Method = "POST";
            Headers(request);
            WriteData(request, requestBody);
            WebResponse response = request.GetResponse();
            string responseString = ReadStream(response.GetResponseStream());
            response.Close();
            return responseString;
        }

        private string ReadStream(Stream dataStream)
        {
            StreamReader reader = new StreamReader(dataStream);
            string response = reader.ReadToEnd();
            reader.Close();
            return response;
        }

        private void WriteData(WebRequest request, string data)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
        }
    }
}