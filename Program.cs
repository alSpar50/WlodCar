using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WlodCar.Components;
using WlodCar.Components.Account;
using WlodCar.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using WlodCar.Services;

var builder = WebApplication.CreateBuilder(args);

/* ----------- 1.  DB + Identity  ---------------------------------- */
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Brak connection stringa 'DefaultConnection'.");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
})
 .AddPasswordValidator<DomainPasswordValidator>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

/* ----------- 3.  Blazor + komponenty Identity -------------------- */
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
                          IdentityRevalidatingAuthenticationStateProvider>();

/* ----------- 4.  Inne serwisy ------------------------------------ */
builder.Services.AddScoped<IAppEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>(); // NOWY SERWIS

builder.Services.AddSingleton<ReservationState>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, ConsoleEmailSender>();

// Dodaj obs³ugê BackgroundService dla przypomnieñ
builder.Services.AddHostedService<ReminderService>();

/* ----------- 5.  Budujemy aplikacjê ------------------------------ */
var app = builder.Build();

/* ----------- 6.  Middleware -------------------------------------- */
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapGet("/Account/Logout",
    async (HttpContext http, SignInManager<ApplicationUser> signIn) =>
    {
        await signIn.SignOutAsync();
        http.Response.Redirect("/");
    });

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAsync(db);
}



app.Run();