namespace Dotnet.Amqp.Core.Dtos.Student;

public class UpdateStudentDto
{
    public int Id { get; set; }
    public int SchoolYear { get; set; }
    public string? SchoolDocument { get; set; }
}