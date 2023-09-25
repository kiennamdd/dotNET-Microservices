namespace Order.Application.Common.Models
{
    public class CheckoutResponse
    {
        public string StripeSessionId { get; set; } = string.Empty;
        public string StripeCheckoutUrl { get; set; } = string.Empty;
    }
}