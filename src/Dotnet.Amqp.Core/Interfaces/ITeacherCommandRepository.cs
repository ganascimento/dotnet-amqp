using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface ITeacherCommandRepository
{
    Task CreateAsync(TeacherEntity entity);
    Task UpdateAsync(TeacherEntity entity);
    Task DeleteAsync(TeacherEntity entity);
}