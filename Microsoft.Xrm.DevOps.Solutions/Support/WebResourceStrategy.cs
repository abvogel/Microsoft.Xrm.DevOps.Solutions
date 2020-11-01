using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions.Libraries
{
    abstract class WebResourceStrategy
    {
        protected String WebResourcePath = String.Empty;
        protected IOrganizationService Conn;
        protected FileHeader localFileHeader = new FileHeader();
        protected WebResourceHeader onlineWebResourceHeader = new WebResourceHeader();

        protected SyncAction Action = SyncAction.Ignore;
        protected SyncTarget Target = SyncTarget.Online;

        public WebResourceStrategy(IOrganizationService Conn, FileHeader LocalFileHeader, WebResourceHeader OnlineWebResourceHeader,
            String webResourcePath)
        {
            this.Conn = Conn;
            this.WebResourcePath = webResourcePath;
            this.localFileHeader = LocalFileHeader;
            this.onlineWebResourceHeader = OnlineWebResourceHeader;
        }

        internal string RetrieveLocalContent(string webResourcePath)
        {
            this.WebResourcePath = webResourcePath;
            this.localFileHeader.Content = System.IO.File.ReadAllText(this.localFileHeader.FullPath);
            return this.localFileHeader.Content;
        }

        internal string RetrieveOnlineContent(string webResourcePath)
        {
            this.WebResourcePath = webResourcePath;
            this.onlineWebResourceHeader.Entity = RetrieveEntityOnline();

            if (this.onlineWebResourceHeader.Entity == null)
                return null;

            return this.onlineWebResourceHeader.Entity.GetAttributeValue<String>("content");
        }

        public virtual SyncAction GetAction()
        {
            return this.Action;
        }

        public virtual void Execute()
        {
            switch (this.Action)
            {
                case SyncAction.Create:
                case SyncAction.Update:
                    if (this.Target == SyncTarget.Online)
                        UpsertOnlineWebResource();
                    break;
                case SyncAction.Ignore:
                    break;
                default:
                    break;
            }
        }

        private void UpsertOnlineWebResource()
        {
            var targetEntity = new Entity("webresource");
            targetEntity["content"] = this.localFileHeader.Content;

            if (this.onlineWebResourceHeader.Entity != null)
            {
                targetEntity.Id = this.onlineWebResourceHeader.Entity.Id;
            } 
            else
            {
                targetEntity["name"] = this.WebResourcePath;
                targetEntity["displayname"] = this.localFileHeader.FileName;
            }

            var request = new Sdk.Messages.UpsertRequest();
            request.Target = targetEntity;

            try
            {
                var response = this.Conn.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
                throw ex;
            }
        }

        private Entity RetrieveEntityOnline()
        {
            var webresourceQuery = new QueryExpression("webresource");
            webresourceQuery.TopCount = 50;
            webresourceQuery.ColumnSet.AddColumns("content", "webresourceid", "name", "displayname", "description", "isenabledformobileclient", "isavailableformobileoffline", "languagecode", "modifiedon");
            webresourceQuery.Criteria.AddCondition("name", ConditionOperator.Equal, this.WebResourcePath);

            EntityCollection webresourceResult = this.Conn.RetrieveMultiple(webresourceQuery);

            if (webresourceResult.Entities.Count == 0)
            {
                webresourceQuery.Criteria.Conditions.Clear();
                webresourceQuery.Criteria.AddCondition("displayname", ConditionOperator.Equal, this.WebResourcePath);
                webresourceResult = this.Conn.RetrieveMultiple(webresourceQuery);
            }

            if (webresourceResult.Entities.Count == 1)
            {
                return webresourceResult.Entities.First();
            }
            else
            {
                return null;
            }
        }
    }
}
