# Testing practice: answers

## The 7 bugs

| # | Where | Bug | Test that finds it |
|---|---|---|---|
| 1 | `Grader.Grade` | `score > 60`, so **60 → C** instead of B | `[InlineData(60, "B")]` |
| 2 | `Money.Subtract` | **No currency check**: 100 KES − 10 USD gives 90 KES | `Assert.Throws<InvalidOperationException>(() => kes.Subtract(usd))` |
| 3 | `SaccoReturn.Approve` | Only rejects already-approved, so **Draft → Approved** is allowed | Approve a new (draft) return → expect `InvalidOperationException` |
| 4 | `ShoppingCart.ApplyDiscount` | `Subtotal > 1000`, so **exactly 1,000 gets no discount** | Subtotal 1,000 + `SAVE10` → `Total == 900` |
| 5 | `OrderService.PlaceOrderAsync` | **Sends the email before saving.** If the save fails, the customer was already told it succeeded | Fake repository that throws → assert the fake email sender **sent nothing** |
| 6 | `POST /products` | Returns **200 OK**, not **201 Created**, and **no `Location` header** | `Assert.Equal(HttpStatusCode.Created, response.StatusCode)` and `Assert.NotNull(response.Headers.Location)` |
| 7 | `POST /products` | **Price isn't validated**: 0 and negative prices are accepted | POST with price 0 → expect 400 |

## Classes with no bug (tests should pass first time)

- `VatCalculator`: 10.555 × 1.16 = 12.2438 → **12.24**
- `PasswordPolicy`: `"abc"` returns all three errors; `null` and `""` return only "required"
- `Converter`: same currency in any case returns the amount with **0** rate calls
- `ShoppingCart.AddItem`: merging the same SKU works

Testing correct code matters too: these tests protect against **future** bugs (regressions).

## Fixes

1. `score >= 60`
2. Add the currency check from `Add` to `Subtract`
3. `if (Status != ReturnStatus.Submitted) throw ...`
4. `Subtotal >= 1000m`
5. Swap the two lines: `await repository.SaveAsync(...)` first, then send the email
6. `return Results.Created($"/products/{product.Id}", product);`
7. Add `request.Price <= 0` to the validation, with its own error key (`"Price"`)

## Example: the trickiest test (bug 5)

```csharp
[Fact]
public async Task PlaceOrderAsync_SaveFails_SendsNoEmail()
{
    // Arrange
    var repository = new FakeOrderRepository { ShouldThrow = true };
    var email = new FakeEmailSender();
    var service = new OrderService(repository, email);
    var order = new Order(Guid.NewGuid(), "a@b.c", [new CartLine("TEA", 450m, 1)]);

    // Act
    await Assert.ThrowsAnyAsync<Exception>(() => service.PlaceOrderAsync(order));

    // Assert
    Assert.Empty(email.Sent);
}

public class FakeOrderRepository : IOrderRepository
{
    public bool ShouldThrow { get; set; }
    public List<Order> Saved { get; } = [];

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (ShouldThrow) throw new InvalidOperationException("Database down");
        Saved.Add(order);
        return Task.CompletedTask;
    }
}

public class FakeEmailSender : IEmailSender
{
    public List<string> Sent { get; } = [];

    public Task SendAsync(string to, string subject, CancellationToken cancellationToken = default)
    {
        Sent.Add(to);
        return Task.CompletedTask;
    }
}
```

This is an **interaction test**: it checks what the class *did* to its dependencies, not just what it returned.

## Scoring

- 7/7 bugs found from the specs → strong.
- Tests named `Method_Condition_Result`, Arrange/Act/Assert, one Act each → good habits.
- You broke each fix and saw a test go red → you trust your tests.
