using Auth0.AspNetCore.Authentication;
using ThAmCo.Staff.Services;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Configure ProductApiClient based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient("ProductsClient", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7068/");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

    builder.Services.AddScoped<IProductApiClient, ProductApiClient>();
}
else
{
    if (builder.Configuration.GetValue<bool>("WebServices:Products:UseFake", false))
    {
        builder.Services.AddTransient<IProductApiClient, ProductApiClientFake>();
    }
    else
    {
        builder.Services.AddHttpClient("ProductsClient", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["WebServices:Products:BaseAddress"]);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        builder.Services.AddScoped<IProductApiClient, ProductApiClient>();
    }
}

// Configure Auth0
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];

    var clientSecret = builder.Configuration["Auth0:ClientSecret"];
    if (string.IsNullOrEmpty(clientSecret))
    {
        throw new InvalidOperationException("Auth0 Client Secret is not configured. Please check your configuration.");
    }
    options.ClientSecret = clientSecret;
    options.CallbackPath = "/callback";
})
.WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0:Audience"];
});

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

var app = builder.Build();

// Configure the HTTP error based on environment.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Polly resilience policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}