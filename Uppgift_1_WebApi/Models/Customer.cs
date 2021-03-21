using System;
using System.Collections.Generic;

#nullable disable

namespace Uppgift_1_WebApi.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Cases = new HashSet<Case>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Case> Cases { get; set; }
    }
}
