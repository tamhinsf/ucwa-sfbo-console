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

            Helpers.SharedHttpClient.DefaultRequestHeaders.Clear();
            Helpers.SharedHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            Helpers.SharedHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var createMeetingPostData = JsonConvert.SerializeObject(ucwaMyOnlineMeetingObject);
            Console.WriteLine("CreateMyOnlineMeeting POST data is " + createMeetingPostData);
            var httpResponseMessage =
                Helpers.SharedHttpClient.PostAsync(ucwaMyOnlineMeetingsUserRootUri, new StringContent(createMeetingPostData, Encoding.UTF8,
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
            Helpers.SharedHttpClient.DefaultRequestHeaders.Clear();
            Helpers.SharedHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            Helpers.SharedHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var httpResponseMessage =
                Helpers.SharedHttpClient.DeleteAsync(ucwaMyCreatedOnlineMeetingUri).Result;
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
            Helpers.SharedHttpClient.DefaultRequestHeaders.Clear();
            Helpers.SharedHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            Helpers.SharedHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var httpResponseMessage =
                Helpers.SharedHttpClient.GetAsync(ucwaMyOnlineMeetingsUserRootUri).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("ListMyOnlineMeetings response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            }
        }


    }
}
