namespace Certify.ApiService.Domain;

public static class CertificationRules
{
    public static CertificationLevel LevelFor(int score)
    {
        if (score > 85)
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
