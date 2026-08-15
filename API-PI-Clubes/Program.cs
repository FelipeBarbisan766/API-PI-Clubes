using API_PI_Clubes.Application;
using API_PI_Clubes.Application.DependencyInjection;
using API_PI_Clubes.Application.Email;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Hubs;
using API_PI_Clubes.Infrastructure.Jobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Serviços de Infraestrutura e Base ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// --- 2. Configurações de Storage (Azure Blob Storage) ---
var storageConnectionString = builder.Configuration.GetSection("AzureStorage:ConnectionString").Value
    ?? throw new InvalidOperationException("Azure Storage connection string not found.");

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(storageConnectionString);
});

// --- 3. Injeção de Dependência da Camada Application ---
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddTransient<TokenService>();

// Configurações de E-mail
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, EmailService>();


// --- 4. Configurações de Segurança (JWT & Cookies) ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Jwt settings not configured.");
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "clubera_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Lax;

        options.ExpireTimeSpan = TimeSpan.FromHours(jwtSettings.Expiration);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });;

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

// --- 5. Configuração de CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("CluberaPolicy", policy =>
    {
        policy.WithOrigins("https://www.clubera.dev", "https://clubera.dev", "https://api.clubera.dev", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,               
                maxRetryDelay: TimeSpan.FromSeconds(10), 
                errorNumbersToAdd: null);      
        }));


builder.Services.AddHostedService<SubscriptionExpiryJob>();

var app = builder.Build();
    
// --- 6. Pipeline de Execução ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Servir a pasta wwwroot/uploads via HTTP

// Aplicação automática de Migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }
var wwwrootPath = app.Environment.WebRootPath 
                  ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(wwwrootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(wwwrootPath),
    RequestPath = ""
});
app.MapHub<CourtAvailabilityHub>("api/hubs/court-availability");
app.UseCors("CluberaPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();