using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions.Tests.Helpers
{
    static class WebResources
    {
        public static void Delete(IOrganizationService service, String fileName)
        {
            Entity webresource = WebResources.FindOnline(service, fileName);

            if (null == webresource)
            {
                return;
            }

            try
            {
                service.Delete("webresource", webresource.Id);
            } 
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Entity FindOnline(IOrganizationService service, String fileName)
        {
            var webresourceQuery = new QueryExpression("webresource");
            webresourceQuery.TopCount = 50;
            webresourceQuery.ColumnSet.AddColumns("name", "displayname", "solutionid", "webresourceid");
            webresourceQuery.Criteria.AddCondition("name", ConditionOperator.Equal, fileName);

            EntityCollection webresourceResult = service.RetrieveMultiple(webresourceQuery);

            if (webresourceResult.Entities.Count == 0)
            {
                webresourceQuery.Criteria.Conditions.Clear();
                webresourceQuery.Criteria.AddCondition("displayname", ConditionOperator.Equal, fileName);
                webresourceResult = service.RetrieveMultiple(webresourceQuery);
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

        public static string GenerateNew(string baseFolder, string relativeFolder)
        {
            var temporaryFilePath = System.IO.Path.GetTempFileName();
            var temporaryFile = System.IO.Path.GetFileName(temporaryFilePath);
            var newFilePath = String.Format("{0}\\{1}\\{2}\\{3}", Environment.CurrentDirectory, baseFolder, relativeFolder, temporaryFile);

            System.IO.File.Copy(temporaryFilePath, newFilePath);

            return (String.Format("{0}\\{1}", relativeFolder, temporaryFile));
        }

        internal static void DeleteTmpFile(string relativefilePath)
        {
            System.IO.File.Delete(String.Format("{0}\\{1}\\{2}", Environment.CurrentDirectory, "root", relativefilePath));
        }
    }
}
