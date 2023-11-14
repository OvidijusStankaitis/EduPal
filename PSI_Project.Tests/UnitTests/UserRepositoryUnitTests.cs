﻿using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class UserRepositoryUnitTests
{
    private readonly UserRepository _userRepository;
    private readonly DbContextOptions<EduPalDatabaseContext> _options;
    private readonly EduPalDatabaseContext _context;

    public UserRepositoryUnitTests()
    {
        _options = GetInMemoryDatabaseOptions("TestUserDB"); 
        _context = new EduPalDatabaseContext(_options); //should it be in using? 
        _userRepository = new UserRepository(_context);
        
        // adding one test element for a test DB 
        var user = new User("test@test.test", "testPassword", "testName", "testSurname");
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    private DbContextOptions<EduPalDatabaseContext> GetInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<EduPalDatabaseContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }
    
    [Fact]
    public void CheckUserRegister_UserDoesNotExist_ReturnsNotNull()
    {
        // Arrange
        var user = new UserCreationDTO("nonexistentTest@test.test", "nonexistentTestPassword", "nonexistentTestName", "nonexistentTestSurname");

        // Act
        var result = _userRepository.CheckUserRegister(user);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public void CheckUserRegister_UserExists_ReturnsNull()
    {
        // Arrange
        var userExisting = new UserCreationDTO("testName", "testSurname", "test@test.test", "testPassword");
        
        // Act
        var result = _userRepository.CheckUserRegister(userExisting);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GetUserByEmail_UserExists_ReturnsUser()
    {
        // Arrange
        var user = new User("test1@test.test", "testPassword", "testName", "testSurname");
        _context.Users.Add(user);
        _context.SaveChanges();

        // Act
        var result = _userRepository.GetUserByEmail("test1@test.test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test1@test.test", result.Email);
        Assert.Equal("testPassword", result.Password);
        Assert.Equal("testName", result.Name);
        Assert.Equal("testSurname", result.Surname);
        
    }

    [Fact]
    public void GetUserByEmail_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        
        // Act
        var result = _userRepository.GetUserByEmail("nonexistent@test.test");

        // Assert
        Assert.Null(result);
    }
}