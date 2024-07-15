using Ask.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("db connection string wasn't found");
builder.Services.AddDbContext<ApplicationDbContext>((options) =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var clientOrigins = builder.Configuration.GetConnectionString("ClientOrigin")
                    ?? throw new InvalidOperationException("allowed origins for cors weren't found");
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder =>
            builder.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(clientOrigins)
                .AllowCredentials()));

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapControllers();

app.Run();
