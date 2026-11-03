# Testing practice

A testing-only workout. `Practice.Core` holds small classes and `Practice.Api` is a tiny Minimal API. **You write all the tests** in `Practice.Tests`. `VatCalculatorTests.cs` is a finished example to copy the shape from.

**There are 7 bugs hidden in the code.** Each class has a specification below. Where the code disagrees with the spec, the code is wrong. Your tests should find every bug.

## How to work

For every exercise:

1. **Read the spec. Write the test from the spec, not from the code.** If you read the code first, you'll test what it *does*, not what it *should* do.
2. **Run it.** A red test against the spec means you've found a bug. Fix the code, then run it again.
3. **Break the code on purpose** and check a test goes red. A test that can't fail proves nothing.
4. **Name tests** `Method_Condition_Result`, e.g. `Grade_ScoreOf60_ReturnsB`.
5. **Arrange, Act, Assert**, with one Act per test.

Open `src/Practice.sln` and run tests with **Ctrl+R, A** (or `dotnet test` in `src`).

---

## 1. `VatCalculator` (`[Fact]`, `Assert.Equal`)

**Spec:** adds 16% VAT, rounded to 2 decimal places (halves round up). Negative amounts throw `ArgumentOutOfRangeException`.

Two tests are already written. Add:
- 0 → 0
- 10.555 → work out the expected answer **by hand first**, then test it

## 2. `Grader` (`[Theory]`, boundary values)

**Spec:**

| Score | Grade |
|---|---|
| 70 to 100 | A |
| 60 to 69 | B |
| 50 to 59 | C |
| 0 to 49 | Fail |
| below 0 or above 100 | throws `ArgumentOutOfRangeException` |

Write **one** `[Theory]` with a row for **every boundary**, and a separate test for invalid scores.

## 3. `Money` (exceptions, value equality)

**Spec:**
- Amounts can't be negative; currency must be a 3-letter code. It's trimmed and upper-cased (`" kes "` → `"KES"`).
- `Add` and `Subtract` throw `InvalidOperationException` for different currencies.
- `Subtract` throws `InvalidOperationException` if the result would be negative.
- `Multiply` by a negative factor throws `ArgumentOutOfRangeException`.
- Two `Money` values with the same amount and currency are equal.

Test every rule. Hint: `Assert.Throws<T>(() => ...)`.

## 4. `SaccoReturn` (state transitions)

**Spec:**
- A new return is `Draft`, with no submitted or approved dates.
- `Submit` moves **Draft → Submitted** and records the time passed in. Submitting anything other than a draft throws.
- `Approve` moves **Submitted → Approved** and records the time. Approving anything other than a submitted return throws.
- An empty SACCO name throws.

Test every allowed transition **and every forbidden one**.

## 5. `ShoppingCart` (collections, calculations)

**Spec:**
- Quantity must be at least 1.
- Adding a SKU already in the cart increases its quantity (one line per SKU).
- `Subtotal` is the sum of the lines; `Total` applies any discount.
- Discount code `SAVE10` gives 10% off when the subtotal is **1,000 or more**. Any other code throws `ArgumentException`.

Hints: `Assert.Single(cart.Lines)`, `Assert.Equal(3, cart.Lines[0].Quantity)`.

## 6. `PasswordPolicy` (asserting on a list)

**Spec:** returns a list of errors, empty if the password is valid:
- null or empty → only `"Password is required."`
- shorter than 8 characters, no digit, no uppercase letter → one error each

Hints: `Assert.Empty(...)`, `Assert.Contains("...", errors)`, `Assert.Equal(3, errors.Count)`.

## 7. `Converter` (a fake dependency)

**Spec:** converts using the rate from `IExchangeRates`, rounded to 2 decimal places. Converting to the **same** currency (in any letter case) returns the amount **without asking for a rate**.

1. Write a `FakeExchangeRates : IExchangeRates` that returns a fixed rate and **counts how many times it was called**.
2. Test a conversion with a known rate.
3. Test that a same-currency conversion makes **zero** calls.

## 8. `OrderService` (async, fakes, the order of calls)

**Spec:**
- An order with no lines throws `InvalidOperationException` and nothing is saved or sent.
- A valid order is **saved**, and then a confirmation email is sent.
- If saving fails, **no email is sent**.

Write fakes for `IOrderRepository` (one that can be told to throw) and `IEmailSender` (one that records what it sent). Tests are `async Task` and use `await` and `Assert.ThrowsAsync<T>`.

## 9. `Practice.Api` (integration test with `WebApplicationFactory`)

This runs the real API in memory: real routing, real JSON, no Aspire or Docker needed.

```csharp
public class ProductApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Example()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/products/1");
        // Assert...
    }
}
```

**Spec:**
- `GET /products/{id}` → 200 with the product, or **404** if it doesn't exist.
- `POST /products` → **400** if the name is empty **or the price is 0 or less**; otherwise **201 Created** with a `Location` header pointing at the new product.
- `GET` on that `Location` returns the new product.

You'll need `using Microsoft.AspNetCore.Mvc.Testing;`, `using System.Net;` and `using System.Net.Http.Json;`.

---

## When you're done

Count the bugs you found (there are 7), then compare with the `answer-key` branch:

```
git fetch origin
git checkout answer-key -- testing-practice/ANSWERS.md
```
