[![Build Status](https://dev.azure.com/MicrosoftXrmDevOps/Microsoft.Xrm.DevOps.Data/_apis/build/status/abvogel.Microsoft.Xrm.DevOps.Solutions?branchName=master)](https://dev.azure.com/MicrosoftXrmDevOps/Microsoft.Xrm.DevOps.Data/_build/latest?definitionId=3&branchName=master)

# Microsoft.Xrm.DevOps.Solutions
This library provides PowerShell commands to improve the development and release experience for Dynamics CRM with an eventual goal of making source code a first class citizen.

# Microsoft.Xrm.DevOps.Solutions.PowerShell
This wrapper uses the Microsoft.Xrm.DevOps.Solutions library providing a simple PowerShell interface.

## Installing
The PowerShell module has been posted to PowerShell Gallery. Install using their standard commands - 
https://www.powershellgallery.com/packages/Microsoft.Xrm.DevOps.Solutions.PowerShell/

    Install-Module -Name Microsoft.Xrm.DevOps.Solutions.PowerShell

## Get-CrmEarlyBoundCode
### Description
  Returns an array of text that can be passed into a .cs file and compiled for early bound classes. This uses the spkl wrapper around crmsvcutil.exe and improves delivery by simplifying how to run it (it's now a simple PowerShell command) and extending how you authenticate (connection string instead of config file).
  * Run when necessary to generate a new EarlyBoundCode file. Allow PowerShell to keep libraries up to date. No recall required of how you generated the file the last time you needed it.
  * Run in a release pipeline to always have the latest EarlyBoundCode representing your environment. This is critical for automated tests to actually be testing your CRM environment.
  * Add to a build or debug command in Visual Studio to always have the latest EarlyBoundCode in your project.
  
### Parameters
* String ConnectionString
  * Allows any valid connection string. This grants the greatest flexibility in connection methods, including oauth 2.0 and client/secrets.
* String[] Entities
	* Include the logical name of any entity you wish to include in the early bound code

### Features
* Instead of the generic "error 2" exceptions from crmsvcutil.exe, the relevant trace log message is passed out, allowing for easier and faster troubleshooting when the engine fails to complete.
* No guesswork, only a ConnectionString and Entities are required, allowing any team to easily generate new EarlyBoundCode without having lengthy or inconsistent configurations.

### Limitations
* The early bound code generator uses Microsoft's crmsvcutil.exe which unfortunately does not accept the standard OrganizationService. A ConnectionString allows the same functionality, in a more manual way.

### Roadmap
* We'll see what features become helpful. This is also guided by the spkl library, as this PowerShell command is a wrapper around the spkl implementation.

### Example
    Get-CrmEarlyBoundCode -ConnectionString $CString -Entities ("contact", "account") | Out-File -path "c:\git\CrmEarlyBoundCode.cs"

## Sync-CrmWebResources
Coming soon...
