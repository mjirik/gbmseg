using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GBMWeb.Client.Data
{
    public abstract class BaseService
    {
        public TResult DoGet<TResult>(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            if ((int)httpWebResponse.StatusCode > 299)
            {
                return default;
            }

            using var streamReader =
                new StreamReader(httpWebResponse.GetResponseStream() ?? throw new InvalidOperationException());

            return JsonConvert.DeserializeObject<TResult>(streamReader.ReadToEnd());
        }

        public TResult DoPost<TArgs, TResult>(string url, TArgs args)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(args));
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            if ((int)httpWebResponse.StatusCode > 299)
            {
                return default;
            }

            using var streamReader =
                new StreamReader(httpWebResponse.GetResponseStream() ?? throw new InvalidOperationException());

            return JsonConvert.DeserializeObject<TResult>(streamReader.ReadToEnd());
        }

        public void DoPut<TArgs>(string url, TArgs args)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(args));
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            if ((int)httpWebResponse.StatusCode > 299)
            {
                throw new Exception($"Could not perform PUT request on {url}");
            }

            using var streamReader =
                new StreamReader(httpWebResponse.GetResponseStream() ?? throw new InvalidOperationException());
        }
    }
}
