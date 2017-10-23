using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace EFCoreAuditing.Test
{
    [TestClass]
    public class TestPropertyLevelEmbeddedAuditLogs
    {
        [TestMethod]
        public void Test_Logging_And_DoNotAudit_Attribute()
        {
            using (var myDbContext = new MyDBContext())
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

                customer.LastName = "TestLastName"; // This should throw an exception below
                myDbContext.SaveChanges("Test User");

                Debug.WriteLine($"Added object with {auditablePropCount} auditable properties and {nonAuditablePropCount} non-auditable properties.");

                var addedAuditLogs = myDbContext.GetAuditLogs().Where(_=>_.EventType=="Added").ToList();
                var modifiedAuditLogs = myDbContext.GetAuditLogs().Where(_ => _.EventType == "Modified").ToList();
                Debug.WriteLine($"Audit log contains {addedAuditLogs.Count()} added entries.");
                Debug.WriteLine($"Audit log contains {modifiedAuditLogs.Count()} modified entries.");
                foreach (var auditLog in myDbContext.GetAuditLogs())
                {
                    Debug.WriteLine($"AuditLogId:{auditLog.AuditLogId} TableName:{auditLog.TableName} ColumnName:{auditLog.ColumnName} OriginalValue:{auditLog.OriginalValue} NewValue:{auditLog.NewValue} EventDateTime:{auditLog.EventDateTime} EventType:{auditLog.EventType}");
                }

                Assert.AreEqual(addedAuditLogs.Count() , auditablePropCount);
                Assert.AreEqual(modifiedAuditLogs.Count(), 1);

                myDbContext.Database.EnsureDeleted();
            }
        }
    }
}
