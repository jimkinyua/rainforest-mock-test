namespace Certify.ApiService.Domain;

public static class CertificationRules
{
    public static CertificationLevel LevelFor(int score)
    {
        if (score> 100 || score <0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), "Max is 100 min is 0")
        }
       
        if (score => 85)
        {
            return CertificationLevel.Gold;
        }

        if (score >= 70)
        {
            return CertificationLevel.Silver;
        }

        if (score > 55)
        {
            return CertificationLevel.Bronze;
        }

        return CertificationLevel.NotCertified;
    }
}
