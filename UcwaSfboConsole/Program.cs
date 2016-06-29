using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using UcwaSfboConsole.UcwaSfbo;
using System.Linq;

namespace UcwaSfboConsole
{
    class Program
    {
        #region Init

        // replace tenant with the name of your Azure AD instance
        // this is usually in the form of your-tenant.onmicrosoft.com

        private static string tenant = "";

        // replace clientID with the clientID of the SFBO native app you created
        // in your Azure AD instance.  

        // make sure you grant at least these three permissions to your app
        // Create Skype Meetings
        // Initiate conversations and join meetings
        // Read/write Skype user information (preview)

        private static string clientId = "";

        // sfboResourceAppId is a constant you don't have to change

        private static string sfboResourceAppId = "00000004-0000-0ff1-ce00-000000000000";

        private static string aadInstance = "https://login.microsoftonline.com/{0}";

        // feeling lazy?  hard code your username and password in the variables below 
        // if values are present, the "login" command will automatically use them and not
        // prompt you for credentials

        private static string hardcodedUsername = "";
        private static string hardcodedPassword = "";

        // we've set up some global variables you'll use across UCWA API calls
        // to access user resources
        // ucwaApplicationsUri
        // ucwaApplicationsHost
        // createUcwaAppsResults

        // ucwaApplicationsUri - stores the UCWA applications resource uri
        // as returned by the autodiscovery process
        // i.e. https://webpoolXY.infra.lync.com/ucwa/oauth/v1/applications

        private static string ucwaApplicationsUri = "";

        // ucwaApplicationsHost - stores the UCWA application resource
        // protocol and hostname, as derived from ucwaApplicationsUri
        // i.e. https://webpoolXY.infra.lync.com/
        // you will combine ucwaApplicationsHost with links to individual
        // UCWA application resources as stored in the createUcwaAppsResults
        // string as described next

        private static string ucwaApplicationsHost = "";

        // createUcwaAppsResults - stores the result of making a POST call to
        // ucwaApplicationsUri.  This is a JSON string that contains the link to
        // UCWA application resources, such as:
        // me, people, onlineMeetings, and communciation

        private static string createUcwaAppsResults = "";

        // ucwaAuthenticationResult - stores the result of the Azure AD auth call
        // against ucwaApplicationsHost.  It's used in API calls against
        // UCWA app resources

        private static AuthenticationResult ucwaAuthenticationResult = null;
       
        #endregion

        static void Main(string[] args)
        {

            string commandString = string.Empty;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("**************************************************");
            Console.WriteLine("*  Azure AD + Skype for Business Online via UCWA *");
            Console.WriteLine("**************************************************");
            Console.WriteLine("");
            Console.WriteLine("**************************************************");
            Console.WriteLine(" tentant is " + tenant);
            Console.WriteLine(" clientId is " + clientId);
            Console.WriteLine("**************************************************");

            if (tenant == "" && clientId == "")
            {
                Console.WriteLine("You need to provide your Azure AD tenant name");
                Console.WriteLine("and application clientId in Program.cs before");
                Console.WriteLine("you can run this app");
                return;
            }

            Help();

            // main command cycle

            while (!commandString.Equals("Exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Enter command (login | meeting | presence | help | exit ) >");
                commandString = Console.ReadLine();

                switch (commandString.ToUpper())
                {
                    case "LOGIN":
                        Login();
                        break;
                    case "MEETING":
                        Meeting();
                        break;
                    case "PRESENCE":
                        Presence();
                        break;
                    case "HELP":
                        Help();
                        break;
                    case "EXIT":
                        Console.WriteLine("Bye!"); ;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }
        }

        #region Textual UX

        // Gather user credentials form the command line 

        static UserCredential GetUserCredentials()
        {

            if (hardcodedUsername == String.Empty && hardcodedPassword == String.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please enter username and password to sign in.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("User>");
                string user = Console.ReadLine();
                Console.WriteLine("Password>");
                string password = ReadPasswordFromConsole();
                Console.WriteLine("");
                return new UserPasswordCredential(user, password);
            }
            return new UserPasswordCredential(hardcodedUsername, hardcodedPassword);

        }

        // Obscure the password being entered
        static string ReadPasswordFromConsole()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return password;
        }
        #endregion

