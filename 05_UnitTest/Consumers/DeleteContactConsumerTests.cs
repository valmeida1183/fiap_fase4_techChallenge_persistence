using Application.Service.Interface;
using Consumer.Consumers;
using Core.Entity;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.Message.Command;
public class DeleteContactConsumerTests
{
    private readonly Mock<IContactService> _mockContactService;
    private readonly Mock<ILogger<DeleteContactCommand>> _mockLogger;
    private readonly DeleteContactConsumer _consumer;

    public DeleteContactConsumerTests()
    {
        _mockContactService = new Mock<IContactService>();
        _mockLogger = new Mock<ILogger<DeleteContactCommand>>();
        _consumer = new DeleteContactConsumer(_mockContactService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Consume_ValidCommand_CallsContactServiceDeletesync()
    {
        // Arrange
        var command = new DeleteContactCommand(1);
        var mockConsumeContext = new Mock<ConsumeContext<DeleteContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        _mockContactService.Setup(s => s.GetByIdAsync(command.Id))
            .ReturnsAsync(new Contact { 
                Id = command.Id, 
                Name = "Test User", 
                Phone = "99988-4567", 
                Email = "testuser@gmail.com", 
                DddId = 21 });

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockContactService.Verify(
            s => s.DeleteAsync(It.Is<Contact>(c => c.Id == command.Id)), Times.Once());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received Contact: 1")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task Consume_NullCommand_LogsInformationAndReturns()
    {
        // Arrange
        var mockConsumeContext = new Mock<ConsumeContext<DeleteContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns((DeleteContactCommand)null); // Simulate null message

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
        var command = new DeleteContactCommand(1);
        var mockConsumeContext = new Mock<ConsumeContext<DeleteContactCommand>>();
        mockConsumeContext.Setup(c => c.Message).Returns(command);

        var expectedException = new InvalidOperationException("Database error");
        _mockContactService.Setup(s => s.DeleteAsync(It.IsAny<Contact>())).ThrowsAsync(expectedException);
        _mockContactService.Setup(s => s.GetByIdAsync(command.Id))
            .ReturnsAsync(new Contact
            {
                Id = command.Id,
                Name = "Test User",
                Phone = "99988-4567",
                Email = "testuser@gmail.com",
                DddId = 21
            });

        // Act & Assert
        await _consumer.Consume(mockConsumeContext.Object);

        _mockContactService.Verify(
               s => s.DeleteAsync(It.IsAny<Contact>()),
               Times.Once());

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("01P05 - Error on deleting Contact: 1")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }
}
