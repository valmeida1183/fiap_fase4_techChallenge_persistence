using Application.Service.Interface;
using Core.Entity;
using Core.Message.Command;
using MassTransit;

namespace Consumer.Consumers;
public class DeleteContactConsumer : IConsumer<DeleteContactCommand>
{
    private readonly IContactService _contactService;
    private readonly ILogger<DeleteContactCommand> _logger;

    public DeleteContactConsumer(IContactService contactService, ILogger<DeleteContactCommand> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteContactCommand> context)
    {
        var command = context.Message;

        if (command == null)
        {
            _logger.LogInformation("No contact was received");

            return;
        }

        _logger.LogInformation("Received Contact: {Id}", command.Id);

        try
        {
            var contact = await _contactService.GetByIdAsync(command.Id);

            if (contact != null)
                await _contactService.DeleteAsync(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "01P05 - Error on Deleting Contact: {Id}", command.Id);
        }
    }
}
