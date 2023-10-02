using Dotnet.Amqp.Core.Entities.Base;

namespace Dotnet.Amqp.Core.Entities;

public class TeacherEntity : BaseEntity
{
    public TeacherEntity()
    {
    }

    public TeacherEntity(int id)
    {
        this.Id = id;
    }

    public string? Subject { get; set; }
    public int PersonId { get; set; }
    public virtual PersonEntity? Person { get; set; }
}