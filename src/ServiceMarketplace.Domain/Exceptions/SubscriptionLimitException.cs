namespace ServiceMarketplace.Domain.Exceptions;

public class SubscriptionLimitException : DomainException
{
    public SubscriptionLimitException(string message = "Subscription limit reached.") : base(message) { }
}
