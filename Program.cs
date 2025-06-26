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

/*  !!! IdentityUserAccessor i RedirectManager dodaj TUTAJ,
      bo UserManager jest ju¿ w kontenerze */
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
                          IdentityRevalidatingAuthenticationStateProvider>();

/* ----------- 4.  Inne serwisy ------------------------------------ */
builder.Services.AddScoped<IAppEmailSender, EmailSender>();

builder.Services.AddSingleton<ReservationState>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, ConsoleEmailSender>();



//builder.Services.AddSingleton<
// IEmailSender<ApplicationUser>,
// WlodCar.Components.Account.NoOpEmailSender<ApplicationUser>>();


/* ----------- 5.  Budujemy aplikacjê ------------------------------ */
var app = builder.Build();

/* Inicjalizacja danych/ ról / admina */
await DbInitializer.SeedAsync(app.Services);

/* ----------- 6.  Middleware -------------------------------------- */
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();                      // HSTS – patrz artyku³ MS Learn
}

app.UseHttpsRedirection();              // middleware przekierowuj¹cy HTTP -> HTTPS
app.UseStaticFiles();

app.UseRouting();

// GET /Account/Logout  – kasuje cookie i robi 302 /
app.MapGet("/Account/Logout",
    async (HttpContext http, SignInManager<ApplicationUser> signIn) =>
    {
        await signIn.SignOutAsync();              // usuwa cookie
        http.Response.Redirect("/");              // wróæ na Start
    });

app.UseAuthentication();                // konieczne po AddAuthentication
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