using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class UcwaPresence
    {
        // Presence options for UcwaPresenceOptions https://msdn.microsoft.com/en-us/library/office/dn323684.aspx

        public static List<string> UcwaPresenceOptions = new List<string> { "Away", "BeRightBack", "Busy", "DoNotDisturb", "Offwork", "Online" };

        public class UcwaPresenceObject
        {
            public string availability { get; set; } 
        }

        public static String GetPresenceUri(String createUcwaAppsResults, String ucwaApplicationHostRootUri)
        {
            dynamic createUcwaAppsResultsObject = JObject.Parse(createUcwaAppsResults);
            string getPresenceUri = String.Empty;

            try
            {
                getPresenceUri = ucwaApplicationHostRootUri +
                   createUcwaAppsResultsObject._embedded.me._links.presence.href;
            }
            catch
            {

            }
            Console.WriteLine("getPresenceUri is " + getPresenceUri);
            return getPresenceUri;
        }

        public static void SetPresence(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, String getPresenceUri,
            UcwaPresenceObject ucwaPresenceObject)
        {
            string setPresenceResults = string.Empty;
            Console.WriteLine("URI is " + getPresenceUri);

            //            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var setPresencePostData = JsonConvert.SerializeObject(ucwaPresenceObject);
            Console.WriteLine("SetPresence POST data is " + setPresencePostData);
            var httpResponseMessage =
                httpClient.PostAsync(getPresenceUri, new StringContent(setPresencePostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("SetPresence response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            Console.WriteLine("SetPresence response should be empty");



        }
    }
}
