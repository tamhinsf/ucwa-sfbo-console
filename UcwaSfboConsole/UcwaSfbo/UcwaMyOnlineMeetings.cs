using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class UcwaMyOnlineMeetings
    {
        public class UcwaMyOnlineMeetingObject
        {
            public string subject { get; set; }
            public string description { get; set; }
            public List<string> attendees { get; set; }
        }

        public static string GetMyOnlineMeetingUri(String createUcwaAppsResults, String ucwaApplicationHostRootUri)
        {
            dynamic createUcwaAppsResultsObject = JObject.Parse(createUcwaAppsResults);
            var ucwaMyOnlineMeetingsUserRootUri = ucwaApplicationHostRootUri +
                createUcwaAppsResultsObject._embedded.onlineMeetings._links.myOnlineMeetings.href;
            Console.WriteLine("ucwaMyOnlineMeetingsUserRootUri is " + ucwaMyOnlineMeetingsUserRootUri);
            return ucwaMyOnlineMeetingsUserRootUri;
        }

        public static string CreateMyOnlineMeeting(AuthenticationResult ucwaAuthenticationResult, String ucwaMyOnlineMeetingsUserRootUri,
            String ucwaApplicationHostRootUri, UcwaMyOnlineMeetingObject ucwaMyOnlineMeetingObject)
        {
            string ucwaMyCreatedOnlineMeetingUri = String.Empty;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var createMeetingPostData = JsonConvert.SerializeObject(ucwaMyOnlineMeetingObject);
            Console.WriteLine("CreateMyOnlineMeeting POST data is " + createMeetingPostData);
            var httpResponseMessage =
                httpClient.PostAsync(ucwaMyOnlineMeetingsUserRootUri, new StringContent(createMeetingPostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("CreateMyOnlineMeeting response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var resultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                dynamic resultObject = JObject.Parse(resultString);
                ucwaMyCreatedOnlineMeetingUri = ucwaApplicationHostRootUri + resultObject._links.self.href;
                Console.WriteLine("CreateMyOnlineMeeting URI is " + ucwaMyCreatedOnlineMeetingUri);
            }

            return ucwaMyCreatedOnlineMeetingUri;

        }

        public static bool DeleteMyOnlineMeeting(AuthenticationResult ucwaAuthenticationResult, string ucwaMyCreatedOnlineMeetingUri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var httpResponseMessage =
                httpClient.DeleteAsync(ucwaMyCreatedOnlineMeetingUri).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("DeleteMyOnlineMeeting response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                Console.WriteLine("DeleteMyOnlineMeeting response should be empty");
                return true;
            }
            return false;
        }

        public static void ListMyOnlineMeetings(AuthenticationResult ucwaAuthenticationResult, String ucwaMyOnlineMeetingsUserRootUri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var httpResponseMessage =
                httpClient.GetAsync(ucwaMyOnlineMeetingsUserRootUri).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("ListMyOnlineMeetings response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            }
        }


    }
}
