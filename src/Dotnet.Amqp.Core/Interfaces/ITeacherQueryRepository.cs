using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface ITeacherQueryRepository
{
    Task<TeacherEntity?> GetByIdAsync(int id);
    Task<List<TeacherEntity>> GetAllAsync();
}