

namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string ExpirationDate { get; } = default!;
        public string CVV { get; } = default!;
        public int PaymentMethod { get; } = default!;

        protected Payment() { }

        private Payment(string? name, string number, string expDate, string cvv, int paymentMethod)
        {
            CardName = name; CardNumber = number; ExpirationDate = expDate; CVV = cvv; PaymentMethod = paymentMethod;
        }

        public Payment Of(string? name, string number, string expDate, string cvv, int paymentMethod)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(number);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(cvv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 3);

            return new Payment(name, number, expDate, cvv, paymentMethod);


        }
    }
}
