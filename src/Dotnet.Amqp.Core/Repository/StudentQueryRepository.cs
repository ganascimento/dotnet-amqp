using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class StudentQueryRepository : IStudentQueryRepository
{
    private readonly string _connectionString;

    public StudentQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task<StudentEntity?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id, DbType.Int32);

        var query = @"
        SELECT  student.Id,
                student.SchoolYear,
                student.SchoolDocument,
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
        FROM    tb_student AS student
        INNER   JOIN
                tb_person AS person
        ON      student.PersonId = person.Id
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id
        WHERE   student.Id = @Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<StudentEntity, PersonEntity, AddressEntity, StudentEntity>(
                query,
                (student, person, address) =>
                {
                    person.Address = address;
                    student.Person = person;
                    return student;
                },
                splitOn: "Name, StreetName",
                param: parameters
            );

            return persons.FirstOrDefault();
        }
    }

    public async Task<List<StudentEntity>> GetAllAsync()
    {
        var query = @"
        SELECT  student.Id,
                student.SchoolYear,
                student.SchoolDocument,
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
        FROM    tb_student AS student
        INNER   JOIN
                tb_person AS person
        ON      student.PersonId = person.Id
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<StudentEntity, PersonEntity, AddressEntity, StudentEntity>(
                query,
                (student, person, address) =>
                {
                    person.Address = address;
                    student.Person = person;
                    return student;
                },
                splitOn: "Name, StreetName"
            );

            return persons.ToList();
        }
    }
}