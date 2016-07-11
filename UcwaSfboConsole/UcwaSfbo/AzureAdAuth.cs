using System;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class AzureAdAuth
    {
        public static AuthenticationResult GetAzureAdToken(AuthenticationContext authContext, String resourceHostUri,
            string clientId, string redirectUri, UserCredential uc)
        { 

            AuthenticationResult authenticationResult = null;

            Console.WriteLine("Performing GetAzureAdToken");
            try
            {
                Console.WriteLine("Passed resource host URI is " + resourceHostUri);
                if (resourceHostUri.StartsWith("http"))
                {
                    resourceHostUri = Helpers.ReduceUriToProtoAndHost(resourceHostUri);
                    Console.WriteLine("Normalized the resourceHostUri to just the protocol and hostname " + resourceHostUri);
                }

                // check if there's a user credential - i.e. a username and password

                if(uc != null)
                    {
                    authenticationResult = authContext.AcquireTokenAsync(resourceHostUri, clientId, uc).Result;

                }
                else {
                    PlatformParameters platformParams = new PlatformParameters(PromptBehavior.Auto);
                    authenticationResult = authContext.AcquireTokenAsync(resourceHostUri, clientId, new Uri(redirectUri), platformParams).Result;
                }

                //Console.WriteLine("Bearer token from Azure AD is " + authenticationResult.AccessToken);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unexpected error occurred.");
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += Environment.NewLine + "Inner Exception : " + ex.InnerException.Message;
                }
                Console.WriteLine("Message: {0}", message);
                Console.ForegroundColor = ConsoleColor.White;

            }

            return authenticationResult;
        }
    }
}
