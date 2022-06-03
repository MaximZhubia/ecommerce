using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Client.Helpers
{
    static class HttpRequests
    {
        public static IRestResponse Post(string baseUrl, string resource, string body, out string content, string authToken = null)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest(resource);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(body);

            if (!string.IsNullOrEmpty(authToken))
            {
                request.AddHeader("Authorization", "Bearer " + authToken);
            }

            IRestResponse response = client.Post(request);
            content = response.Content;

            return response;
        }

        public static IRestResponse Get(string baseUrl, string resource, out string content, string authToken = null)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest(resource);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(authToken))
            {
                request.AddHeader("Authorization", "Bearer " + authToken);
            }

            IRestResponse response = client.Get(request);
            content = response.Content;

            return response;
        }
    }
}
