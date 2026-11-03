using Certify.ApiService.Data;
using Certify.ApiService.Domain;
using Certify.ApiService.Services;
using Microsoft.EntityFrameworkCore;

namespace Certify.ApiService.Endpoints;

public static class FarmEndpoints
{
    public static void MapFarmEndpoints(this WebApplication app)
    {
        app.MapGet("/farms", async (string? country, CertifyDbContext db) =>
        {
            var farms = await db.Farms.Include(f => f.Audits).ToListAsync();

            if (!string.IsNullOrEmpty(country))
            {
                farms = farms.Where(f => f.Country == country).ToList();
            }

            return Results.Ok(farms);
        });

        app.MapGet("/farms/{id}", async (int id, CertifyDbContext db) =>
        {
            var farm = await db.Farms
                .Include(f => f.Audits)
                .FirstOrDefaultAsync(f => f.Id == id);

            return Results.Ok(farm);
        });

        app.MapPost("/farms", async (Farm farm, CertifyDbContext db) =>
        {
            db.Farms.Add(farm);
            await db.SaveChangesAsync();

            return Results.Ok(farm);
        });

        app.MapGet("/stats", (FarmStatsService stats) => stats.CountByLevel());
    }
}
