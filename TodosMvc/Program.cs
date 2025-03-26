using System.Text;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodosMvc.Models;
using TodosMvc.Services;
using TodosMvc.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var keyvaultUrl = builder.Configuration["Keyvault:KeyvaultURL"];

if (!string.IsNullOrEmpty(keyvaultUrl))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyvaultUrl), new DefaultAzureCredential());
}

void ConfigureAuthentication()
{
    var jwtConfig = builder.Configuration.GetSection("Jwt");
    var jwtKey = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new Exception("JWT Key is missing! Ensure Key Vault is configured correctly.");
    }

    var key = Encoding.UTF8.GetBytes(jwtKey);

    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.HttpContext.Request.Cookies["AuthToken"];
                return Task.CompletedTask;
            }
        };
    });
}

void ConfigureDatabase()
{
    var connectionString = builder.Environment.IsProduction()
        ? builder.Configuration["TodosDBConnection"]
        : builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Database connection string is missing!");
    }

    builder.Services.AddDbContext<TodosContext>(options =>
        options.UseSqlServer(connectionString));
}


ConfigureAuthentication();
ConfigureDatabase();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITodosService, TodosService>();

var app = builder.Build();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/login");
    }
});


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
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

