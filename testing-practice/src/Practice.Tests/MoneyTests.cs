using System;
using System.Collections.Generic;
using System.Text;

namespace Practice.Tests
{
    public class MoneyTests
    {
        [Theory]
        [InlineData("kes", 100, "KES", 100)]
        [InlineData("usd", 100, "USD", 100)]
        [InlineData("Kes ",100, "KES", 100)]

        public void Validate_TrimmingAndCapitalization_ReturnNomarlised(string currency, decimal amount, string expectedcurrency, decimal expectedamount)
        {
            Money money = new Money(amount, currency);
            Money expectedMoeny = new Money(expectedamount, expectedcurrency);

            Assert.Equal(money, expectedMoeny);
        }
        [Fact]

        public void Validate_AddDiffCurrencies_ReturnInvalidOperation()
        {
            Money fist = new Money(100,"USD");
            Money second = new Money(100, "KES");

            Assert.Throws<InvalidOperationException>(() => fist.Add(second));
        }

        [Fact]
        public void Validate_SubtractDiffCurrencies_ReturnInvalidOperation()
        {
            Money fist = new Money(100, "USD");
            Money second = new Money(100, "KES");

            Assert.Throws<InvalidOperationException>(() => fist.Subtract(second));
        }

        [Fact]
        public void Validate_TwoSimilarMonesAreEqual_ReturnsEqual()
        {
            Money first = new Money(100, "KES");
            Money second = new Money(100, "KES");

            Assert.Equal(first, second);
        }

    }
}
