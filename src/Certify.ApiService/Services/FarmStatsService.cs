using Certify.ApiService.Data;
using Certify.ApiService.Domain;

namespace Certify.ApiService.Services;

public class FarmStatsService(CertifyDbContext db)
{
    public Dictionary<CertificationLevel, int> CountByLevel()
    {
        var audits = db.Audits.ToList();

        return audits
            .GroupBy(a => a.Level)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
