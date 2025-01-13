using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 
using System.Text;
using Microsoft.EntityFrameworkCore; 
using Infrastructure.Database;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL DbContext
builder.Services.AddScoped<IUserReadRepository, UserReadRepository>(); // DI kaydý
builder.Services.AddScoped<IUserWriteRepository, UserWriteRepository>(); // DI kaydý
builder.Services.AddScoped<AuthService>(); // AuthService kaydý
// Add DbContext for writing
var writeConnection = builder.Configuration.GetConnectionString("WriteConnection");  
// Add DbContext for reading
var readConnection = builder.Configuration.GetConnectionString("ReadConnection");
 
builder.Services.AddApplicationServices(writeConnection, readConnection);

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
