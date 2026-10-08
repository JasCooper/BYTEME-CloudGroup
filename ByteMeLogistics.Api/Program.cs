using ByteMeLogistics.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendPolicy");

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await SeedData.InitialiseAsync(context);
}

app.Run();