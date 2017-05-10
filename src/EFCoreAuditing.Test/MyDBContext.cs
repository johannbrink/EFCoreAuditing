using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreAuditing.Test
{
    public class MyDBContext : AuditingDbContext
    {
        public MyDBContext()
        {

        }

        public MyDBContext(IExternalAuditStoreProvider externalAuditStoreProvider) : base(externalAuditStoreProvider)
        {

        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase();//.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFAuditingTestHarness;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(optionsBuilder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        "Server=(localdb)\\mssqllocaldb;Database=EFAuditingTestHarness;Trusted_Connection=True;MultipleActiveResultSets=true");
        //}
    }
}
