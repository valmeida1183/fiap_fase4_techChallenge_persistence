using Application.Service.Interface;
using Core.Entity;
using Core.Message.Command;
using MassTransit;

namespace Consumer.Consumers;
public class EditContactConsumer : IConsumer<EditContactCommand>
{
    private readonly IContactService _contactService;
    private readonly ILogger<EditContactCommand> _logger;

    public EditContactConsumer(IContactService contactService, ILogger<EditContactCommand> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EditContactCommand> context)
    {
        var command = context.Message;

        if (command == null)
        {
            _logger.LogInformation("No contact was received");

            return;
        }

        _logger.LogInformation("Received Contact: {Name}", command.Name);
        
        try
        {
            Contact contact = command;
            await _contactService.EditAsync(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "01P05 - Error on editing Contact: {Name}", command.Name);
        }
    }
}
