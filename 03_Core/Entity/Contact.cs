using Core.Entity.Base;

namespace Core.Entity;
public class Contact : BaseEntity
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int DddId { get; set; }

    public virtual DirectDistanceDialing? Ddd { get; set; }
}
