using BSUIR.Repositories.UnitOfWork;
using BSUIR.Survey.Foundation;
using BSUIR.Survey.Repositories;
using BSUIR.Survey.Repositories.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSurveyDbContext(builder.Configuration.GetConnectionString("default"));
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<IUnitOfWork, SurveyUnitOfWork>();
builder.Services.AddScoped<ISurveyUnitOfWork, SurveyUnitOfWork>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
