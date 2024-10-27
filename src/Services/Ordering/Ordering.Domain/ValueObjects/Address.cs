

namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; } = default!;
        public string LastName { get; } = default!;
        public string? EmailAddress { get; } = default!;
        public string AddressLine {  get; } = default!; 
        public string Country { get; } = default!;
        public string State { get; } = default!;
        public string ZipCode {  get; } = default!;

        protected Address() { } //for Entity Framework 

        private Address(string first, string last, string email, string address, string country, string state, string zip)
        {
            FirstName = first;
            LastName = last;
            EmailAddress = email;
            AddressLine = address;
            Country = country;
            State = state;
            ZipCode = zip;
        }

        public static Address Of(string first, string last, string email, string address, string country, string state, string zip)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(address);

            return new Address(first, last, email, address, country, state, zip);
        }


    }
}
