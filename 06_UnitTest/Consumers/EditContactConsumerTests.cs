using Application.Service.Interface;
using Consumer.Consumers;
using Core.Entity;
using Core.Message.Command;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Consumers;
public class EditContactConsumerTests
{
    private readonly Mock<IContactService> _mockContactService;
    private readonly Mock<ILogger<EditContactCommand>> _mockLogger;
    private readonly EditContactConsumer _consumer;

    public EditContactConsumerTests()
    {
        _mockContactService = new Mock<IContactService>();
        _mockLogger = new Mock<ILogger<EditContactCommand>>();
        _consumer = new EditContactConsumer(_mockContactService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Consume_ValidCommand_CallsContactServiceEditAsync()
    {
        // Arrange
        var command = new EditContactCommand(1, "Test User", "99988-4567", "testuser@gmail.com", 21);
        var mockConsumeContext = new Mock<ConsumeContext<EditContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockContactService.Verify(
            s => s.EditAsync(It.Is<Contact>(c =>
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
        var mockConsumeContext = new Mock<ConsumeContext<EditContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns((EditContactCommand)null); // Simulate null message

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockContactService.Verify(s => s.EditAsync(It.IsAny<Contact>()), Times.Never());

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
        var command = new EditContactCommand(1, "Test User", "99988-4567", "testuser@gmail.com", 21);
        var mockConsumeContext = new Mock<ConsumeContext<EditContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        var expectedException = new InvalidOperationException("Database error");
        _mockContactService.Setup(s => s.EditAsync(It.IsAny<Contact>())).ThrowsAsync(expectedException);

        // Act & Assert
        await _consumer.Consume(mockConsumeContext.Object);

        _mockContactService.Verify(
               s => s.EditAsync(It.IsAny<Contact>()),
               Times.Once());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("01P05 - Error on editing Contact: Test User")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }
}
