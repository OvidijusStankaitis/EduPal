using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Repositories;
using PSI_Project.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PSI_Project.Hubs;
using PSI_Project.Middleware;
using PSI_Project.Repositories.For_tests;
using Serilog;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(logger);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the IHttpClientFactory service
builder.Services.AddHttpClient();

// Add CORS services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://localhost:44402")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddScoped<AuditingInterceptor>();

builder.Services.AddDbContext<EduPalDatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration["DatabaseConnectionString"]);
    var serviceProvider = builder.Services.BuildServiceProvider();
    var interceptor = serviceProvider.GetRequiredService<AuditingInterceptor>();
    options.AddInterceptors(interceptor);
});

builder.Services.AddSignalR();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options => options.Cookie.Name = "token")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? String.Empty)),
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    });

// services dependency injections
builder.Services.AddTransient<GoalService>();
builder.Services.AddTransient<OpenAIService>();
builder.Services.AddTransient<NoteService>();
builder.Services.AddTransient<ChatService>();
builder.Services.AddTransient<ConspectusService>();
builder.Services.AddTransient<IUserAuthService, UserAuthService>();
builder.Services.AddSingleton<PomodoroService>();

// repositories dependency injections
builder.Services.AddTransient<GoalsRepository>();
builder.Services.AddTransient<SubjectRepository>();
builder.Services.AddTransient<TopicRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<ConspectusRepository>();
builder.Services.AddTransient<CommentRepository>();
builder.Services.AddTransient<OpenAIRepository>();
builder.Services.AddTransient<NoteRepository>();

builder.Services.AddTransient<IFileOperations, FileOperations>(); // for tests

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // middleware usage

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.MapHub<ChatHub>("/chat-hub");

app.Run();