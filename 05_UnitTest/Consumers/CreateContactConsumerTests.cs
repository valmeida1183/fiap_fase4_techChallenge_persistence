using Application.Service.Interface;
using Consumer.Consumers;
using Core.Entity;
using Core.Message.Command;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace UnitTest.Consumers;
public class CreateContactConsumerTests
{
    private readonly Mock<IContactService> _mockContactService;
    private readonly Mock<ILogger<CreateContactCommand>> _mockLogger;
    private readonly CreateContactConsumer _consumer;

    public CreateContactConsumerTests()
    {
        _mockContactService = new Mock<IContactService>();
        _mockLogger = new Mock<ILogger<CreateContactCommand>>();
        _consumer = new CreateContactConsumer(_mockContactService.Object, _mockLogger.Object);
    }


    [Fact]
    public async Task Consume_ValidCommand_CallsContactServiceCreateAsync()
    {
        // Arrange
        var command = new CreateContactCommand("Test User", "99988-4567", "testuser@gmail.com", 21);
        var mockConsumeContext = new Mock<ConsumeContext<CreateContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockContactService.Verify(
            s => s.CreateAsync(It.Is<Contact>(c =>
                c.Name == command.Name &&
                c.Phone == command.Phone &&
                c.Email == command.Email &&
                c.DddId == command.DddId)),
            Times.Once());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received Contact: Test User")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task Consume_NullCommand_LogsInformationAndReturns()
    {
        // Arrange
        var mockConsumeContext = new Mock<ConsumeContext<CreateContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns((CreateContactCommand)null); // Simulate null message

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockContactService.Verify(s => s.CreateAsync(It.IsAny<Contact>()), Times.Never());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No contact was received")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task Consume_ServiceThrowsException_LogsError()
    {
        // Arrange
        var command = new CreateContactCommand("Test User", "99988-4567", "testuser@gmail.com", 21);
        var mockConsumeContext = new Mock<ConsumeContext<CreateContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        var expectedException = new InvalidOperationException("Database error");
        _mockContactService.Setup(s => s.CreateAsync(It.IsAny<Contact>())).ThrowsAsync(expectedException);

        // Act & Assert
        await _consumer.Consume(mockConsumeContext.Object);

        _mockContactService.Verify(
               s => s.CreateAsync(It.IsAny<Contact>()),
               Times.Once());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("01P05 - Error on creating Contact: Test User")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());        
    }    
}
