cd C:\Git\Microsoft.Xrm.DevOps.Solutions-Github\Microsoft.Xrm.DevOps.Solutions.PowerShell\bin\Debug\net4.6.2
import-module .\Microsoft.Xrm.DevOps.Solutions.PowerShell.dll -Force
Get-CrmEarlyBoundCode -ConnectionString "sdf" -Entities "sdfdd"

$ConnectionString = "authtype=ClientSecret;url=https://url.crm.dynamics.com;ClientId=clientid;ClientSecret=secret";
#$Conn = Get-CrmConnection -ConnectionString $ConnectionString;
Get-CrmEarlyBoundCode -ConnectionString $ConnectionString -Entities "contact";