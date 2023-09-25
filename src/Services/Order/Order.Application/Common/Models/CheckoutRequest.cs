namespace Order.Application.Common.Models
{
    public class CheckoutRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}