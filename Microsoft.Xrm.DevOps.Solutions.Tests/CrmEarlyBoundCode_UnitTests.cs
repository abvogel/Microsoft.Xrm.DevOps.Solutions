using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Xrm.DevOps.Solutions.Tests
{
    [TestClass]
    public class CrmEarlyBoundCode_UnitTests
    {
        [TestMethod]
        public void CreateClassHelper_WithEmptyConnectionNoEntities_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(String.Empty, new string[0]));
            Assert.IsTrue(ex.Message.Equals("Missing connection string."));
        }

        [TestMethod]
        public void CreateClassHelper_WithEmptyConnectionOneEntity_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(String.Empty, new String[] { "asdfsd" }));
            Assert.IsTrue(ex.Message.Equals("Missing connection string."));
        }

        [TestMethod]
        public void CreateClassHelper_WithInvalidConnectionOneEntity_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass("abc", new String[] { "asdfsd" }));
            Assert.IsTrue(ex.Message.Equals("Organization cannot be null or empty."));
        }

        [TestMethod]
        public void CreateClassHelper_WithNoEntity_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(ConfigurationManager.AppSettings["ConnectionString"], new String[0]));
            Assert.IsTrue(ex.Message.Equals("LogicalName is required when entity id is not specified"));
        }

        [TestMethod]
        public void CreateClassHelper_WithInvalidEntity_ReturnsException()
        {
            string className = "asdf";
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(ConfigurationManager.AppSettings["ConnectionString"], new String[] { className }));
            Assert.IsTrue(ex.Message.Equals(String.Format("Could not find an entity with name {0} and id 00000000-0000-0000-0000-000000000000.", className)));
        }

        [TestMethod]
        public void CreateClassHelper_WithSingleEntity_ReturnsCode()
        {
            var earlyboundCode = Solutions.Helpers.CreateEarlyBoundClass(ConfigurationManager.AppSettings["ConnectionString"], new String[] { "contact" });
            Assert.IsInstanceOfType(earlyboundCode, (new String[0]).GetType());
            Assert.IsTrue(earlyboundCode.Length > 0);
        }

        [TestMethod]
        public void CreateClassHelper_WithMultipleEntities_ReturnsCode()
        {
            var earlyboundCode = Solutions.Helpers.CreateEarlyBoundClass(ConfigurationManager.AppSettings["ConnectionString"], new String[] { "contact", "account" });
            Assert.IsInstanceOfType(earlyboundCode, (new String[0]).GetType());
            Assert.IsTrue(earlyboundCode.Length > 0);
        }
    }
}
