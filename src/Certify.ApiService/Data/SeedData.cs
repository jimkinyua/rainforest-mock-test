using Certify.ApiService.Domain;

namespace Certify.ApiService.Data;

public static class SeedData
{
    public static void Initialise(CertifyDbContext db)
    {
        if (db.Farms.Any())
        {
            return;
        }

        var farms = new List<Farm>
        {
            NewFarm("Kericho Highlands Tea", "Kenya", (92, "A. Otieno", -400), (85, "A. Otieno", -30)),
            NewFarm("Nyeri Coffee Growers", "Kenya", (70, "M. Wanjiru", -60)),
            NewFarm("Sidama Coffee Co-op", "Ethiopia", (55, "T. Bekele", -90)),
            NewFarm("Ashanti Cocoa Farmers", "Ghana", (40, "K. Mensah", -1200)),
        };

        db.Farms.AddRange(farms);
        db.SaveChanges();
    }

    private static Farm NewFarm(string name, string country, params (int Score, string Auditor, int DaysAgo)[] audits)
    {
        var farm = new Farm { Name = name, Country = country };

        foreach (var audit in audits)
        {
            farm.Audits.Add(new Audit
            {
                Score = audit.Score,
                Level = CertificationRules.LevelFor(audit.Score),
                Auditor = audit.Auditor,
                AuditedOn = DateTime.Now.AddDays(audit.DaysAgo)
            });
        }

        return farm;
    }
}
