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
    public class UcwaMyGroupMemberships
    {
        private static string GetMyGroupMembershipsResource(AuthenticationResult ucwaAuthenticationResult, 
            String ucwaMyGroupMembershipsRootUri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var httpResponseMessage =
                httpClient.GetAsync(ucwaMyGroupMembershipsRootUri).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("GetMyGroupMembershipsResource response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                return httpResponseMessage.Content.ReadAsStringAsync().Result;
            }

            return null;
        }

        private static string GetMyGroupMembershipsRootUri(String ucwaMyGroupMembershipsResource, String ucwaApplicationsHost)
        {
            dynamic myGroupMembershipResultsObject = JObject.Parse(ucwaMyGroupMembershipsResource);
            var ucwaMyGroupMembershipsRootUri = ucwaApplicationsHost +
                  myGroupMembershipResultsObject._embedded.people._links.myGroupMemberships.href;
            Console.WriteLine("ucwaMyGroupMembershipsRootUri is " + ucwaMyGroupMembershipsRootUri);
            return ucwaMyGroupMembershipsRootUri;
        }

        public static void ListMyGroupMemberships(AuthenticationResult ucwaAuthenticationResult,
            String createUcwaAppsResults, String ucwaApplicationsHost) 
        {
            var ucwaMyGroupMembershipsRootUri = GetMyGroupMembershipsRootUri(createUcwaAppsResults, ucwaApplicationsHost);
            var ucwaMyGroupMembershipsResource = GetMyGroupMembershipsResource(ucwaAuthenticationResult, ucwaMyGroupMembershipsRootUri);
        }

        public static bool AddMyGroupMemberships(AuthenticationResult ucwaAuthenticationResult,
            String createUcwaAppsResults, String ucwaApplicationsHost, String ucwaContactUri)
        {
            var ucwaMyGroupMembershipsRootUri = GetMyGroupMembershipsRootUri(createUcwaAppsResults, ucwaApplicationsHost);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-MS-RequiresMinResourceVersion", "2");
            var httpResponseMessage =
                httpClient.PostAsync(ucwaMyGroupMembershipsRootUri + "?contactUri=sip:" + ucwaContactUri, null).Result;

            Console.WriteLine("AddMyGroupMemberships POST must pass X-MS-RequiresMinResourceVersion 2 in the request headers");
            Console.WriteLine("This is because it is a UCWA Revision / Version 2.0 request");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("AddMyGroupMemberships POST response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                Console.WriteLine("AddMyGroupMemberships response should be empty");
                return true;
            }
            return false;

        }

        public static bool DeleteMyGroupMemberships(AuthenticationResult ucwaAuthenticationResult,
            String createUcwaAppsResults, String ucwaApplicationsHost, String ucwaContactUri)
        {
            var ucwaMyGroupMembershipsRootUri = GetMyGroupMembershipsRootUri(createUcwaAppsResults, ucwaApplicationsHost);
            var ucwaMyGroupMembershipsResource = GetMyGroupMembershipsResource(ucwaAuthenticationResult, ucwaMyGroupMembershipsRootUri);

            dynamic myGroupMembershipResultsObject = JObject.Parse(ucwaMyGroupMembershipsResource);
            var ucwaDeleteMyGroupMembershipsRootUri = ucwaApplicationsHost +
                  myGroupMembershipResultsObject._links.removeContactFromAllGroups.href;
            Console.WriteLine("ucwaDeleteMyGroupMembershipsRootUri is " + ucwaDeleteMyGroupMembershipsRootUri);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-MS-RequiresMinResourceVersion", "2");
            var httpResponseMessage =
                httpClient.PostAsync(ucwaDeleteMyGroupMembershipsRootUri + "?contactUri=sip:" + ucwaContactUri, null).Result;

            Console.WriteLine("DeleteMyGroupMemberships POST must pass X-MS-RequiresMinResourceVersion 2 in the request headers");
            Console.WriteLine("This is because it is a UCWA Revision / Version 2.0 request");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("DeleteMyGroupMemberships POST response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                Console.WriteLine("DeleteMyGroupMemberships response should be empty");
                return true;
            }
            return false;

        }

    }
}
