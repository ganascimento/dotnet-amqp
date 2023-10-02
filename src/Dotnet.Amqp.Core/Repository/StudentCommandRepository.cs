using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class StudentCommandRepository : IStudentCommandRepository
{
    private readonly string _connectionString;

    public StudentCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task CreateAsync(StudentEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@SchoolYear", entity.SchoolYear, DbType.Int32);
        parameters.Add("@SchoolDocument", entity.SchoolDocument, DbType.String);
        parameters.Add("@PersonId", entity.PersonId, DbType.Int32);

        var query = @"
        INSERT INTO tb_student
        (
            SchoolYear,
            SchoolDocument,
            PersonId
        )
        VALUES
        (
            @SchoolYear,
            @SchoolDocument,
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

    public async Task UpdateAsync(StudentEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);
        parameters.Add("@SchoolYear", entity.SchoolYear, DbType.Int32);
        parameters.Add("@SchoolDocument", entity.SchoolDocument, DbType.String);

        var query = @"
        UPDATE  tb_student
        SET     SchoolYear = @SchoolYear,
                SchoolDocument = @SchoolDocument
        WHERE   Id = @Id;";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteAsync(StudentEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);

        var query = "DELETE FROM tb_student WHERE Id = @Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }
}