using Microsoft.EntityFrameworkCore;
using BookshopMVC.Data;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add API Controllers support
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bookshop API",
        Version = "v1",
        Description = "A comprehensive API for managing a bookshop with authentication, cart, orders, and book management",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Bookshop API Support",
            Email = "support@bookshop.com"
        }
    });

    // Add security definition for cookie authentication
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"Cookie-based authentication. 
                      Step 1: Use the /api/Auth/register or /api/Auth/login endpoint to get authenticated.
                      Step 2: The authentication will be stored in cookies automatically.
                      Step 3: You can then access protected endpoints.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Add security requirement
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Enable annotations for better documentation
    // c.EnableAnnotations();  // Requires Swashbuckle.AspNetCore.Annotations

    // Include XML comments if available (optional)
    // c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "BookshopMVC.xml"), true);
});

// Add Authentication & Authorization
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/Auth/login";
        options.LogoutPath = "/api/Auth/logout";
        options.AccessDeniedPath = "/api/Auth/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    // Admin-only policy
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Customer or Admin policy
    options.AddPolicy("CustomerOrAdmin", policy =>
        policy.RequireRole("Customer", "Admin"));
});

// Add Entity Framework
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                       builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add AutoMapper (needed by other controllers)
builder.Services.AddAutoMapper(typeof(Program));

// Configure Stripe (optional - you can also do this in the controller)
var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ??
                      builder.Configuration["Stripe:SecretKey"];
Stripe.StripeConfiguration.ApiKey = stripeSecretKey;

// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         // 🛡️ Enables circular reference protection by preserving object references
//         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

//         // 🎨 Optional: Makes returned JSON easier to read during dev
//         options.JsonSerializerOptions.WriteIndented = true;
//     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Enable Swagger only in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookshop API v1");
        c.RoutePrefix = "swagger"; // Available at /swagger
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map API Controllers
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed the database with test data (commented out - using manual SQL script instead)
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     await BookshopMVC.Data.DataSeeder.SeedAsync(context);
// }

app.Run();
