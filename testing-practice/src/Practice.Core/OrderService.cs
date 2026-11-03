namespace Practice.Core;

public record Order(Guid Id, string CustomerEmail, IReadOnlyList<CartLine> Lines);

public interface IOrderRepository
{
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, CancellationToken cancellationToken = default);
}

public class OrderService(IOrderRepository repository, IEmailSender emailSender)
{
    public async Task PlaceOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order.Lines.Count == 0)
        {
            throw new InvalidOperationException("An order must have at least one line.");
        }

        await emailSender.SendAsync(order.CustomerEmail, $"Order {order.Id} confirmed", cancellationToken);
        await repository.SaveAsync(order, cancellationToken);
    }
}
