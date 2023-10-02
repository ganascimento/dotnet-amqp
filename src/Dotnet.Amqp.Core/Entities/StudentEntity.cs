using Dotnet.Amqp.Core.Entities.Base;

namespace Dotnet.Amqp.Core.Entities;

public class StudentEntity : BaseEntity
{
    public StudentEntity()
    {
    }

    public StudentEntity(int id)
    {
        this.Id = id;
    }

    public int SchoolYear { get; set; }
    public string? SchoolDocument { get; set; }
    public int PersonId { get; set; }
    public virtual PersonEntity? Person { get; set; }
}