using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dotnet.Amqp.Core.Database;

public class DatabaseMigration
{
    private readonly string _connectionString;

    public DatabaseMigration(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionString not found!");
    }

    public void CreateDataBase(int attempt = 0)
    {
        var query = @"
        CREATE TABLE IF NOT EXISTS tb_address
        (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            StreetName VARCHAR(100) NOT NULL,
            ZipCode VARCHAR(15) NOT NULL,
            Neighborhood VARCHAR(100) NULL,
            Number VARCHAR(10) NULL,
            State VARCHAR(100) NOT NULL,
            Country VARCHAR(100) NOT NULL
        );

        CREATE TABLE IF NOT EXISTS tb_person
        (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(100) NOT NULL,
            BirthDate DATE,
            Phone VARCHAR(11) NOT NULL,
            Document VARCHAR(14) NOT NULL,
            AddressId INT NOT NULL,
            FOREIGN KEY (AddressId) REFERENCES tb_address(Id)
        );

        CREATE TABLE IF NOT EXISTS tb_student
        (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            SchoolYear INT NOT NULL,
            SchoolDocument VARCHAR(15) NOT NULL,
            PersonId INT NOT NULL,
            FOREIGN KEY (PersonId) REFERENCES tb_person(Id)
        );

        CREATE TABLE IF NOT EXISTS tb_teacher
        (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Subject VARCHAR(50) NOT NULL,
            PersonId INT NOT NULL,
            FOREIGN KEY (PersonId) REFERENCES tb_person(Id)
        );
        ";


        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Execute(query);
            }
        }
        catch (Exception ex)
        {
            if (attempt < 5)
            {
                Thread.Sleep(5000);
                this.CreateDataBase(attempt++);
            }

            throw new Exception($"Database error: {ex.Message} - [{_connectionString}]");
        }
    }
}