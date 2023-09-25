
namespace Order.Domain.Enums
{
    public class Currency: Enumeration
    {
        public static Currency USD = new(1, nameof(USD).ToLowerInvariant());

        public Currency(int id, string name) : base(id, name)
        {
            
        }
    }
}