using MC_GymMasterWebAPI.Controllers;
using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using MC_GymMasterWebAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


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
    }); ;

});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// Configure DbContext with SQL Server
builder.Services.AddDbContext<GymMasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GymConnection")));
builder.Services.AddScoped<IGymMasterService, GymMasterDBContext>();
// Configure CORS to allow requests from the Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // Update this if your Angular app is hosted on a different domain or port
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Only if you need to allow credentials (e.g., cookies)
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

/*// Use the configured CORS policy
app.UseCors("AllowAll");*/
app.UseCors("AllowAll");

// Authentication and Authorization middlewares
app.UseAuthentication(); // Ensure you have authentication services configured if you use this
app.UseAuthorization();

app.MapControllers();

app.Run();
