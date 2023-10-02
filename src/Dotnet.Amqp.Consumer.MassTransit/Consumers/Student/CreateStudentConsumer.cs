using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using MassTransit;

namespace Dotnet.Amqp.Consumer.MassTransit.Consumers.Student;

public class CreateStudentConsumer : IConsumer<StudentEntity>
{
    private readonly IStudentCommandRepository _studentCommandRepository;

    public CreateStudentConsumer(IStudentCommandRepository studentCommandRepository)
    {
        _studentCommandRepository = studentCommandRepository;
    }

    public async Task Consume(ConsumeContext<StudentEntity> context)
    {
        Console.WriteLine("Start process Create Student!");

        await _studentCommandRepository.CreateAsync(context.Message);

        Console.WriteLine("End process Create Student!");
    }
}