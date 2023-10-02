using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface IStudentQueryRepository
{
    Task<StudentEntity?> GetByIdAsync(int id);
    Task<List<StudentEntity>> GetAllAsync();
}