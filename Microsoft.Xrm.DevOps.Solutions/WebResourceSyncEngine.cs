using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using SparkleXrm.Tasks;
using SparkleXrm.Tasks.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xrm.DevOps.Solutions.Libraries;

namespace Microsoft.Xrm.DevOps.Solutions
{
    public enum SyncAuthority
    {
        Local,
        Online,
        Latest
    }

    public enum SyncAction
    {
        Create,
        Ignore,
        Update
    }

    public enum SyncTarget
    {
        Local,
        Online
    }

    public class WebResourceSyncEngine
    {
        public string Path { get; set; }
        public IOrganizationService Conn { get; set; }
        public string SolutionName { get; set; }

        private SyncAuthority Authority = SyncAuthority.Local;
        private Guid SolutionId
        {
            get {
                if (null != _solutionId && Guid.Empty != _solutionId)
                    return _solutionId;
                
                var solutionQuery = new QueryExpression("solution");
                    solutionQuery.ColumnSet.AddColumns("uniquename", "friendlyname", "publisherid", "solutionid");
                    solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, SolutionName);
                var results = Conn.RetrieveMultiple(solutionQuery);
                if (results.Entities.Count.Equals(1))
                {
                    _solutionId = results.Entities[0].Id;
                }
                
                return _solutionId;
            }
        }
        private Guid _solutionId = Guid.Empty;

