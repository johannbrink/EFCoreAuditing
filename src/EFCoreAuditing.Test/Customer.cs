using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFCoreAuditing.Test
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Timestamp]
        [DoNotAudit]
        public byte[] ConcurrencyStamp { get; set; }
    }
}
