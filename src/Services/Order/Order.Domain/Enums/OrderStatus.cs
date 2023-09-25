
namespace Order.Domain.Enums
{
    public class OrderStatus : Enumeration
    {
        public static OrderStatus Unknown = new(0, nameof(Unknown).ToLowerInvariant());
        public static OrderStatus AwaitingPayment = new(1, nameof(AwaitingPayment).ToLowerInvariant());
        public static OrderStatus Paid = new(2, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new(3, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new(4, nameof(Cancelled).ToLowerInvariant());
        public static OrderStatus Refunded = new(5, nameof(Refunded).ToLowerInvariant());

        public OrderStatus(int id, string name) : base(id, name)
        {

        }

        public static IEnumerable<OrderStatus> GetAll()
        {
            return new List<OrderStatus>()
            {
                Unknown,
                AwaitingPayment,
                Paid,
                Shipped,
                Cancelled,
                Refunded
            };
        }
    }
}