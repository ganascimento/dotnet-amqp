using Dotnet.Amqp.Core.Entities.Base;

namespace Dotnet.Amqp.Core.Entities;

public class PersonEntity : BaseEntity
{
    public PersonEntity()
    {
    }

    public PersonEntity(int id)
    {
        this.Id = id;
    }

    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string? Document { get; set; }
    public int AddressId { get; set; }
    public virtual AddressEntity? Address { get; set; }
}