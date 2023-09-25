
using Order.Domain.Common;
using Order.Domain.Exceptions;

namespace Order.Domain.Entities
{
    public class Address : ValueObject
    {
        public int Id { get; private set; }
        public int BuyerId { get; private set; }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        public Address() 
        {
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Country = string.Empty;
            ZipCode = string.Empty;
        }

        public Address(int buyerId, string street, string city, string state, string country, string zipCode)
        {
            BuyerId = buyerId;
            Street = !string.IsNullOrWhiteSpace(street) ? street : throw new OrderDomainException(nameof(street));
            City = !string.IsNullOrWhiteSpace(city) ? city : throw new OrderDomainException(nameof(city));
            State = !string.IsNullOrWhiteSpace(state) ? state : throw new OrderDomainException(nameof(state));
            Country = !string.IsNullOrWhiteSpace(country) ? country : throw new OrderDomainException(nameof(country));
            ZipCode = !string.IsNullOrWhiteSpace(zipCode) ? zipCode : throw new OrderDomainException(nameof(zipCode));
        }

        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = !string.IsNullOrWhiteSpace(street) ? street : throw new OrderDomainException(nameof(street));
            City = !string.IsNullOrWhiteSpace(city) ? city : throw new OrderDomainException(nameof(city));
            State = !string.IsNullOrWhiteSpace(state) ? state : throw new OrderDomainException(nameof(state));
            Country = !string.IsNullOrWhiteSpace(country) ? country : throw new OrderDomainException(nameof(country));
            ZipCode = !string.IsNullOrWhiteSpace(zipCode) ? zipCode : throw new OrderDomainException(nameof(zipCode));
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}