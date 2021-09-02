using Microsoft.AspNetCore.ResponseCompression;
using SignalRTest.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


//https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0#bearer-token-authentication


builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

//https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0
//builder.Services.AddAuthentication(options =>
//{
//    // Identity made Cookie authentication the default.
//    // However, we want JWT Bearer Auth to be the default.
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//        .AddJwtBearer(options =>
//        {
//            // Configure the Authority to the expected value for your authentication provider
//            // This ensures the token is appropriately validated
//            // options.Authority = /* TODO: Insert Authority URL here */;

//            // We have to hook the OnMessageReceived event in order to
//            // allow the JWT authentication handler to read the access
//            // token from the query string when a WebSocket or 
//            // Server-Sent Events request comes in.

//            // Sending the access token in the query string is required due to
//            // a limitation in Browser APIs. We restrict it to only calls to the
//            // SignalR hub in this code.
//            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
//            // for more information about security considerations when using
//            // the query string to transmit the access token.
//            options.Events = new JwtBearerEvents
//            {
//                OnMessageReceived = context =>
//                {
//                    var accessToken = context.Request.Query["access_token"];

//                    // If the request is for our hub...
//                    var path = context.HttpContext.Request.Path;
//                    if (!string.IsNullOrEmpty(accessToken) &&
//                        (path.StartsWithSegments("/hubs/chat")))
//                    {
//                        // Read the token out of the query string
//                        context.Token = accessToken;
//                    }
//                    return Task.CompletedTask;
//                }
//            };
//        });




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

//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-5.0#websocket-origin-restriction
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
