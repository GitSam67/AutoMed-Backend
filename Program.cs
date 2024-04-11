using AutoMed_Backend;
using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using AutoMed_Backend.SecurityInfra;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using AutoMed_Backend.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AutoMed"));
});      


builder.Services.AddDbContext<AppSecurityDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("AutoMedSecurity"));
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppSecurityDbContext>();

IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
await SuperAdminAssign.CreateApplicationAdministrator(serviceProvider);


builder.Services.AddScoped<SecurityManagement>();
builder.Services.AddScoped<AdminLogic>();
builder.Services.AddScoped<CustomerLogic>();


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        // Allowing any browser client to access the API
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

byte[] secretKey = Convert.FromBase64String(builder.Configuration["JWTCoreSettings:SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Header Requirements
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Use the JWT Bearer as a default scheme for the API so that the client MUST send the Bearer token in HTTP Header for Each Request
})
    // and validate the token for completing Authentication and Authorization
    .AddJwtBearer(token =>
    {
        // Check if the Https Metadata information is needed
        token.RequireHttpsMetadata = false;
        token.SaveToken = true; // Token will be maintained by the server
        token.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,  // Signature based verification
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };

    });


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JSON Web Token Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomException();

app.MapControllers();

app.Run();
