
namespace Order.Domain.Exceptions
{
    public class OrderDomainException: Exception
    {
        public OrderDomainException()
        {
            
        }

        public OrderDomainException(string message)
            :base(message)
        {
            
        }
    }
}