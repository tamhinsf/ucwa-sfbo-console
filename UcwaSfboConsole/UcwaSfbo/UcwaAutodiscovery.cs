using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class UcwaAutodiscovery
    {
        private static string ucwaAutoDiscoveryUri = "https://webdir.online.lync.com/autodiscover/autodiscoverservice.svc/root";

        public static string GetUcwaRootUri(String sfboResourceAppId, UserCredential uc, String tenant, string clientId, string aadInstance)
        {
            Console.WriteLine("Now we'll call UCWA Autodiscovery to get the root/oauth/user URI");
            var ucwaAutoDiscoveryUserRootUri = DoUcwaAutoDiscovery(sfboResourceAppId, uc, tenant, clientId, aadInstance);

            Console.WriteLine("Now we'll get the UCWA Applications URI for the user");
            var ucwaRootUri = GetUcwaUserResourceUri(ucwaAutoDiscoveryUserRootUri, uc, tenant, clientId, aadInstance);

            return ucwaRootUri;
        }

        private static string DoUcwaAutoDiscovery(String sfboResourceAppId, UserCredential uc, String tenant, string clientId, string aadInstance)
        {
            AuthenticationResult authenticationResult = null;
            authenticationResult = AzureAdAuth.GetAzureAdToken(sfboResourceAppId, uc, tenant, clientId, aadInstance);

            string ucwaAutoDiscoveryUserRootUri = string.Empty;

            var httpClient = new HttpClient();
            //Console.WriteLine("Using this access token " + result.AccessToken);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            var httpResponseMessage = httpClient.GetAsync(ucwaAutoDiscoveryUri).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Called " + ucwaAutoDiscoveryUri);
                var resultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Console.WriteLine("DoUcwaDiscovery URI " + resultString);
                dynamic resultObject = JObject.Parse(resultString);
                ucwaAutoDiscoveryUserRootUri = resultObject._links.user.href;
                Console.WriteLine("DoUcwaDiscovery Root URI is " + ucwaAutoDiscoveryUserRootUri);
            }
            return ucwaAutoDiscoveryUserRootUri;
        }


        private static string GetUcwaUserResourceUri(String ucwaUserDiscoveryUri, UserCredential uc, String tenant, string clientId, string aadInstance)
        {
            AuthenticationResult authenticationResult = null;
            authenticationResult = AzureAdAuth.GetAzureAdToken(ucwaUserDiscoveryUri, uc, tenant, clientId, aadInstance);

            string ucwaUserResourceUri = String.Empty;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            var httpResponseMessage = httpClient.GetAsync(ucwaUserDiscoveryUri).Result;

            Console.WriteLine("Called " + ucwaUserDiscoveryUri);
            var resultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine("GetUcwaUserResourceUri Body " + resultString);
            dynamic resultObject = JObject.Parse(resultString);
            string redirectUri = "";
            try
            {
                redirectUri = resultObject._links.redirect.href;
            }
            catch
            {
                Console.WriteLine("No re-direct");
            }
            if (redirectUri != "")
            {
                Console.WriteLine("GetUcwaUserResourceUri redirect is " + redirectUri);
                redirectUri += "/oauth/user";  // for some reason, the redirectUri doesn't include /oauth/user
                Console.WriteLine("Modifying GetUcwaUserResourceUri to be correct " + redirectUri);
                // recursion is your friend
                ucwaUserResourceUri = GetUcwaUserResourceUri(redirectUri,uc, tenant, clientId, aadInstance);
            }
            else  // if there's no redirect then the applications URI is there for us to grab
            {
                ucwaUserResourceUri = resultObject._links.applications.href;
            }
            Console.WriteLine("GetUcwaUserResourceUri is " + ucwaUserResourceUri);
            return ucwaUserResourceUri;
        }
    }
}
