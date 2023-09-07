
using System.ComponentModel.DataAnnotations;

namespace Cart.API.Domain.Common
{
    public class EntityBase<TKey>
        where TKey: notnull
    {
        [Key]
        public TKey Id { get; private set; } = default!;

        public EntityBase()
        {
            //
        }

        public EntityBase(TKey id)
        {
            Id = id;
        }
    }
}