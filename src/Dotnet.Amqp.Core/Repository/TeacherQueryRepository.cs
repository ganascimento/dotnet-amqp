using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class TeacherQueryRepository : ITeacherQueryRepository
{
    private readonly string _connectionString;

    public TeacherQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task<TeacherEntity?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id, DbType.Int32);

        var query = @"
        SELECT  teacher.Id,
                teacher.Subject,
                person.Name,
                person.Id,
                person.BirthDate,
                person.Phone,
                person.Document,
                person.AddressId,
                address.StreetName,
                address.Id,
                address.ZipCode,
                address.Neighborhood,
                address.Number,
                address.State,
                address.Country
        FROM    tb_teacher AS teacher
        INNER   JOIN
                tb_person AS person
        ON      teacher.PersonId = person.Id
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id
        WHERE   teacher.Id = @Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<TeacherEntity, PersonEntity, AddressEntity, TeacherEntity>(
                query,
                (teacher, person, address) =>
                {
                    person.Address = address;
                    teacher.Person = person;
                    return teacher;
                },
                splitOn: "Name, StreetName",
                param: parameters
            );

            return persons.FirstOrDefault();
        }
    }

    public async Task<List<TeacherEntity>> GetAllAsync()
    {
        var query = @"
        SELECT  teacher.Id,
                teacher.Subject,
                person.Name,
                person.Id,
                person.BirthDate,
                person.Phone,
                person.Document,
                person.AddressId,
                address.StreetName,
                address.Id,
                address.ZipCode,
                address.Neighborhood,
                address.Number,
                address.State,
                address.Country
        FROM    tb_teacher AS teacher
        INNER   JOIN
                tb_person AS person
        ON      teacher.PersonId = person.Id
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<TeacherEntity, PersonEntity, AddressEntity, TeacherEntity>(
                query,
                (teacher, person, address) =>
                {
                    person.Address = address;
                    teacher.Person = person;
                    return teacher;
                },
                splitOn: "Name, StreetName"
            );

            return persons.ToList();
        }
    }
}