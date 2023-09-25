using Order.Domain.Entities;

namespace Order.Application.Common.Models
{
    public class AddressDto
    {
        public int Id { get; set; }

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    public static class AddressDtoExtension
    {
        public static AddressDto ToAddressDto(this Address address)
        {
            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                Country = address.Country,
                ZipCode = address.ZipCode
            };
        }

        public static IEnumerable<AddressDto> ToAddressDtoList(this IEnumerable<Address> list)
        {
            return list.Select(address => address.ToAddressDto());
        }
    }
}