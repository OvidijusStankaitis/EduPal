using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PSI_Project;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Repositories;
using Xunit;
namespace PSI_Project.Tests;

public class UserRepositoryTests
{
    private DbContextOptions<EduPalDatabaseContext> GetInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<EduPalDatabaseContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }
    
    [Fact]
    public void CheckUserRegister_UserDoesNotExist_ReturnsTrue()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions("EduPal");
        using var context = new EduPalDatabaseContext(options);
        var userRepository = new UserRepository(context);

        var user = new User("test@example.com", "testPassword", "testName", "testSurname");

        // Act
        var result = userRepository.CheckUserRegister(user);

        // Assert
        Assert.True(result);
    }
}
