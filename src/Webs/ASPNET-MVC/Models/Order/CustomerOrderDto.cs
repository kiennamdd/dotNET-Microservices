namespace ASPNET_MVC.Models.Order
{
    public class CustomerOrderDto
    {
        public Guid Id { get; set; }
        public AddressDto? Address { get; set; }
        public double OrderTotal { get; set; }
        public DateTime OrderDate { get; set; } 
        public DateTime PaidDate { get; set; } 
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set;} = new List<OrderItemDto>();
    }
}