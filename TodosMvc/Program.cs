using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodosMvc.Models;
using TodosMvc.Services.Interfaces;
using TodosMvc.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? "");

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( options =>
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

builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    var keyvaultUrl = builder.Configuration.GetSection("Keyvault:KeyVaultUrl");
    var keyvaultClient = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID", EnvironmentVariableTarget.User);
    var keyvaultClientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET", EnvironmentVariableTarget.User);
    var keyvaultDirectoryId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID", EnvironmentVariableTarget.User);

    var credential = new ClientSecretCredential(keyvaultDirectoryId, keyvaultClient, keyvaultClientSecret);

    var client = new SecretClient(new Uri(keyvaultUrl.Value!), credential);

    builder.Services.AddDbContext<TodosContext>(options =>
    options.UseSqlServer(client.GetSecret("TodosDBConnection").Value.Value.ToString()));
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<TodosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}   


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
