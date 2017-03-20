# Create Skype for Business Online Apps and Meetings using the UCWA API

This example demonstrates how to create Skype for Business Online (SfBO) apps and meetings using UCWA - Microsoft’s Unified Communications Web API

  * Using the Skype for Business Online AppSDK? We'll show you how to create Meeting URLs that you can provide to the iOS and Android AppSDK

Many of you have created Skype for Business On-Premise integrations using UCWA.  This interactive, console-based .NET application will demonstrate how you can do the same using Skype for Business Online.  You'll also see how you can accept user credentials using a platform-specific, pre-built dialog box.  Deploying to device without a web browser?  No problem!  You can show a message that tells users to visit a website on another device where they can enter a unique code to begin sign in.  Direct username and password entry to a console is demonstrated as well.

Skype for Business Online works with Azure Active Directory to perform user authentication.  We'll be using the methods described [here](http://www.cloudidentity.com/blog/2014/07/08/using-adal-net-to-authenticate-users-via-usernamepassword/) to perform direct username and password authentication using the [ADAL library](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory), which also provides access to dialog box based-authentication as well as [device code](www.cloudidentity.com/blog/2015/12/02/new-adal-3-x-previewdevice-profile-linux-and-os-x-sample/) initiated sign-in.

#### Breaking Change 3/20/2017

An instance of System.Net.Http.HttpClient no longer needs to be passed to the methods in UcwaSfbo (i.e. UcwaApplications.CreateUcwaApps). Instead, there is a new shared instance (Helpers.SharedHttpClient) residing in Utils.cs as that they all now reference.  

## UCWA Autodiscovery Demistified

The input and result of Skype for Business Online UCWA and Azure AD backend calls will be displayed so you can understand the flow.  You'll be able to sign in, create an application, create/list/delete an online meeting, list all contacts, add and delete a contact, and make a user available with a specific presence value.

Future versions of this example may provide a related nuget package.  Watch us for updates!   In the meantime, use our example to begin migrating your Skype for Business On-Premise apps to Skype for Business Online.

## Setup a development environment

* Clone this GitHub repository.
* Install Visual Studio 2015.  Don't have it?  Download the free [Visual Studio Community Edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx)

## Identify a test user account

* Sign in to your Office 365 environment as an administrator at [https://portal.office.com/admin/default.aspx](https://portal.office.com/admin/default.aspx)
* Find a user whose account you'd like to use with this example
  * Ensure the user's "Product licenses" gives them access to SfBO.  
  * Disable any multi-factor authentication for this account if you're using direct username and password authentication.
  * If you're resetting a user's password, make sure you un-select "Make this user change their password ..."
  * If you're creating a new user or assigning a license to an existing user, you'll need to wait an hour before they can sign in to SfBO (and thus, this example).

## Create a SfBO Application in Azure Active Directory

* Sign in to your Azure Management Portal at https://manage.windowsazure.com
* Select Active Directory -> Applications.  Click Add at the bottom of the screen to "Add an application my organization is developing."
    * Name: SfboUcwa (anything will work)
    * Type: NATIVE CLIENT APPLICATION
    * Redirect URI: http://demo-sfbo-ucwa (anything will work)
* Once Azure has created your app, copy your Client ID and give your application access to SfBO.  
   * Click "configure" on your app's page
   * Copy the Client ID
   * Scroll to the bottom of the page: Permissions to other applications.  
     * Click Add application.  
     * From the pop-up window, select Skype for Business Online.  
     * Click the check mark to close the pop-up.
   * Skype for Business Online should now be below "Permissions to other applications".   Click the down arrow to the right of Delegated Permissions and check at least the following:
     * Initiate conversations and join meetings
     * Create Skype Meetings
     * Read/write Skype user information (preview)
     * Receive conversation invites (preview)
     * Read/write Skype user contacts and groups
   * Click Save at the bottom of the page
* Copy your Tenant name, which is typically in the form of your-domain.onmicrosoft.com.  You can also use your Tenant ID in its place.  How do you find your Tenant ID?
  * While still logged into manage.windowsazure.com, Select Active Directory -> Name of the directory where you created your app.
  * Select Applications.  Click View Endpoints at the bottom of the page.
  * Your Tenant ID is the alphanumeric value separated by dashes, immediate after the hostname.  Make sure you copy the entire value: it's between the "/" characters.
 
 
  ![Tenant ID Example](https://raw.githubusercontent.com/OfficeDev/TrainingContent/master/O3653/O3653-8%20Deep%20Dive%20into%20the%20Office%20365%20Unified%20API/Images/Figure04.png)
   
   
* Applications built using the UCWA API for Skype for Business Online require tenant consent.  Perform the steps here as an admin to enable users to sign in to the application we're creating:
   
   https://msdn.microsoft.com/en-us/skype/websdk/docs/developwebsdkappsforsfbonline#tenant-administrator-consent-flow
  
## Build and Run UcwaSfboConsole

* Open the cloned code from this repository in Visual Studio
* Update Program.cs in the UcwaSfboConsole folder with your Tenant name (tenant) and Client ID (clientId) 
  * Or, leave these values empty and provide them at the command line like so: 
    * UcwaSfboConsole mytenant.onmicrosoft.com my-alphanumeric-client-id
* Optionally, you can hard code the username (i.e. username@your-domain.onmicrosoft.com) and password you want to use in the variables called hardcodedUsername and hardcodedPassword
* Optionally, if you want to try dialog box authentication, update the redirectUri variable to be the value of Redirect URI you provided to Azure AD (i.e. http://demo-sfo-ucwa)
  * Or, you can provide the Redirect URI as a third command line parameter, like so:
    * UcwaSfboConsole mytenant.onmicrosoft.com my-alphanumeric-client-id http://demo-sfo-ucwa
* Build and run the app: UcwaSfboConsole

## Using UcwaSfboConsole
 
You'll be presented with a number of options.  Begin with "login".  Choose if you want to enter credentials of the user you identified earlier in "console" or "dialog" mode.  Enter "code" if you want to tell the user to visit a web page and type in the code shown before providing their credentials.

After you successfully login, you'll see the output of the UCWA Autodiscovery flow.
 
Then, you can type "meeting" to see the flow related to creating, listing, and deleting a meeting using UCWA's MyOnlineMeeting resource.  "Presence" will first make a call to UCWA's MakeMeAvailable resource and, if necessary, Presence to set your presence to one of the displayed values.  "Contact" will list your contacts, and give you the option to add or delete a contact.

## Questions and comments

We'd love to get your feedback about this sample. You can send your questions and suggestions to us in the Issues section of this repository.

Questions about Skype for Business development in general should be posted to [Stack Overflow](http://stackoverflow.com/questions/tagged/skype-for-business). Make sure that your questions or comments are tagged with [skype-for-business].

## Additional resources

* [Developing UCWA applications for Skype for Business Online](https://msdn.microsoft.com/en-us/skype/ucwa/developingucwaapplicationsforsfbonline)
* [Using ADAL .NET to Authenticate Users via Username/Password](http://www.cloudidentity.com/blog/2014/07/08/using-adal-net-to-authenticate-users-via-usernamepassword/)
* [How to get an Azure Active Directory tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/)
* [Skype Developer Platform](https://dev.office.com/skype)
* [Geting Started - Skype for Business Online](https://dev.office.com/skype/getting-started)

## Copyright

Copyright (c) 2017 Tam Huynh. All rights reserved. 


### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