        #region Commands

        static void Help()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("LOGIN  - sign in to your Azure AD + SFB Online App using the UCWA API");
            Console.WriteLine("MEETING  - create, list all, and delete an online meeting");
            Console.WriteLine("PRESENCE  - change your presence to be away");
            Console.WriteLine("HELP  - displays this page");
            Console.WriteLine("EXIT  - closes this program");
            Console.WriteLine("");
        }

        // Login to the user's account and create a UCWA app

        static void Login()
        {
            #region Login

            Console.WriteLine("Let's gather username and password credentials from the user");
            var uc = GetUserCredentials();

            Console.WriteLine("Let's validate the user's credentials before beginning UCWA AutoDiscovery");
            AuthenticationResult testCredentials = null;
            testCredentials = UcwaSfbo.AzureAdAuth.GetAzureAdToken(sfboResourceAppId, uc, tenant, clientId, aadInstance);

            if(testCredentials == null)
            {
                Console.WriteLine("We encountered an Azure AD error");
                Console.WriteLine("Check your tenant, clientID, and credentials");
                return;
            }

            ucwaApplicationsUri = UcwaAutodiscovery.GetUcwaRootUri(sfboResourceAppId, uc, tenant, clientId, aadInstance);

            Console.WriteLine("We'll store the base UCWA app URI for use with UCWA app calls");
            Console.WriteLine("We prefix this to the links returned from the UCWA apps POST");
            Console.WriteLine("Since these links aren't full URIs");
            ucwaApplicationsHost = Helpers.ReduceUriToProtoAndHost(ucwaApplicationsUri);
            Console.WriteLine("ucwaApplicationsHost is " + ucwaApplicationsHost);

            Console.WriteLine("Get a token to access the user's UCWA Applications Resources from Azure AD.");
            Console.WriteLine("We can re-use this token for each UCWA app call");
            ucwaAuthenticationResult = AzureAdAuth.GetAzureAdToken(ucwaApplicationsHost, uc, tenant, clientId, aadInstance);

            Console.WriteLine("Now we'll create and/or query UCWA Apps via POST");
            Console.WriteLine("Well create a UCWA apps object to pass to CreateUcwaApps");

            UcwaApplications.UcwaMyAppsObject ucwaMyAppsObject = new UcwaApplications.UcwaMyAppsObject()
            {
                UserAgent = "myAgent",
                EndpointId = "1234",
                Culture = "en-US"
            };

            Console.WriteLine("Making request to ucwaApplicationsUri " + ucwaApplicationsUri);
            createUcwaAppsResults = UcwaApplications.CreateUcwaApps(ucwaAuthenticationResult, ucwaApplicationsUri, ucwaMyAppsObject);

            return;
        }

        // create a meeting, list all meetings, delete the created meeting, and list all meetings again

        static void Meeting()
        {
            if (ucwaAuthenticationResult == null)
            {
                Console.WriteLine("You haven't logged in yet!");
                return;
            }
            Console.WriteLine("createUcwaAppsResults is a JSON string containing links to all resources");
            Console.WriteLine("We will parse to find GetMyOnlineMeetingUri");
            var ucwaMyOnlineMeetingsUserRootUri = UcwaMyOnlineMeetings.GetMyOnlineMeetingUri(createUcwaAppsResults, ucwaApplicationsHost);

            Console.WriteLine("Now we'll create an online meeting, list all meetings, delete the meeting we created, and list again");
            Console.WriteLine("Now we'll create an online meeting object to pass to CreateMyOnlineMeeting");

            UcwaMyOnlineMeetings.UcwaMyOnlineMeetingObject ucwaMyOnlineMeetingObject = new UcwaMyOnlineMeetings.UcwaMyOnlineMeetingObject()
            {
                subject = "my subject",
                description = "my description",
                attendees = new List<string>()
                {
                    "sip:joe@one.com",
                    "sip:jane@another.com"
                }

            };

            Console.WriteLine("Now we'll create an online meeting");
            var ucwaMyCreatedOnlineMeetingUri = UcwaMyOnlineMeetings.CreateMyOnlineMeeting(ucwaAuthenticationResult, ucwaMyOnlineMeetingsUserRootUri,
                ucwaApplicationsHost, ucwaMyOnlineMeetingObject);

            Console.WriteLine("Now we'll list all meetings");
            UcwaMyOnlineMeetings.ListMyOnlineMeetings(ucwaAuthenticationResult, ucwaMyOnlineMeetingsUserRootUri);

            Console.WriteLine("Now we'll delete the meeting we created " + ucwaMyCreatedOnlineMeetingUri);
            UcwaMyOnlineMeetings.DeleteMyOnlineMeeting(ucwaAuthenticationResult, ucwaMyCreatedOnlineMeetingUri);

            Console.WriteLine("Now we'll list all meetings again");
            UcwaMyOnlineMeetings.ListMyOnlineMeetings(ucwaAuthenticationResult, ucwaMyOnlineMeetingsUserRootUri);

            Console.WriteLine("Is this meeting there in the JSON response above? " + ucwaMyCreatedOnlineMeetingUri);

        }

