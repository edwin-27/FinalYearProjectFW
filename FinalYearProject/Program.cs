using FinalYearProject.Data;
using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using Microsoft.AspNetCore.Identity;
using FinalYearProject.Support;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // default Identity login path
    
});
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IConnectionSettingsValues>(sp =>
{
    var node = new Uri("https://localhost:9200");   // https://search-fasion-warehouse-66lj3bekwgujr5hdvobh374sh4.aos.eu-west-2.on.aws");
    var settings = new ConnectionSettings(node).BasicAuthentication("admin", "Edw1nJ0hn.01")
        .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true)
            .DefaultIndex("fashion-warehouse-index"); // default index

    return settings;
});

builder.Services.AddSingleton<IOpenSearchClient, OpenSearchClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedAdminUserAsync(services);
}

static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@fashionwarehouse.com";
    string adminPassword = "Admin123!";

    //  This Create Admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // THis creates an admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}


app.Run();
