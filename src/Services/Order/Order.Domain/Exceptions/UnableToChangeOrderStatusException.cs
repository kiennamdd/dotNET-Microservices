
using Order.Domain.Enums;

namespace Order.Domain.Exceptions
{
    public class UnableToChangeOrderStatusException: Exception
    {
        public UnableToChangeOrderStatusException(OrderStatus currentStatus, OrderStatus newStatus)
            :base($"Unable to change from status {currentStatus} to {newStatus}")
        {
            //
        }

        public UnableToChangeOrderStatusException(string message)
            :base(message)
        {
            //
        }
    }
}