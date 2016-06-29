# Create Skype for Business Online Apps using the UCWA API and Password Authentication

This example demonstrates how to create a Skype for Business Online (SfBO) app using the UCWA API and password authentication.   

Many of you have created Skype for Business On-Premise integrations using UCWA and password authentication.  This interactive, console-based .NET application will demonstrate how you can do the same using Skype for Business Online.  The input and result of backend calls will be displayed so you can understand the flow.  You'll be able to sign in, create an application, create/list/delete an online meeting, and make a user available with a specific presence value.

Skype for Business Online works with Azure Active Directory to perform user authentication.  We'll be using the methods described [here](http://www.cloudidentity.com/blog/2014/07/08/using-adal-net-to-authenticate-users-via-usernamepassword/) to perform direct username and password authentication

Future versions of this example may demonstrate how to authenticate with OAuth and provide a related nuget package.  Watch us for updates! 


## Setup a development environment

* Clone this GitHub repository.
* Install Visual Studio 2015.  Don't have it?  Download the free [Visual Studio Community Edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx)

## Identify a test user account

* Sign in to your Office 365 environment as an administrator at [https://portal.office.com/admin/default.aspx](https://portal.office.com/admin/default.aspx)
* Find a user whose account you'd like to use with this example
  * Ensure the user's "Product licenses" gives them access to SfBO.  
  * Disable any multi-factor authentication for this account
  * If you're resetting a user's password, make sure you un-select "Make this user change their password ..."
  * If you're creating a new user or assigning a license to an existing user, you'll need to wait an hour before they can sign in to SfBO (and thus, this example).

## Create a SfBO Application in Azure Active Directory

* Sign in to your Azure Management Portal at https://manage.windowsazure.com
* Select Active Directory -> Applications.  Click Add at the bottom of the screen to "Add an application my organization is developing."
    * Name: SfboUcwa (anything will work)
    * Redirect URI: http://localhost (anything will work)
* Once Azure has created your app, copy your Client ID and give your application access to SfBO.  
   * Click "configure" on your app's page
   * Copy the Client ID
   * Scroll to the bottom of the page: Permissions to other applications.  
     * Click Add application.  
     * From the pop-up window, select Skype for Business Online.  
     * Click the check mark to close the pop-up.
   * Skype for Business Online should now be below "Permissions to other applications".   Click the down arrow to the right of Delegated Permissions and check at least the following:
     * Create Skype Meetings
     * Initiate conversations and join meetings
     * Read/write Skype user information (preview)
   * Click Save at the bottom of the page
* Copy your Tenant name, which is typically in the form of your-domain.onmicrosoft.com.  You can also use your Tenant ID in its place.
  * While still logged into manage.windowsazure.com, Select Active Directory -> Name of the directory where you created your app.
  * Select Applications.  Click View Endpoints at the bottom of the page.
  * Your Tenant ID is the alphanumeric value separated by dashes, immediate after the hostname.  Make sure you copy the entire value: it's between the "/" characters.
  ![](https://acom.azurecomcdn.net/80C57D/cdn/mediahandler/docarticles/dpsmedia-prod/azure.microsoft.com/en-us/documentation/articles/resource-group-create-service-principal-portal/20160525050127/save-tenant.png)
    
  
## Build and Run UcwaSfboConsole

* Open the cloned code from this repository in Visual Studio
* Update Program.cs with your Client ID and Tenant name
* Optionally, you can hard code the username (i.e. username@your-domain.onmicrosoft.com) and password you want to use
* Build and run the app: UcwaSfboConsole

## Using UcwaSfboConsole
 
You'll be presented with a number of options.  Begin with "login" and enter the credentials of the user you identified earlier.  You'll see the output of the UCWA Autodiscovery flow.
 
After you login, you can type "meeting" to see the flow related to creating, listing, and deleting a meeting using UCWA's MyOnlineMeeting resource.  "Presence" will first make a call to UCWA's MakeMeAvailable resource and, if necessary, Presence to set your presence to one of the displayed values.

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

Copyright (c) 2016 Tam Huynh. All rights reserved.

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**