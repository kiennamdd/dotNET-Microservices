
using Order.Domain.Common;
using Order.Domain.Events;
using Order.Domain.Exceptions;

namespace Order.Domain.Entities
{
    public class Buyer: Entity<int>
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; private set; }

        public readonly List<Address> _addresses;
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        public Buyer() 
        {
            UserEmail = string.Empty;
            _addresses = new List<Address>();
        }

        public Buyer(Guid userId, string userName)
        {
            UserId = userId != Guid.Empty ? userId : throw new OrderDomainException("UserId can not be empty Guid.");
            UserEmail = !string.IsNullOrWhiteSpace(userName) ? userName : throw new OrderDomainException("UserName can not be empty.");

            _addresses = new List<Address>();
        }

        public Address VerifyOrAddAddress(Address address)
        {
            Address? existedAddress = _addresses.SingleOrDefault(o => o == address);

            if(existedAddress is null)
            {
                existedAddress = address;
                _addresses.Add(address);
            }
            
            return existedAddress;
        }
    }
}