        // try to set the user's presence
        // first, try to make the user available if they're not
        // then, set their presence

        static void Presence()
        {
            if (ucwaAuthenticationResult == null)
            {
                Console.WriteLine("You haven't logged in yet!");
                return;
            }

            Console.WriteLine("Please enter which presence value you want");
            Console.WriteLine("Away BeRightBack Busy DoNotDisturb Offwork Online");
            Console.WriteLine("Presence>");
            string userPresence = Console.ReadLine();
            if (!UcwaPresence.UcwaPresenceOptions.Contains(userPresence,StringComparer.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("You didn't pick an option from the list");
                Console.WriteLine("Enter presence if you want to try again");
                return;
            }

            Console.WriteLine("MakeMeAvailable has to be called before you can set a user's presence");
            Console.WriteLine("UCWA lets you set a user's presence when calling MakeMeAvailable, so we'll try it");
            Console.WriteLine("We'll create a UcwaMakeMeAvailableObject that sets the user's presence as " + userPresence);

            UcwaMakeMeAvailable.UcwaMakeMeAvailableObject ucwaMakeMeAvailableObject = new UcwaMakeMeAvailable.UcwaMakeMeAvailableObject
            {
                signInAs = userPresence,
                phoneNumber = "212 867 5309"
            };

            Console.WriteLine("createUcwaAppsResults is a JSON string containing links to all resources");
            Console.WriteLine("We will parse to find MakeMeAvailable");

            var ucwaMakeMeAvailableRootUri = UcwaMakeMeAvailable.GetMakeMeAvailableUri(createUcwaAppsResults, ucwaApplicationsHost);
            if (ucwaMakeMeAvailableRootUri != String.Empty)
            {

                if (UcwaMakeMeAvailable.MakeMeAvailable(ucwaAuthenticationResult, ucwaMakeMeAvailableRootUri, ucwaMakeMeAvailableObject))
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Looks like we encountered an error.  Wait before trying this again");
                    return;
                }
            }

            Console.WriteLine("Whoops! MakeMeAvailable isn't in createUcwaAppsResults");
            Console.WriteLine("The user is already available, let's simply change their presence to away");

            Console.WriteLine("createUcwaAppsResults is a JSON string containing links to all resources");
            Console.WriteLine("We will parse to find presence");
            var ucwaPresenceRootUri = UcwaPresence.GetPresenceUri(createUcwaAppsResults, ucwaApplicationsHost);

            Console.WriteLine("We'll create a UcwaPresenceObject that sets the user's presence to be the same as we intended in UcwaMakeMeAvailableObject"); 

            UcwaPresence.UcwaPresenceObject ucwaPresenceObject = new UcwaPresence.UcwaPresenceObject
            {
                availability = ucwaMakeMeAvailableObject.signInAs 
            };
            UcwaPresence.SetPresence(ucwaAuthenticationResult, ucwaPresenceRootUri, ucwaPresenceObject);
            

            #endregion

        }

        #endregion
    }



}
