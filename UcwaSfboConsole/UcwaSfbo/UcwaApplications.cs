using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class UcwaApplications
    {
        public class UcwaMyAppsObject
        {
            public string UserAgent { get; set; }
            public string EndpointId { get; set; }
            public string Culture { get; set; }
        }

        public static string CreateUcwaApps(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, string ucwaApplicationsRootUri,
            UcwaMyAppsObject ucwaAppsObject)
        {
            string createUcwaAppsResults = string.Empty;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var createUcwaPostData = JsonConvert.SerializeObject(ucwaAppsObject);
            Console.WriteLine("CreateUcwaApps POST data is " + createUcwaPostData);
            var httpResponseMessage =
                httpClient.PostAsync(ucwaApplicationsRootUri, new StringContent(createUcwaPostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("CreateUcwaApps response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                createUcwaAppsResults = httpResponseMessage.Content.ReadAsStringAsync().Result;
            }

            return createUcwaAppsResults;
        }

    }
}
