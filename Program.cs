




using ZtracSignalrTest.Hubs;







var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();





//Add Signalr to register services required by signalr hubs

builder.Services.AddSignalR();







var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();







//configure signalr endpoints

app.MapHub<ServerHub>("/ServerHub");







app.Run();
