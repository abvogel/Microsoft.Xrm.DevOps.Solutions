using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Xrm.DevOps.Solutions.Tests
{
    [TestClass]
    public class SyncCrmWebResource_UnitTests
    {
        [TestMethod]
        public void NoFolderOrConnection_Constructs()
        {
            var webResourceSyncEngine = new WebResourceSyncEngine(String.Empty, null);
            Assert.IsInstanceOfType(webResourceSyncEngine, typeof(Microsoft.Xrm.DevOps.Solutions.WebResourceSyncEngine));
        }

        [TestMethod]
        public void FileNoConnection_Constructs()
        {
            var webResourceSyncEngine = new WebResourceSyncEngine(Environment.CurrentDirectory + "\\test\\test_a.html", null);
            Assert.IsInstanceOfType(webResourceSyncEngine, typeof(Microsoft.Xrm.DevOps.Solutions.WebResourceSyncEngine));
        }

        [TestMethod]
        public void FolderNoConnection_Constructs()
        {
            var webResourceSyncEngine = new WebResourceSyncEngine(Environment.CurrentDirectory + "\\test", null);
            Assert.IsInstanceOfType(webResourceSyncEngine, typeof(Microsoft.Xrm.DevOps.Solutions.WebResourceSyncEngine));
        }

        [TestMethod]
        public void File_Constructs()
        {
            var filePath = Solutions.Tests.Helpers.WebResources.GenerateNew("root", "test");
            var service = Solutions.Tests.Helpers.Service.Initialize();
            
            var webResourceSyncEngine = new WebResourceSyncEngine(Environment.CurrentDirectory + "\\root", service);
            Assert.IsInstanceOfType(webResourceSyncEngine, typeof(Microsoft.Xrm.DevOps.Solutions.WebResourceSyncEngine));
        }

        // TwoFiles_ReturnsTwoImpactedFiles
        // TwoFilesAndOnlineAuthority_ReturnsNoFiles
        // TwoFilesAndOneOnlineWithLatestAuthority_ReturnsThreeImpactedFiles

        [TestMethod]
        public void LocalAuthorityAndFile_Creates()
        {
            var relativefilePath = Solutions.Tests.Helpers.WebResources.GenerateNew("root", "test");
            var service = Solutions.Tests.Helpers.Service.Initialize();

            var webResourceSyncEngine = new WebResourceSyncEngine(Environment.CurrentDirectory + "\\root", service);
            webResourceSyncEngine.ProcessWebResource(relativefilePath);

            var newFile = Solutions.Tests.Helpers.WebResources.FindOnline(service, relativefilePath);
            Assert.IsInstanceOfType(newFile, typeof(Microsoft.Xrm.Sdk.Entity));

            var action = webResourceSyncEngine.GetAction(relativefilePath);
            Assert.IsTrue(action == SyncAction.Create);

            Solutions.Tests.Helpers.WebResources.DeleteTmpFile(relativefilePath);
        }

        [TestMethod]
        public void LocalAuthorityAndFileBothSides_Ignores()
        {
            var relativefilePath = Solutions.Tests.Helpers.WebResources.GenerateNew("root", "test");
            Entity webResourceEntity = new Entity("webresource", Guid.NewGuid());
            webResourceEntity["name"] = relativefilePath;
            webResourceEntity["content"] = String.Empty;
            var service = Solutions.Tests.Helpers.Service.Initialize(webResourceEntity);

            var webResourceSyncEngine = new WebResourceSyncEngine(Environment.CurrentDirectory + "\\root", service);
            webResourceSyncEngine.ProcessWebResource(relativefilePath);

            var newFile = Solutions.Tests.Helpers.WebResources.FindOnline(service, relativefilePath);
            Assert.IsInstanceOfType(newFile, typeof(Microsoft.Xrm.Sdk.Entity));

            var action = webResourceSyncEngine.GetAction(relativefilePath);
            Assert.IsTrue(action == SyncAction.Ignore);

            Solutions.Tests.Helpers.WebResources.DeleteTmpFile(relativefilePath);
        }

        //LocalAuthorityAndTwoFilesOneOnline_CreatesOne
        //OnlineAuthorityAndLocalFile_Ignores()
        //OnlineAuthorityAndLocalFileAndDelete_Prompts()
        //OnlineAuthorityAndLocalFileAndDeleteAndOverwrite_Deletes()
        //OnlineAuthorityAndLocalFileAndOverwrite_Ignores()
        //LatestAuthorityAndOneLocal_CreatesOne
        //LatestAuthorityAndOneOnline_CreatesOne
        //LatestAuthorityAndOneLocalOneOnlineLaterLocal_UpdatesOnline
        //LatestAuthorityAndOneLocalOneOnlineLaterOnline_UpdatesLocal
        //LatestAuthorityAndOneLocalOneOnlineDifferentFiles_CreatesLocalAndOnline
        //LocalAuthorityMissingManifest_CreatesManifest
        //
    }
}
