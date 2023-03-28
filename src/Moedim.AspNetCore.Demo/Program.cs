using Hellang.Middleware.ProblemDetails;

using Moedim.Microservices.AspNetCore.Security.UserStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// configure swagger
builder.Services
    .AddSwagger(enableSwaggerVersionSupport: true)
    .ConfigureVersioning()
    .ConfigureSwaggerGen<Program>((options, sp) =>
    {
        options.AddBearerAuth();
        options.AddApiKeyQueryAuth();
        options.AddBasicHeaderAuth();
        options.AddApiKeyHeaderAuth();
    })
    .ConfigureUIOptions(options =>
    {
    })
    .ConfigureSwaggerXmlDocs<Program>();

var m = builder.AddMicroServices(options => options.ServiceName = "AppNameTest3")
                .AddDataProtection()
                .AddContainerSupport()
                .AddApplicationInsightsTelemetry()
                .AddProblemDetails(options => { });

builder.Services.AddDefaultJwtAuthentication<int, InMemoryUserStoreProvider<int>>();

builder.Services.AddApiKeyHeaderAuthentication<int, InMemoryUserStoreProvider<int>>(
    optoins =>
    {
        optoins.AuthenticationType = "apiKey";
        optoins.HeaderName = "apiKey";
    },
    sectionName: "UserStore");

builder.Services.AddApiKeyQueryStringAuthentication<int, InMemoryUserStoreProvider<int>>(
    optoins =>
    {
        optoins.AuthenticationType = "token";
        optoins.QueryStringName = "token";
    },
    sectionName: "UserStore");

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        m.Options.DefaultAllCorsAllowedPolicyName,
        builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// required for the proxy headers to be forwarded.
app.UseForwardedHeaders();

app.UseProblemDetails(); // Add the middleware

// enable or disable based on config flag
app.UseHttpsRedirect();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(m.Options.DefaultAllCorsAllowedPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Logger.LogInformation("App Started");

// https://localhost:7018/liveness
app.MapLivenessHealthCheck();

// https://localhost:7018/healthy
app.MapHealthyHealthCheck();

app.UseMoedimLogging();

var group = app.MapGroup("/version").WithOpenApi();

group.MapGet(
    "/version",
    () => typeof(RequestDelegateFactory)
                .Assembly.GetCustomAttributes(true)
                .OfType<System.Reflection.AssemblyInformationalVersionAttribute>().First().InformationalVersion);

await app.RunAsync();

public partial class Program
{
    // Expose the Program class for use with WebApplicationFactory<T>
}
