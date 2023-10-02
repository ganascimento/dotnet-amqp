using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface IPersonQueryRepository
{
    Task<PersonEntity?> GetByIdAsync(int id);
    Task<List<PersonEntity>> GetAllAsync();
}