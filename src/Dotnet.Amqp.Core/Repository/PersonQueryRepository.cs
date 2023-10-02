using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class PersonQueryRepository : IPersonQueryRepository
{
    private readonly string _connectionString;

    public PersonQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task<PersonEntity?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id, DbType.Int32);

        var query = @"
        SELECT  person.Id,
                person.Name,
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
        FROM    tb_person AS person
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id
        WHERE   person.Id = @Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<PersonEntity, AddressEntity, PersonEntity>(
                query,
                (person, address) =>
                {
                    person.Address = address;
                    return person;
                },
                splitOn: "StreetName",
                param: parameters
            );

            return persons.FirstOrDefault();
        }
    }

    public async Task<List<PersonEntity>> GetAllAsync()
    {
        var query = @"
        SELECT  person.Id,
                person.Name,
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
        FROM    tb_person AS person
        INNER   JOIN
                tb_address AS address
        ON      person.AddressId = address.Id";

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var persons = await connection.QueryAsync<PersonEntity, AddressEntity, PersonEntity>(
                query,
                (person, address) =>
                {
                    person.Address = address;
                    return person;
                },
                splitOn: "StreetName"
            );
            return persons.ToList();
        }
    }
}
