using System.Linq.Expressions;
using Moq;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class UserRepositoryTests
{
    [Fact]
    public async Task CreateAsync_UserWithSuchEmailAlreadyExists_ReturnsNull()
    {
        // Arrange
        var mockDbContext= new Mock<EduPalDatabaseContext>();
        var mockUserRepository = new Mock<UserRepository>(mockDbContext.Object);

        mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>
            {
                new ("existing-email@email.com", "password", "John", "Doe")
            });

        var userRepository = mockUserRepository.Object;
        var registerRequest = new RegisterRequest("Joanna", "Doe", "existing-email@email.com", "password");
        
        // Act 
        var result = await userRepository.CreateAsync(registerRequest);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task CreateAsync_UserWithSuchEmailDoesNotExist_ReturnsCreatedUser()
    {
        // Arrange
        var mockDbContext= new Mock<EduPalDatabaseContext>();
        var mockUserRepository = new Mock<UserRepository>(mockDbContext.Object);

        mockUserRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        var userRepository = mockUserRepository.Object;
        var registerRequest = new RegisterRequest("Joanna", "Doe", "not-existing-email@email.com", "password");
        
        // Act
        var result = await userRepository.CreateAsync(registerRequest);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(registerRequest.Name, result.Name);
        Assert.Equal(registerRequest.Surname, result.Surname);
        Assert.Equal(registerRequest.Email, result.Email);
        Assert.Equal(registerRequest.Password, result.Password);
    }
}