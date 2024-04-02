var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Aspirations_ApiService>("apiservice");

builder.AddProject<Projects.Aspirations_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
