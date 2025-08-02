using Carregamentos.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ApiDataService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:FlaskApiBaseUrl"]);
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
