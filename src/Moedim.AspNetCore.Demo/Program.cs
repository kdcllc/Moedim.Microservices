var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGenWithApiVersion<Program>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddBearer();
    options.AddTokenAuth();
});

_ = builder.AddMicroServices()
            .AddDataProtection()
            .AddContainerSupport()
            .AddApplicationInsightsTelemetry();

// add microsoft logger for azure log analytics store
// builder.Services.AddAzureLogAnalytics(builder.Configuration, configure: o => o.ApplicationName = $"AppNameTest{builder.Environment.EnvironmentName}", filter: (s, l) => true);

builder.AddSerilogLogging("AppNameTest2");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// required for the proxy headers to be forwared.
app.UseForwardedHeaders();

// enable or disable based on config flag
app.UseHttpsRedirect();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Logger.LogInformation("App Started");

app.MapGet(
    "/version",
    () => typeof(RequestDelegateFactory)
                .Assembly.GetCustomAttributes(true)
                .OfType<System.Reflection.AssemblyInformationalVersionAttribute>().First().InformationalVersion);

await app.RunAsync();
