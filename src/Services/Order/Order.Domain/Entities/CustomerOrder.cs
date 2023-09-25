using Order.Domain.Common;
using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.Exceptions;

namespace Order.Domain.Entities
{
    public class CustomerOrder: AuditableEntity<Guid>
    {
        // Reference to Buyer
        public int? BuyerId { get; private set; }
        public Buyer? Buyer { get; private set; }

        // Reference to Address
        public int? AddressId { get; private set; }
        public Address? Address { get; private set; }

        // Order information
        public double OrderTotal { get; private set; }
        public DateTime OrderDate { get; private set; } 
        public DateTime PaidDate { get; private set; } 
        public OrderStatus Status { get; private set; }
        public string Description { get; private set; }
        public bool IsPaid { get; private set; }

        // Discount information
        public string? AppliedCouponCode { get; private set; }
        public double DiscountAmount { get; private set; }
        public double DiscountPercent { get; private set; }

        // Stripe checkout
        public string? StripePaymentIntentId { get; private set; }
        public string? StripeSessionId { get; private set; }

        // Reference to order items
        private readonly List<OrderItem> _items;
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // For migration
        public CustomerOrder() 
        {
            Description = string.Empty;
            _items = new List<OrderItem>();
            Status = OrderStatus.Unknown;
        }

        public CustomerOrder(Guid userId, string userName, Address address,
                    string? appliedCouponCode, double discountAmount, double discountPercent, 
                    double orderTotal)
        {
            // Address and Buyer should be verified or updated before saving to Order
            _items = new List<OrderItem>(); 
            
            OrderTotal = orderTotal;
            OrderDate = DateTime.Now;
            Status = OrderStatus.AwaitingPayment;
            Description = "Awaiting Payment";

            AppliedCouponCode = appliedCouponCode;
            DiscountAmount = discountAmount >= 0 ? discountAmount : throw new OrderDomainException("DiscountAmount can not be negative value.");
            DiscountPercent = discountPercent >= 0 ? discountPercent : throw new OrderDomainException("DiscountPercent can not be negative value.");

            IsPaid = false;

            AddDomainEvent(new OrderStartedDomainEvent(userId, userName, address, this));
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            var existedItem = _items.SingleOrDefault(o => o.ProductId == orderItem.ProductId);
            if(existedItem is null)
            {
                _items.Add(orderItem);
            }
            else
            {
                existedItem.IncreaseQuantity(orderItem.Quantity);
            }
        }

        public void SetStripeSessionId(string stripeSessionId)
        {
            StripeSessionId = !string.IsNullOrWhiteSpace(stripeSessionId)
                            ? stripeSessionId
                            : throw new OrderDomainException("StripeSessionId can not be empty");
        }

        public void SetStripePaymentIntentId(string stripePaymentIntentId)
        {
            StripePaymentIntentId = !string.IsNullOrWhiteSpace(stripePaymentIntentId) 
                                        ? stripePaymentIntentId
                                        : throw new OrderDomainException("StripePaymentIntentId can not be empty");
        }

        public void SetBuyer(int buyerId)
        {
            BuyerId = buyerId;
        }

        public void SetBuyer(Buyer buyer)
        {
            Buyer = buyer;
        }

        public void SetAddress(int addressId)
        {
            AddressId = addressId;
        }

        public void SetAddress(Address address)
        {
            Address = address;
        }

        public void SetStatusToPaid()
        {
            if(Status != OrderStatus.AwaitingPayment)
                throw new UnableToChangeOrderStatusException(Status, OrderStatus.Paid);

            Status = OrderStatus.Paid;
            PaidDate = DateTime.Now;
            IsPaid = true;
            Description = "Paid";

            AddDomainEvent(new OrderPaidDomainEvent(Id, Items));
        }

        public void SetStatusToShipped()
        {
            if(Status != OrderStatus.Paid)
                throw new UnableToChangeOrderStatusException(Status, OrderStatus.Shipped);

            Status = OrderStatus.Shipped;
            Description = "Shipped";
            AddDomainEvent(new OrderShippedDomainEvent(Id, Items));
        }

        public void SetStatusToCancelled()
        {
            if(Status != OrderStatus.AwaitingPayment && Status != OrderStatus.Paid)
                throw new UnableToChangeOrderStatusException(Status, OrderStatus.Cancelled);

            Status = OrderStatus.Cancelled;
            Description = "Cancelled";
            AddDomainEvent(new OrderCancelledDomainEvent(Id, Items));
        }

        public void SetStatusToRefunded()
        {
            if(IsPaid == false)
                throw new UnableToChangeOrderStatusException(Status, OrderStatus.Refunded);

            if((DateTime.Now - PaidDate).TotalDays > 30)
                throw new OrderDomainException("Unable to refund order had been paid more than 30 days ago.");

            Status = OrderStatus.Refunded;
            Description = "Refunded";
            AddDomainEvent(new OrderRefundedDomainEvent(Id, OrderTotal, Items));
        }
    }
}