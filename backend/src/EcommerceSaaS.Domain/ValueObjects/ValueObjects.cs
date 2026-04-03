namespace EcommerceSaaS.Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
            throw new ArgumentException("Invalid email address");
        Value = value.ToLower();
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Email email && email.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
}

public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }

    public override bool Equals(object? obj) => 
        obj is Money money && 
        money.Amount == Amount && 
        money.Currency == Currency;
    
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }

    public Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    public override bool Equals(object? obj) =>
        obj is Address address &&
        address.Street == Street &&
        address.City == City &&
        address.State == State &&
        address.ZipCode == ZipCode &&
        address.Country == Country;

    public override int GetHashCode() => 
        HashCode.Combine(Street, City, State, ZipCode, Country);
}
