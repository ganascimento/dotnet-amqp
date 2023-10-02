using Dotnet.Amqp.Core.Database;
using Dotnet.Amqp.Core.Interfaces;
using Dotnet.Amqp.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Amqp.Core.Configuration;

public static class ConfigureCore
{
    public static void InitializeCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPersonCommandRepository, PersonCommandRepository>();
        services.AddScoped<IPersonQueryRepository, PersonQueryRepository>();
        services.AddScoped<IStudentCommandRepository, StudentCommandRepository>();
        services.AddScoped<IStudentQueryRepository, StudentQueryRepository>();
        services.AddScoped<ITeacherCommandRepository, TeacherCommandRepository>();
        services.AddScoped<ITeacherQueryRepository, TeacherQueryRepository>();

        new DatabaseMigration(configuration)
            .CreateDataBase();
    }
}