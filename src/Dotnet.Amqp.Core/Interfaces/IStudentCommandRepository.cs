using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface IStudentCommandRepository
{
    Task CreateAsync(StudentEntity entity);
    Task UpdateAsync(StudentEntity entity);
    Task DeleteAsync(StudentEntity entity);
}