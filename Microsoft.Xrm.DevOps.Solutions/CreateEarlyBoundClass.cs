using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SparkleXrm.Tasks;
using SparkleXrm.Tasks.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Xrm.DevOps.Solutions
{
    public partial class Helpers
    {
        public static String[] CreateEarlyBoundClass(String connectionString, String[] entities)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Missing connection string.");

            var trace = new MemorySpklTraceLogger();
            MemoryTraceLogger traceCollection = new MemoryTraceLogger();

            var temporaryFilePath = System.IO.Path.GetTempFileName();
            var temporaryFile = System.IO.Path.GetFileName(temporaryFilePath);
            var temporaryFolder = System.IO.Path.GetDirectoryName(temporaryFilePath);
            ConfigFile mockConfigFile = GetMockConfigFile(entities, temporaryFile, temporaryFolder);
            var fakeContext = new FakeServiceContext();

            try
            {
                var earlyBound = new EarlyBoundClassGeneratorTask(fakeContext, trace);
                earlyBound.ConectionString = connectionString;
                earlyBound.CreateEarlyBoundTypes(null, mockConfigFile);
            }
            catch (Exception ex)
            {
                for (int i = traceCollection.traces.Count - 1; i > 0; i--)
                {
                    if (!Regex.Match(traceCollection.traces[i], "Exiting program with exception").Success)
                        continue;

                    var snippets = traceCollection.traces[i].Split(new string[] { "Exiting program with exception: " }, StringSplitOptions.RemoveEmptyEntries);
                    throw new Exception(snippets[snippets.Length - 1], ex);
                }

                throw ex;
            }

            System.Collections.Generic.IEnumerable<String> lines = System.IO.File.ReadLines(temporaryFilePath);
            return lines.ToArray<String>();
        }

        private static ConfigFile GetMockConfigFile(string[] entities, string temporaryFile, string temporaryFolder)
        {
            var mockConfigFile = new SparkleXrm.Tasks.Config.ConfigFile
            {
                filePath = temporaryFolder,
                earlyboundtypes = new List<EarlyBoundTypeConfig>()
            };

            mockConfigFile.earlyboundtypes.Add(new EarlyBoundTypeConfig()
            {
                filename = temporaryFile,
                entities = String.Join(",", entities),
                classNamespace = "Xrm",
                generateOptionsetEnums = true,
                generateStateEnums = true
            });
            return mockConfigFile;
        }
    }

    internal class FakeServiceContext : IOrganizationService
    {
        public FakeServiceContext()
        {
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            throw new NotImplementedException();
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
