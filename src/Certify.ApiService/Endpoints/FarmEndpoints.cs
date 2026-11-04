using Certify.ApiService.Data;
using Certify.ApiService.Domain;
using Certify.ApiService.DTO;
using Certify.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Certify.ApiService.Endpoints;

public static class FarmEndpoints
{
    public static void MapFarmEndpoints(this WebApplication app)
    {
        app.MapGet("/farms", async (string? country, CertifyDbContext db, CancellationToken ct) =>
        {
            IQueryable<Farm> farmQuery = db.Farms;

            if (!string.IsNullOrEmpty(country))
            {
                farmQuery = farmQuery.Where(x => x.Country ==country);
            }
            var farms = await farmQuery
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new FarmItem
            {
                Id = f.Id,
                Name = f.Name,
                Country = f.Country,
                Audits = f.Audits.Select(a => new AuditItem
                {
                    Level = a.Level,
                    Score = a.Score,
                    Auditor = a.Auditor
                })
                .ToList()
            })
            
            .ToListAsync(ct);
            return TypedResults.Ok(farms);
        });

        app.MapGet("/farms/{id}", async Task<Results<Ok<FarmItem>, NotFound>> (int id, CertifyDbContext db, CancellationToken ct) =>
        {

            var farm = await db.Farms
                .Select( f =>new FarmItem
                {
                    Id= f.Id,
                    Name = f.Name,
                    Country = f.Country,
                    Audits = f.Audits.Select(a => new AuditItem
                    {
                        Level = a.Level,
                        Auditor = a.Auditor,
                        Score = a.Score
                    }).ToList()
                })
                .FirstOrDefaultAsync(f => f.Id == id, ct);

            if (farm is null )
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(farm);
        });

        app.MapPost("/farms", async (NewFarmItem farm, CertifyDbContext db) =>
        {
            var name = farm.Name.Trim();
            var country = farm.Country.Trim();

            // Does this farm exits?
            var exits =  await db.Farms.AnyAsync(x => x.Name == name && x.Country == country);

            if (!exits)
            {
                var newfarm = new Farm{
                    Name = name,
                    Country = country
                };

                db.Farms.Add(newfarm);
                await db.SaveChangesAsync();

                return Results.Created($"/farms/{newfarm.Id}", new FarmItem
                {
                    Id = newfarm.Id,
                    Country = newfarm.Country
                });

            }

            return Results.Conflict("Exists");


        });

        app.MapGet("/stats", (FarmStatsService stats) => stats.CountByLevel());
    }
}
