using Dotnet.Amqp.Core.Dtos.Student;
using Dotnet.Amqp.Core.Interfaces;
using MassTransit;

namespace Dotnet.Amqp.Consumer.MassTransit.Consumers.Student;

public class RemoveStudentConsumer : IConsumer<RemoveStudentDto>
{
    private readonly IStudentQueryRepository _studentQueryRepository;
    private readonly IStudentCommandRepository _studentCommandRepository;

    public RemoveStudentConsumer(
        IStudentQueryRepository studentQueryRepository,
        IStudentCommandRepository studentCommandRepository)
    {
        _studentQueryRepository = studentQueryRepository;
        _studentCommandRepository = studentCommandRepository;
    }

    public async Task Consume(ConsumeContext<RemoveStudentDto> context)
    {
        Console.WriteLine("Start process Remove Student!");

        var student = await _studentQueryRepository.GetByIdAsync(context.Message.Id);
        if (student == null) return;

        await _studentCommandRepository.DeleteAsync(student);

        Console.WriteLine("End process Remove Student!");
    }
}