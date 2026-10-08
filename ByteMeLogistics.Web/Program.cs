using ByteMeLogistics.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var apiUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? "https://localhost:7001/";

builder.Services.AddHttpClient<ApiService>(
    client =>
    {
        client.BaseAddress = new Uri(apiUrl);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
    "{controller=Home}/{action=Index}/{id?}");

app.Run();