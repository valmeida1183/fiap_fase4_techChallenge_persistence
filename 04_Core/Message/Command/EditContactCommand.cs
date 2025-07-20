using Core.Entity;

namespace Core.Message.Command;
public record EditContactCommand(int Id, string Name, string Phone, string Email, int DddId)
{
    public static implicit operator Contact(EditContactCommand command)
    {
        return new Contact
        {
            Id = command.Id,
            Name = command.Name,
            Phone = command.Phone,
            Email = command.Email,
            DddId = command.DddId
        };
    }
}
