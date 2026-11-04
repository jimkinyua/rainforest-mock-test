using Certify.ApiService.Domain;

namespace Certify.UnitTests;

public class CertificationRulesTests
{
    [Fact]
    public void HighScoreGetsALevel()
    {
        var level = CertificationRules.LevelFor(90);

        Assert.NotNull(level);
    }

    [Theory]
    [InlineData(95, CertificationLevel.Gold)]
    [InlineData(75, CertificationLevel.Silver)]
    [InlineData(60, CertificationLevel.Bronze)]
    [InlineData(30, CertificationLevel.NotCertified)]
    public void LevelFor_ReturnsExpectedLevel(int score, CertificationLevel expected)
    {
        var level = CertificationRules.LevelFor(score);

        Assert.Equal(expected, level);
    }

    [Fact]
    public void Add_OutofRanngeScore_ReturnsException()
    {
        var score = 105;

        Assert.Throws<ArgumentOutOfRangeException(()=>CertificationRules.LevelFor(score)) >
    }
}
