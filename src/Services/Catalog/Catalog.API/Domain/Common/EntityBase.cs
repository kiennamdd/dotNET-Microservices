using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Domain.Common
{
    public abstract class EntityBase<TKey>
        where TKey: notnull
    {
        [Key]
        public TKey Id { get; protected set; } = default!;
    }
}