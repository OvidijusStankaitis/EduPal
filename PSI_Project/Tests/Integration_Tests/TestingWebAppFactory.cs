using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Tests.Integration_Tests;

internal class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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

                    Subject subject1 = new Subject("testSubject1");
                    Subject subject2 = new Subject("testSubject2");
                    Subject subject3 = new Subject("testSubject3");
                    appContext.Subjects.Add(subject1);
                    appContext.Subjects.Add(subject2);
                    appContext.Subjects.Add(subject3);

                    User user1 = new User("test1@test.test", "testPassword1", "testName", "testSurname");
                    appContext.Users.Add(user1);
                    
                    appContext.SaveChanges();
                }
            }
        });
    }
}