using System;
using System.Collections.Generic;

#nullable disable

namespace Uppgift_1_WebApi.Models
{
    public partial class Case
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceWorkerId { get; set; }
        public DateTime CaseDate { get; set; }
        public DateTime? CaseResolveDate { get; set; }
        public string CaseDescription { get; set; }
        public string CaseStatus { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ServiceWorker ServiceWorker { get; set; }
    }
}
