using AutoMed_Backend;
using AutoMed_Backend.Models;
using AutoMed_Backend.Repositories;
using AutoMed_Backend.SecurityInfra;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

app.Run();
