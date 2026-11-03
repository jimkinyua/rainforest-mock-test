var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var certifyDb = sql.AddDatabase("certifydb");

var apiService = builder.AddProject<Projects.Certify_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(certifyDb)
    .WaitFor(certifyDb);

builder.AddProject<Projects.Certify_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
