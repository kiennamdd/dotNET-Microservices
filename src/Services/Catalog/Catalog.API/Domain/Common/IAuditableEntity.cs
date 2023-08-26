using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Domain.Common
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime ModifiedAt { get; set; }
        string ModifiedBy { get; set; }
    }
}