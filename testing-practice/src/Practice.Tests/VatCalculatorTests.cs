namespace Practice.Tests;

// A finished example to copy the shape from. Every other test is yours to write.
public class VatCalculatorTests
{
    [Fact]
    public void AddVat_PositiveAmount_AddsSixteenPercent()
    {
        // Arrange
        var net = 100m;

        // Act
        var gross = VatCalculator.AddVat(net);

        // Assert
        Assert.Equal(116m, gross);
    }

    [Fact]
    public void AddVat_NegativeAmount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => VatCalculator.AddVat(-1m));
    }
}
