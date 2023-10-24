using PSI_Project.Repositories;
using PSI_Project.Services;
using System.Text;

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

// Repositories and Services for DI
builder.Services.AddTransient<GoalService>();
builder.Services.AddTransient<GoalsRepository>();
builder.Services.AddTransient<SubjectRepository>();
builder.Services.AddTransient<TopicRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<ConspectusRepository>();
builder.Services.AddTransient<CommentRepository>();
builder.Services.AddTransient<OpenAIService>();
builder.Services.AddTransient<OpenAIRepository>();
builder.Services.AddTransient<NoteService>();


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

app.Run();