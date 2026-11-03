using Certify.ApiService.Data;
using Certify.ApiService.Endpoints;
using Certify.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.AddSqlServerDbContext<CertifyDbContext>("certificationdb");

builder.Services.AddSingleton<FarmStatsService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CertifyDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialise(db);
}

app.MapGet("/", () => "Certify API is running.");

app.MapFarmEndpoints();

app.MapDefaultEndpoints();

app.Run();
