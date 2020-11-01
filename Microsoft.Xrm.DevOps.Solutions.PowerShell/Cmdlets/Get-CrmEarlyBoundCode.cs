using Microsoft.Xrm.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Xrm.DevOps.Solutions.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "CrmEarlyBoundCode")]
    [OutputType(typeof(Object[]))]
    public class CrmEarlyBoundCode : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public String ConnectionString { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public String[] Entities { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string[] earlyBoundCode = Solutions.Helpers.CreateEarlyBoundClass(ConnectionString, Entities);
                base.WriteObject(earlyBoundCode);
            }
            catch (Exception ex)
            {
                base.WriteError(new ErrorRecord(ex, "CreateEarlyBoundClass", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
