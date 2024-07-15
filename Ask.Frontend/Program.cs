using Ask.Frontend.Components;
using Ask.Frontend.Identity;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie();

var connectionString = builder.Configuration.GetConnectionString("ApiConnection") 
            ?? throw new InvalidOperationException("No connection string found for the API");
builder.Services.AddHttpClient("Auth", options =>
    options.BaseAddress = new Uri(connectionString));

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.Run();
