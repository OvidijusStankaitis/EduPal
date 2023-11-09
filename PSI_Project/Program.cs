using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Repositories;
using PSI_Project.Services;
using System.Text;
using PSI_Project.Hubs;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the IHttpClientFactory service
builder.Services.AddHttpClient();

// Add CORS services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.WithOrigins("https://localhost:44402") // Updated with your React app's URL
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<EduPalDatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration["DatabaseConnectionString"]);
});

builder.Services.AddSignalR();

// services dependency injections
builder.Services.AddTransient<GoalService>();
builder.Services.AddTransient<OpenAIService>();
builder.Services.AddTransient<NoteService>();
builder.Services.AddTransient<ChatService>();
builder.Services.AddTransient<NoteService>();

// repositories dependency injections
builder.Services.AddTransient<GoalsRepository>();
builder.Services.AddTransient<SubjectRepository>();
builder.Services.AddTransient<TopicRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<ConspectusRepository>();
builder.Services.AddTransient<ConspectusService>();
builder.Services.AddTransient<CommentRepository>();
builder.Services.AddTransient<OpenAIRepository>();
builder.Services.AddTransient<NoteRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use CORS middleware here after UseRouting and before UseEndpoints
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.MapHub<ChatHub>("/chat-hub");

app.Run();