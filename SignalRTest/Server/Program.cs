using Microsoft.AspNetCore.ResponseCompression;
using SignalRTest.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);


//https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0#bearer-token-authentication

// Add services to the container.
//builder.Services.AddSignalR();
builder.Services.AddSignalR(hubOptions =>
{
    //https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/handling-connection-lifetime-events
    hubOptions.EnableDetailedErrors = true;
    //hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
});
//builder.Services.AddSignalR(srConfig => srConfig.EnableDetailedErrors = true)
//        .AddAzureSignalR(azureConfig =>
//        {
//            azureConfig.ConnectionString = "your connection string";
//        });
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

//var webSocketOptions = new Microsoft.AspNetCore.Builder.WebSocketOptions()
//{
//    KeepAliveInterval = TimeSpan.FromSeconds(120),
//};
//webSocketOptions.AllowedOrigins.Add("https://client.com");
//webSocketOptions.AllowedOrigins.Add("https://www.client.com");

//app.UseWebSockets(webSocketOptions);

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    //endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapHub<ChatHub>("/chathub", options =>
    {
        //https://docs.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-2.1&tabs=dotnet
        //options.Transports = TransportType.All;
        options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
        options.TransportMaxBufferSize = 256000;
        options.ApplicationMaxBufferSize = 256000;
       
        //options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(10);
        //options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
    });
    endpoints.MapFallbackToFile("index.html");
});

//app.MapRazorPages();
//app.MapControllers();
//app.MapFallbackToFile("index.html");

app.Run();
