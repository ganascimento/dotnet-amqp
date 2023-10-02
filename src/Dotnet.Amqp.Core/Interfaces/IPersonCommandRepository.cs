using Dotnet.Amqp.Core.Entities;

namespace Dotnet.Amqp.Core.Interfaces;

public interface IPersonCommandRepository
{
    Task CreateAsync(PersonEntity entity);
    Task UpdateAsync(PersonEntity entity);
    Task DeleteAsync(PersonEntity entity);
}