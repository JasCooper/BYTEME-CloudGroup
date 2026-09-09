using BYTE_ME_CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton<MenuTableService>();

builder.Services.AddSingleton<StaffBlobService>();

builder.Build().Run();