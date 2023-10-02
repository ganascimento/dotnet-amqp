using System.Data;
using Dapper;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Repository;

public class PersonCommandRepository : IPersonCommandRepository
{
    private readonly string _connectionString;

    public PersonCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public async Task CreateAsync(PersonEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Name", entity.Name, DbType.String);
        parameters.Add("@BirthDate", entity.BirthDate, DbType.Date);
        parameters.Add("@Phone", entity.Phone, DbType.String);
        parameters.Add("@Document", entity.Document, DbType.String);
        parameters.Add("@StreetName", entity.Address?.StreetName, DbType.String);
        parameters.Add("@ZipCode", entity.Address?.ZipCode, DbType.String);
        parameters.Add("@Neighborhood", entity.Address?.Neighborhood, DbType.String);
        parameters.Add("@Number", entity.Address?.Number, DbType.String);
        parameters.Add("@State", entity.Address?.State, DbType.String);
        parameters.Add("@Country", entity.Address?.Country, DbType.String);

        var query = @"
        INSERT INTO tb_address
        (
            StreetName,
            ZipCode,
            Neighborhood,
            Number,
            State,
            Country
        )
        VALUES
        (
            @StreetName,
            @ZipCode,
            @Neighborhood,
            @Number,
            @State,
            @Country
        );
        
        INSERT INTO tb_person
        (
            Name,
            BirthDate,
            Phone,
            Document,
            AddressId
        )
        VALUES
        (
            @Name,
            @BirthDate,
            @Phone,
            @Document,
            LAST_INSERT_ID()
        );";


        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }
    }

    public async Task UpdateAsync(PersonEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);
        parameters.Add("@Name", entity.Name, DbType.String);
        parameters.Add("@BirthDate", entity.BirthDate, DbType.Date);
        parameters.Add("@Phone", entity.Phone, DbType.String);
        parameters.Add("@Document", entity.Document, DbType.String);

        parameters.Add("@AddressId", entity.AddressId, DbType.Int32);
        parameters.Add("@StreetName", entity.Address?.StreetName, DbType.String);
        parameters.Add("@ZipCode", entity.Address?.ZipCode, DbType.String);
        parameters.Add("@Neighborhood", entity.Address?.Neighborhood, DbType.String);
        parameters.Add("@Number", entity.Address?.Number, DbType.String);
        parameters.Add("@State", entity.Address?.State, DbType.String);
        parameters.Add("@Country", entity.Address?.Country, DbType.String);

        var query = @"
        UPDATE  tb_address
        SET     StreetName = @StreetName,
                ZipCode = @ZipCode,
                Neighborhood = @Neighborhood,
                Number = @Number,
                State = @State,
                Country = @Country
        WHERE   Id = @AddressId;
        
        UPDATE  tb_person
        SET     Name = @Name,
                BirthDate = @BirthDate,
                Phone = @Phone,
                Document = @Document
        WHERE   AddressId = @AddressId;";


        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }

    }

    public async Task DeleteAsync(PersonEntity entity)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id, DbType.Int32);
        parameters.Add("@AddressId", entity.AddressId, DbType.Int32);

        var query = @"
        DELETE FROM tb_address WHERE Id = @AddressId;
        DELETE FROM tb_person WHERE Id = @Id;";


        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(query, parameters);
            await transaction.CommitAsync();
        }

    }
}