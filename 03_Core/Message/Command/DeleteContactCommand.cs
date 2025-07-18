using Core.Entity;

namespace Core.Message.Command;
public record DeleteContactCommand(int Id)
{
    public static implicit operator Contact(DeleteContactCommand command)
    {
        return new Contact
        {
            Id = 0,
        };
    }
}
