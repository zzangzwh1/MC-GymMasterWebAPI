using MC_GymMasterWebAPI.Controllers;
using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.HubConfig;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using MC_GymMasterWebAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GymMaster",
        Version = "v1",
        Description = "Following API is for Gym Master",
        Contact = new OpenApiContact
        {
            Name = "Mike Wonhyuk ChO",
            Email = "zzangzwh1@gmail.com",

        },
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// Configure DbContext with SQL Server
builder.Services.AddDbContext<GymMasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GymConnection")));
builder.Services.AddScoped<IGymMasterService, MC_GymMasterWebAPI.Repository.GymMasterDBContext>();
// Configure CORS to allow requests from the Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") 
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); 
        });
});
// JWT Authentication setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
        ClockSkew = TimeSpan.Zero
    };

    // Enable JWT via query string for SignalR
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/shub") ))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
    
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*// Use the configured CORS policy
app.UseCors("AllowAll");*/
app.UseCors("AllowAll");

// Authentication and Authorization middlewares
app.UseAuthentication(); 
app.UseAuthorization();
app.MapHub<SHub>("/shub");
app.MapControllers();

app.Run();
