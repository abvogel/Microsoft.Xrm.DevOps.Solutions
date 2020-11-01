using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions.Libraries
{
    class LocalAuthorityWebResourceStrategy : WebResourceStrategy
    {
        public LocalAuthorityWebResourceStrategy(
            IOrganizationService Conn, FileHeader LocalFileHeader, WebResourceHeader OnlineWebResourceHeader,
            String webResourcePath) :base(Conn, LocalFileHeader, OnlineWebResourceHeader, webResourcePath)
        {
            this.Target = SyncTarget.Online;

            var onlineContent = base.RetrieveOnlineContent(webResourcePath);
            var localContent = base.RetrieveLocalContent(webResourcePath);

            if (null == onlineContent)
            {
                this.Action = SyncAction.Create;
            }
            else if (String.Compare(onlineContent, localContent) != 0)
            {
                this.Action = SyncAction.Update;
            }
            else
            {
                this.Action = SyncAction.Ignore;
            }
        }
    }
}
