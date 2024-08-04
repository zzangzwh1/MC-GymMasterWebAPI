using MC_GymMasterWebAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<GymMasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GymConnection")));

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

// Use the configured CORS policy
app.UseCors("AllowAll");

// Authentication and Authorization middlewares
app.UseAuthentication(); // Ensure you have authentication services configured if you use this
app.UseAuthorization();

app.MapControllers();

app.Run();
