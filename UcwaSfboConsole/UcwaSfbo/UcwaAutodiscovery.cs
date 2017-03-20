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

        public static string GetUcwaRootUri(AuthenticationContext authenticationContext, String sfboResourceAppId, 
                string clientId, string redirectUri, UserCredential uc)
        {
            Console.WriteLine("Now we'll call UCWA Autodiscovery to get the root/oauth/user URI");
            var ucwaAutoDiscoveryUserRootUri = DoUcwaAutoDiscovery(Helpers.SharedHttpClient, authenticationContext, sfboResourceAppId, clientId, redirectUri, uc);

            Console.WriteLine("Now we'll get the UCWA Applications URI for the user");
            var ucwaRootUri = GetUcwaUserResourceUri(Helpers.SharedHttpClient, authenticationContext, ucwaAutoDiscoveryUserRootUri, clientId, redirectUri, uc);

            return ucwaRootUri;
        }

        private static string DoUcwaAutoDiscovery(HttpClient httpClient, AuthenticationContext authenticationContext, String sfboResourceAppId, string clientId, string redirectUri, UserCredential uc)
        {
            AuthenticationResult authenticationResult = null;
            authenticationResult = AzureAdAuth.GetAzureAdToken(authenticationContext, sfboResourceAppId, clientId, redirectUri, uc);

            string ucwaAutoDiscoveryUserRootUri = string.Empty;

            //Console.WriteLine("Using this access token " + result.AccessToken);
            httpClient.DefaultRequestHeaders.Clear();
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

        private static string GetUcwaUserResourceUri(HttpClient httpClient, AuthenticationContext authenticationContext, String ucwaUserDiscoveryUri, string clientId, 
            string redirectUri, UserCredential uc)
        {
            AuthenticationResult authenticationResult = null;
            authenticationResult = AzureAdAuth.GetAzureAdToken(authenticationContext, ucwaUserDiscoveryUri, clientId, redirectUri, uc);

            string ucwaUserResourceUri = String.Empty;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            var httpResponseMessage = httpClient.GetAsync(ucwaUserDiscoveryUri).Result;

            Console.WriteLine("Called " + ucwaUserDiscoveryUri);
            var resultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine("GetUcwaUserResourceUri Body " + resultString);
            dynamic resultObject = JObject.Parse(resultString);
            string resourceRedirectUri = "";
            try
            {
                resourceRedirectUri = resultObject._links.redirect.href;
            }
            catch
            {
                Console.WriteLine("No re-direct");
            }
            if (resourceRedirectUri != "")
            {
                Console.WriteLine("GetUcwaUserResourceUri redirect is " + resourceRedirectUri);
                resourceRedirectUri += "/oauth/user";  // for some reason, the redirectUri doesn't include /oauth/user
                Console.WriteLine("Modifying GetUcwaUserResourceUri to be correct " + resourceRedirectUri);
                // recursion is your friend
                ucwaUserResourceUri = GetUcwaUserResourceUri(httpClient, authenticationContext, resourceRedirectUri, clientId, redirectUri, uc);
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
