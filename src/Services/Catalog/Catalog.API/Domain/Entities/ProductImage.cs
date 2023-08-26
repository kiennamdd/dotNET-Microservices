using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Domain.Common;

namespace Catalog.API.Domain.Entities
{
    public class ProductImage: AuditableEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public string ImageName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageLocalPath { get; set; } = string.Empty;
    }
}