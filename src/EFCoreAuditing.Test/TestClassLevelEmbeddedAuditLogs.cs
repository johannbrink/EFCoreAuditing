using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace EFCoreAuditing.Test
{
    [TestClass]
    public class TestClassLevelEmbeddedAuditLogs
    {
        [TestMethod]
        public void Test_Logging_And_DoNotAudit_Attribute()
        {
            using (var myDbContext = new MyDBContext())
            {
                myDbContext.Database.EnsureDeleted();
                myDbContext.Database.EnsureCreated();

                var customerHistory = new CustomerHistory()
                {
                    FirstName = "TestFirstName",
                    LastName = "TEstLAstNAme"
                };
                myDbContext.CustomerHistory.Add(customerHistory);

                myDbContext.SaveChanges("Test User");

                customerHistory.LastName = "TestLastName"; // This should throw an exception below
                myDbContext.SaveChanges("Test User");

                Debug.WriteLine($"Added object that should result in 0 auditable properties.");

                var addedAuditLogs = myDbContext.GetAuditLogs().Where(_=>_.EventType=="Added").ToList();
                var modifiedAuditLogs = myDbContext.GetAuditLogs().Where(_ => _.EventType == "Modified").ToList();
                Debug.WriteLine($"Audit log contains {addedAuditLogs.Count()} added entries.");
                Debug.WriteLine($"Audit log contains {modifiedAuditLogs.Count()} modified entries.");
                foreach (var auditLog in myDbContext.GetAuditLogs())
                {
                    Debug.WriteLine($"AuditLogId:{auditLog.AuditLogId} TableName:{auditLog.TableName} ColumnName:{auditLog.ColumnName} OriginalValue:{auditLog.OriginalValue} NewValue:{auditLog.NewValue} EventDateTime:{auditLog.EventDateTime} EventType:{auditLog.EventType}");
                }

                Assert.AreEqual(addedAuditLogs.Count() , 0);
                Assert.AreEqual(modifiedAuditLogs.Count(), 0);

                myDbContext.Database.EnsureDeleted();
            }
        }
    }
}
