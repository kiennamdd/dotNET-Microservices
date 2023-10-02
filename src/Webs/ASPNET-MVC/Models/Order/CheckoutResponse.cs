namespace ASPNET_MVC.Models.Order
{
    public class CheckoutResponse
    {
        public string StripeSessionId { get; set; } = string.Empty;
        public string StripeCheckoutUrl { get; set; } = string.Empty;
    }
}