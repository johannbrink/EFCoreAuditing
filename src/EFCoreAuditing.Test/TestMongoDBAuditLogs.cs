using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using EFCoreAuditing.MongoDB;
using System.Collections.Generic;

namespace EFCoreAuditing.Test
{
    [TestClass]
    public class TestMongoDBAuditLogs
    {
        [TestMethod]
        public void Test_MongoLogging_And_DoNotAudit_Attribute()
        {
            var mongoDBAuditStoreProvider = new MongoDbAuditStoreProvider()
                .WithServer("localhost")
                .Start();

            using (var myDbContext = new MyDBContext(mongoDBAuditStoreProvider))
            {
                myDbContext.Database.EnsureDeleted();
                myDbContext.Database.EnsureCreated();

                var customer = new Customer()
                {
                    FirstName = "TestFirstName",
                    LastName = "TEstLAstNAme"
                };
                myDbContext.Customers.Add(customer);

                var auditablePropCount =
                    customer.GetType()
                        .GetProperties()
                        .Count(p => !p.GetCustomAttributes(typeof(DoNotAudit), true).Any());

                var nonAuditablePropCount =
                    customer.GetType()
                        .GetProperties()
                        .Count(p => p.GetCustomAttributes(typeof(DoNotAudit), true).Any());
                myDbContext.SaveChanges("Test User");

                Debug.WriteLine($"Added object with {auditablePropCount} auditable properties and {nonAuditablePropCount} non-auditable properties.");

                var auditLogs = myDbContext.GetAuditLogs()?.ToList();
                Debug.WriteLine($"Audit log contains {auditLogs?.Count()} entries.");
                Assert.AreNotEqual(auditLogs, 0);
                myDbContext.Database.EnsureDeleted();
            }
        }
    }
}
