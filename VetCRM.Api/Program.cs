using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VetCRM.Api.Middleware;
using VetCRM.Api.Services;
using VetCRM.Api.Swagger;
using VetCRM.Modules.Appointments;
using VetCRM.Modules.Clients;
using VetCRM.Modules.Identity;
using VetCRM.Modules.Identity.Infrastructure;
using VetCRM.Modules.MedicalRecords;
using VetCRM.Modules.Notifications;
using VetCRM.Modules.Pets;
using VetCRM.Modules.Reports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddPetsModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddClientModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddMedicalRecordsModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddAppointmentsModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddIdentityModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddNotificationsModule(builder.Configuration.GetConnectionString("Default") ?? string.Empty);
builder.Services.AddReportsModule();
builder.Services.AddScoped<DevSeedService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var settings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerExamplesOperationFilter>();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
