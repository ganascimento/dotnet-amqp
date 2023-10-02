using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class TeacherCommandRepository : ITeacherCommandRepository
{
    private readonly string _connectionString;

    public TeacherCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task CreateAsync(TeacherEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Subject", entity.Subject, DbType.String);
        parameters.Add("@PersonId", entity.PersonId, DbType.Int32);

        var query = @"
        INSERT INTO tb_teacher
        (
            Subject,
            PersonId
        )
        VALUES
        (
            @Subject,
            @PersonId
        );";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }

    public async Task UpdateAsync(TeacherEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);
        parameters.Add("@Subject", entity.Subject, DbType.String);

        var query = @"
        UPDATE  tb_teacher
        SET     Subject = @Subject
        WHERE   Id = @Id;";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteAsync(TeacherEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);

        var query = "DELETE FROM tb_teacher WHERE Id = @Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }
}