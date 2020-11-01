using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions.Libraries
{
    class OnlineAuthorityStrategy : WebResourceStrategy
    {
        public OnlineAuthorityStrategy(
            IOrganizationService Conn, FileHeader LocalFileHeader, WebResourceHeader OnlineWebResourceHeader,
            String webResourcePath) :base(Conn, LocalFileHeader, OnlineWebResourceHeader, webResourcePath)
        {
            this.Target = SyncTarget.Local;

            //var onlineContent = base.RetrieveOnlineWebResource(webResourcePath);
            //var localContent = base.RetrieveLocalWebResource(webResourcePath);

            //if (String.IsNullOrWhiteSpace(localContent))
            {
                this.Action = SyncAction.Create;
            }
            //else if (String.Compare(onlineContent, localContent, true) != 0)
            {
                this.Action = SyncAction.Update;
            }
            //else
            {
                this.Action = SyncAction.Ignore;
            }
        }
    }
}