        private Dictionary<String,WebResourceHeader> OnlineWebResources
        {
            get
            {
                if (_onlineWebResources.Count > 0)
                    return _onlineWebResources;

                List<Guid> _onlineWebResourceIds = new List<Guid>();
                var solutionComponentQuery = new QueryExpression("solutioncomponent");
                    solutionComponentQuery.ColumnSet.AddColumns("objectid");
                    solutionComponentQuery.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 61);
                    solutionComponentQuery.Criteria.AddCondition("solutionid", ConditionOperator.Equal, SolutionId);
                var componentResults = Conn.RetrieveMultiple(solutionComponentQuery);
                _onlineWebResourceIds = componentResults.Entities.Select<Entity, Guid>(x => x.Id).ToList<Guid>();

                if (_onlineWebResourceIds.Count == 0)
                    return _onlineWebResources;

                var webResourceQuery = new QueryExpression("webresource");
                    webResourceQuery.ColumnSet.AddColumns("webresourceid", "name", "displayname", "description", "isenabledformobileclient", "isavailableformobileoffline", "languagecode", "modifiedon"); //, "contentjson");
                    webResourceQuery.Criteria.AddCondition("ismanaged", ConditionOperator.NotEqual, true);
                    webResourceQuery.Criteria.AddCondition("webresourceid", ConditionOperator.In, _onlineWebResourceIds);
                var results = Conn.RetrieveMultiple(webResourceQuery);

                foreach (Entity webResource in results.Entities)
                {
                    if (_onlineWebResources.ContainsKey((String)webResource.Attributes["name"]))
                        continue;

                    _onlineWebResources.Add(((String)webResource.Attributes["name"]).ToLower(),
                        new WebResourceHeader
                        {
                            RelativeFilePath = (String)webResource.Attributes["name"],
                            FileName = (String)webResource.Attributes["displayname"],
                            Entity = webResource
                        }) ;
                }

                return _onlineWebResources;
            }
        }
        private Dictionary<String,WebResourceHeader> _onlineWebResources = new Dictionary<String,WebResourceHeader>();

        private Dictionary<String,FileHeader> LocalWebResources
        {
            get
            {
                if (_localWebResources.Count > 0)
                    return _localWebResources;

                List<String> fullFilePaths = Directory.GetFiles(Path, "*", SearchOption.AllDirectories).ToList();

                foreach (String fullFilePath in fullFilePaths)
                {
                    if (Path.EndsWith("\\"))
                        Path = Path.Substring(0, (Path.Length - 1));

                    var relativeFilePath = SanitizePath(fullFilePath.Replace(Path, ""));

                    if (_localWebResources.ContainsKey(relativeFilePath))
                        continue;

                    var fileInfo = new System.IO.FileInfo(fullFilePath);
                    _localWebResources.Add(relativeFilePath.ToLower(),
                         new FileHeader
                         {
                             FileName = fileInfo.Name,
                             RelativeFilePath = relativeFilePath,
                             FullPath = fullFilePath,
                             Extension = fileInfo.Extension,
                             DirectoryPath = fileInfo.DirectoryName,
                             ModifiedOn = fileInfo.LastWriteTime
                         }
                        );
                }
                
                return _localWebResources;
            }
        }
        private Dictionary<String,FileHeader> _localWebResources = new Dictionary<String,FileHeader>();

        private Dictionary<String, WebResourceStrategy> _webResourceStrategies = new Dictionary<String, WebResourceStrategy>();

        public WebResourceSyncEngine(String path, IOrganizationService conn) : this(path, conn, "default") { }

        public WebResourceSyncEngine(String path, IOrganizationService conn, String solutionName)
        {
            this.Path = path;
            this.Conn = conn;
            this.SolutionName = solutionName;
        }

        public void SetAuthority(SyncAuthority Authority)
        {
            this.Authority = Authority;
        }

        public List<string> GetImpactedWebResources()
        {
            switch (this.Authority)
            {
                case SyncAuthority.Local:
                    return this.LocalWebResources.Keys.ToList();
                case SyncAuthority.Online:
                    return this.OnlineWebResources.Keys.ToList();
                case SyncAuthority.Latest:
                    var AllPaths = this.LocalWebResources.Keys.ToList();
                    AllPaths.AddRange(this.OnlineWebResources.Keys.ToList());
                    AllPaths = AllPaths.Distinct().ToList();
                    return AllPaths;
            }

            return new List<String>();
        }

        public SyncAction GetAction(string webResourcePath)
        {
            return GetProcessStrategy(SanitizePath(webResourcePath)).GetAction();
        }

        public void ProcessWebResource(string webResourcePath)
        {
            GetProcessStrategy(SanitizePath(webResourcePath)).Execute();
        }

        string SanitizePath(string webResourcePath)
        {
            if (webResourcePath.Substring(0, 1) == "\\")
                webResourcePath = webResourcePath.Substring(1, webResourcePath.Length - 1);

            if (webResourcePath.Substring(0, 2) == "\\")
                webResourcePath = webResourcePath.Substring(2, webResourcePath.Length - 2);

            return webResourcePath.ToLower();
        }

        private WebResourceStrategy GetProcessStrategy(String webResourcePath)
        {
            if (_webResourceStrategies.ContainsKey(webResourcePath))
                return _webResourceStrategies[webResourcePath];

            FileHeader localFile = this.LocalWebResources.ContainsKey(webResourcePath) ? this.LocalWebResources[webResourcePath] : new FileHeader();
            WebResourceHeader onlineWebResource = this.OnlineWebResources.ContainsKey(webResourcePath) ? this.OnlineWebResources[webResourcePath] : new WebResourceHeader();

            switch (this.Authority)
            {
                case SyncAuthority.Local:
                    _webResourceStrategies[webResourcePath] = new LocalAuthorityWebResourceStrategy(Conn, localFile, onlineWebResource, webResourcePath);
                    break;
                case SyncAuthority.Online:
                    _webResourceStrategies[webResourcePath] = new OnlineAuthorityStrategy(Conn, localFile, onlineWebResource, webResourcePath);
                    break;
                case SyncAuthority.Latest:
                default:
                    _webResourceStrategies[webResourcePath] = new LatestAuthorityWebResourceStrategy(Conn, localFile, onlineWebResource, webResourcePath);
                    break;
            }

            return _webResourceStrategies[webResourcePath];
        }
    }
}
