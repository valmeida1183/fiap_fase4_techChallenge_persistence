using Core.Message.Interface;
using Core.Entity;

namespace Core.Message.Command;
public record CreateContactCommand(string Name, string Phone, string Email, int DddId)
{
    public static implicit operator Contact(CreateContactCommand command) 
    {
       return new Contact 
       { 
           Id = 0,
           Name = command.Name, 
           Phone = command.Phone, 
           Email = command.Email, 
           DddId = command.DddId 
       }; 
    }
}

