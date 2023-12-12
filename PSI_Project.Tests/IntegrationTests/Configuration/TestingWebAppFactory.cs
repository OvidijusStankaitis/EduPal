using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Services;

namespace PSI_Project.Tests.IntegrationTests.Configuration;

internal class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ConfigureAuth(builder);
        
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<EduPalDatabaseContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<EduPalDatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryEduPalTest");
                options.UseInternalServiceProvider(serviceProvider);
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<EduPalDatabaseContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("=========Data base is not created===============");
                        Console.WriteLine(ex);
                        Console.WriteLine("=================================================");
                    }

                    // adding test data
                    User user1 = new User("test1@test.test", "testPassword1", "testName", "testSurname")
                    {
                        Id = "test-user-id-1"
                    };
                    appContext.Users.Add(user1);
                    
                    Subject subject1 = new Subject("testSubject1")
                    {
                        Id = "test-subject-id-1"
                    };
                    Subject subject2 = new Subject("testSubject2")
                    {
                        Id = "test-subject-id-2"
                    };
                    Subject subject3 = new Subject("testSubject3")
                    {
                        Id = "test-subject-id-3"
                    };
                    appContext.Subjects.Add(subject1);
                    appContext.Subjects.Add(subject2);
                    appContext.Subjects.Add(subject3);  

                    Topic topic1 = new Topic("testTopic1", subject2, KnowledgeLevel.Good);
                    Topic topic2 = new Topic("testTopic2", subject2);
                    Topic topic3 = new Topic("testTopic3", subject3, KnowledgeLevel.Average);
                    appContext.Topics.Add(topic1);
                    appContext.Topics.Add(topic2);
                    appContext.Topics.Add(topic3);

                    Conspectus conspectus1 = new Conspectus
                    {
                        Id = "id1", Name = "conspectus1.pdf", Path = "some/path/conspectus1.pdf", Topic = topic3,
                        Rating = -250
                    };
                    appContext.Conspectuses.Add(conspectus1);
                        
                    Note note1 = new Note("testName1", "testContent1");
                    Note note2 = new Note("testName2", "testContent2");
                    appContext.Notes.Add(note1);
                    appContext.Notes.Add(note2);

                    Goal goal1 = new Goal
                    {
                        Id = "test-goal-1",
                        GoalDate = DateTime.Now,
                        UserId = "test-user-id-1"
                    };
                    SubjectGoal subjectGoal1 = new SubjectGoal(subject1, goal1, 4.6);
                    appContext.Goals.Add(goal1);
                    appContext.SubjectGoal.Add(subjectGoal1);
                    
                    appContext.SaveChanges();
                }
            }
        });
    }

    private void ConfigureAuth(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IUserAuthService, TestUserAuthService>();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        });
    }
}