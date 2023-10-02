using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Models.Discount
{
    public class CouponCreateRequest
    {
        [Required]
        [MinLength(4)]
        public string CouponCode { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public double MinOrderTotal { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int DiscountPercent { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double DiscountAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double MaxDiscountAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ExpiredDate { get; set; }
    }
}
