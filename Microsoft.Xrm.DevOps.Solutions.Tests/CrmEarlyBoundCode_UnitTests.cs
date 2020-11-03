using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Xrm.DevOps.Solutions.Tests
{
    [TestClass]
    public class CrmEarlyBoundCode_UnitTests
    {
        public static string _connectionString = string.Empty;

        [ClassInitialize]
        public static void TestClassInitialize(TestContext context)
        {
            if (context.Properties.Contains("ConnectionString"))
                _connectionString = context.Properties["ConnectionString"].ToString();
        }

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

        // Invalid connection strings throw an "Object reference not set to an instance of an object." error. Could capture and replace, but unsure what part of crmsvcutil is doing this.
        [TestMethod]
        public void CreateClassHelper_WithInvalidConnectionOneEntity_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass("abc", new String[] { "asdfsd" }));
            Assert.IsTrue(ex.Message.Equals("Object reference not set to an instance of an object."));
        }

        [TestMethod]
        public void CreateClassHelper_WithNoEntity_ReturnsException()
        {
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(_connectionString, new String[0]));
            Assert.IsTrue(ex.Message.Equals("LogicalName is required when entity id is not specified"));
        }

        [TestMethod]
        public void CreateClassHelper_WithInvalidEntity_ReturnsException()
        {
            string className = "asdf";
            var ex = Assert.ThrowsException<Exception>(() => Solutions.Helpers.CreateEarlyBoundClass(_connectionString, new String[] { className }));
            Assert.IsTrue(ex.Message.Equals(String.Format("Could not find an entity with name {0} and id 00000000-0000-0000-0000-000000000000.", className)));
        }

        [TestMethod]
        public void CreateClassHelper_WithSingleEntity_ReturnsCode()
        {
            var earlyboundCode = Solutions.Helpers.CreateEarlyBoundClass(_connectionString, new String[] { "contact" });
            Assert.IsInstanceOfType(earlyboundCode, (new String[0]).GetType());
            Assert.IsTrue(earlyboundCode.Length > 0);
        }

        [TestMethod]
        public void CreateClassHelper_WithMultipleEntities_ReturnsCode()
        {
            var earlyboundCode = Solutions.Helpers.CreateEarlyBoundClass(_connectionString, new String[] { "contact", "account" });
            Assert.IsInstanceOfType(earlyboundCode, (new String[0]).GetType());
            Assert.IsTrue(earlyboundCode.Length > 0);
        }
    }
}
