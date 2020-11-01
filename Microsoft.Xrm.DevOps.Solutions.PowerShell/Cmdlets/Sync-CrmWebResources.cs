using Microsoft.Xrm.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Web.UI.HtmlControls;

namespace Microsoft.Xrm.DevOps.Solutions.PowerShell.Cmdlets
{
    //[Cmdlet("Sync", "CrmWebResources")]
    //[OutputType(typeof(Object[]))]
    public class SyncCrmWebResources : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public IOrganizationService Conn { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public String Path { get; set; }

        [Parameter(Position = 2, Mandatory = false)]
        public String Solution { get; set; }
        
        [Parameter(Position = 3, Mandatory = false)]
        [ValidateSet("Local", "Online", "Latest")]
        public String Authority { get; set; }

        protected override void ProcessRecord()
        {
            var webResourceSyncEngine = new Solutions.WebResourceSyncEngine(Path, Conn, Solution);

            switch (Authority)
            {
                case "Local":
                    webResourceSyncEngine.SetAuthority(SyncAuthority.Local);
                    break;
                case "Online":
                    webResourceSyncEngine.SetAuthority(SyncAuthority.Online);
                    break;
                case "Latest":
                    webResourceSyncEngine.SetAuthority(SyncAuthority.Latest);
                    break;
            }

            List<string> impactedWebResources = webResourceSyncEngine.GetImpactedWebResources();
            GenerateVerboseMessage(String.Format("Identified {0} web resources.", impactedWebResources.Count));

            foreach (String webResourcePath in impactedWebResources)
            {
                GenerateVerboseMessage(String.Format("{0} - {1}", webResourcePath, webResourceSyncEngine.GetAction(webResourcePath)));

                try
                {
                    webResourceSyncEngine.ProcessWebResource(webResourcePath);
                }
                catch (Exception ex)
                {
                    GenerateVerboseMessage(String.Format("{0} failed with error message: {1}", webResourcePath, ex.Message));
                    continue;
                }
            }
        }

        private void GenerateVerboseMessage(string v)
        {
            WriteVerbose(String.Format("{0} {1}: {2}", DateTime.Now.ToShortTimeString(), "Sync-CrmWebResources", v));
        }
    }
}
