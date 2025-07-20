using Application.Service.Interface;
using Consumer.Consumers;
using Core.Entity;
using Core.Message.Command;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTest.Consumers;
public class DeleteContactConsumerTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Consumer_ReceivesCommand_CallsContactServiceDeleteAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockContactService = new Mock<IContactService>();

        mockContactService.Setup(s => s.EditAsync(It.IsAny<Contact>())).Returns(Task.CompletedTask);
        mockContactService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new Contact
            {
                Id = 1,
                Name = "Test User",
                Phone = "99988-4567",
                Email = "testuser@gmail.com",
                DddId = 21
            });

        services.AddSingleton(mockContactService.Object);

        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<DeleteContactConsumer>();

            // Configure an in-memory receive endpoint where the consumer will listen
            x.UsingInMemory((context, cfg) =>
            {
                // Or if you explicitly named your queue:
                cfg.ReceiveEndpoint("contact-queue", e =>
                {
                    e.ConfigureConsumer<DeleteContactConsumer>(context);
                });
            });
        });

        await using var provider = services.BuildServiceProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var command = new DeleteContactCommand(1);

        // Act
        // Publish the command to the in-memory bus.
        // This simulates the producer sending the message.
        await harness.Bus.Publish(command);

        // Assert
        // Verify that the consumer received the message
        Assert.True(await harness.Consumed.Any<DeleteContactCommand>());

        mockContactService.Verify(
                s => s.DeleteAsync(It.Is<Contact>(c => c.Id == command.Id)), Times.Once());

        // Stop the TestHarness
        await harness.Stop();
    }
}
