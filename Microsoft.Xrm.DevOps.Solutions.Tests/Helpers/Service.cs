using FakeXrmEasy;
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
    static class Service
    {
        public static IOrganizationService Initialize()
        {
            var fakedContext = new XrmFakedContext();
            fakedContext.InitializeMetadata(typeof(CrmEarlyBound.WebResource).Assembly);
            return fakedContext.GetOrganizationService();
        }
        public static IOrganizationService Initialize(Entity entity)
        {
            var fakedContext = new XrmFakedContext();
            fakedContext.InitializeMetadata(typeof(CrmEarlyBound.WebResource).Assembly);
            fakedContext.Initialize(entity);
            return fakedContext.GetOrganizationService();
        }
    }
}
