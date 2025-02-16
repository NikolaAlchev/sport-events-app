using System.Security.Claims;
using System.Text;
using Domain.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
var connectionString = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var connectionString2 = config.GetConnectionString("SecondAppConnection")
    ?? throw new InvalidOperationException("Connection string 'SecondAppConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<QApplicationDbContext>(options =>
    options.UseSqlServer(connectionString2));

builder.Services.AddDefaultIdentity<EventsAppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure CORS to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder => builder
        .WithOrigins("https://eventsappreact-haewcahuf3cqhwae.northeurope-01.azurewebsites.net")
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()); // Allow credentials (cookies)
});

// Configure Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JwtSettings:Issuer"],
        ValidAudience = config["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RoleClaimType = ClaimTypes.Role, // Ensure this matches the claim type used in token generation
        NameClaimType = ClaimTypes.NameIdentifier
    };

    // Add event logging for troubleshooting token validation
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check if the token is available in the Cookie header
            if (context.Request.Headers.ContainsKey("Cookie"))
            {
                // Extract the token from the Cookie header
                var cookieHeader = context.Request.Headers["Cookie"].ToString();
                var token = cookieHeader.Split("; ")
                    .FirstOrDefault(c => c.StartsWith("jwt="))?.Split("=")[1];

                // Set the token in context if found
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check if the token is available in the Cookie header
            if (context.Request.Headers.ContainsKey("Cookie"))
            {
                // Extract the token from the Cookie header
                var cookieHeader = context.Request.Headers["Cookie"].ToString();
                var token = cookieHeader.Split("; ")
                    .FirstOrDefault(c => c.StartsWith("jwt="))?.Split("=")[1];

                // Set the token in context if found
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
            }

            return Task.CompletedTask;
        }
    };
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Always in production
    options.Cookie.SameSite = SameSiteMode.None; //
    options.LoginPath = "api/User/Login"; // Adjust according to your login path
}); 


    

// Configure Authorization policy to require "User" role
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserRolePolicy", policy =>
        policy.RequireRole("User"));
});

// Configure Identity with roles and set up the default schema

// Register services and repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventsRepository, EventRepository>();
builder.Services.AddScoped<IEventUserRepository, EventUserRepository>();
builder.Services.AddTransient<IEventService, EventService>();

builder.Services.AddScoped<IQuestionAppRepository, QuestionAppRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowFrontend"); // Ensure CORS is set before Authentication/Authorization

// Seed roles and admin user if they don't exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<EventsAppUser>>();

    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@admin.com";
    string adminPassword = "Admin123!";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new EventsAppUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();  // Enable authorization

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();