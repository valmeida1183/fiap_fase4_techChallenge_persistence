using Application.Service.Interface;
using Consumer.Message.Command;
using Core.Entity;
using MassTransit;

namespace Consumer.Consumers;
public class CreateContactConsumer : IConsumer<CreateContactCommand>
{
    private readonly IContactService _contactService;
    private readonly ILogger<CreateContactCommand> _logger;

    public CreateContactConsumer(IContactService contactService, ILogger<CreateContactCommand> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateContactCommand> context)
    {
        var command = context.Message;

        if (command == null)
        {
            _logger.LogInformation("No contact was rteceived");
            
            return;
        }

        _logger.LogInformation("Received Contact: {Name}", command.Name);

        try
        {
            Contact contact = command;
            await _contactService.CreateAsync(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "01P05 - Error on creating Contact: {Name}", command.Name);
        }
    }
}
