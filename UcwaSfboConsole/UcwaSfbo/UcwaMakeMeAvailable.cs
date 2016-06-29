using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace UcwaSfboConsole.UcwaSfbo
{
    public class UcwaMakeMeAvailable
    {
        public class UcwaMakeMeAvailableObject
        {
            public string signInAs { get; set; }
            public string phoneNumber { get; set; }
        }

        public static String GetMakeMeAvailableUri(String createUcwaAppsResults, String ucwaApplicationHostRootUri)
        {
            dynamic createUcwaAppsResultsObject = JObject.Parse(createUcwaAppsResults);
            string getMakeMeAvailableUri = String.Empty;

            try
            {
                getMakeMeAvailableUri = ucwaApplicationHostRootUri +
                   createUcwaAppsResultsObject._embedded.me._links.makeMeAvailable.href;
            }
            catch
            {
                
            }
            Console.WriteLine("getMakeMeAvailableUri is " + getMakeMeAvailableUri);
            return getMakeMeAvailableUri;
        }

        public static bool MakeMeAvailable(AuthenticationResult ucwaAuthenticationResult, String ucwaMakeMeAvailableRootUri,
            UcwaMakeMeAvailableObject ucwaMyPresenceObject)
        {
            string makeMeAvailableResults = string.Empty;
            Console.WriteLine("URI is " + ucwaMakeMeAvailableRootUri);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var makeMeAvailablePostData = JsonConvert.SerializeObject(ucwaMyPresenceObject);
            Console.WriteLine("MakeMeAvailable POST data is " + makeMeAvailablePostData);
            var httpResponseMessage =
                httpClient.PostAsync(ucwaMakeMeAvailableRootUri, new StringContent(makeMeAvailablePostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("MakeMeAvailable response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            Console.WriteLine("MakeMeAvailable response should be empty");

            if (httpResponseMessage.Content.ReadAsStringAsync().Result == String.Empty)
            {
                Console.WriteLine("MakeMeAvailable call succeeded");
                return true;
            }

            Console.WriteLine("MakeMeAvailable call failed");
            return false;
        }
    }
}
