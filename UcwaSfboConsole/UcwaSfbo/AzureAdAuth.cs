using System;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UcwaSfboConsole.UcwaSfbo
{
    public class AzureAdAuth
    {

        public static AuthenticationResult GetAzureAdToken(String resourceHostUri, UserCredential uc, String tenant, string clientId, string aadInstance)
        {
            // Initialize the Authority and AuthenticationContext for the AAD tenant of choice.

            var authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        
            var authContext = new AuthenticationContext(authority);

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

                authenticationResult = authContext.AcquireTokenAsync(resourceHostUri, clientId, uc).Result;
                //Console.WriteLine("Bearer token from Azure AD is " + authenticationResult.AccessToken);
            }
            catch(Exception ex)
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
