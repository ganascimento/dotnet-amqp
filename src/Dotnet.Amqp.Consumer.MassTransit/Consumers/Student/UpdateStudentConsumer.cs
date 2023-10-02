using Dotnet.Amqp.Core.Dtos.Student;
using Dotnet.Amqp.Core.Interfaces;
using MassTransit;

namespace Dotnet.Amqp.Consumer.MassTransit.Consumers.Student;

public class UpdateStudentConsumer : IConsumer<UpdateStudentDto>
{
    private readonly IStudentQueryRepository _studentQueryRepository;
    private readonly IStudentCommandRepository _studentCommandRepository;

    public UpdateStudentConsumer(
        IStudentQueryRepository studentQueryRepository,
        IStudentCommandRepository studentCommandRepository)
    {
        _studentQueryRepository = studentQueryRepository;
        _studentCommandRepository = studentCommandRepository;
    }

    public async Task Consume(ConsumeContext<UpdateStudentDto> context)
    {
        Console.WriteLine("Start process Update Student!");

        var studentUpdate = context.Message;
        var student = await _studentQueryRepository.GetByIdAsync(studentUpdate.Id);
        if (student == null) return;

        student.SchoolDocument = studentUpdate.SchoolDocument;
        student.SchoolYear = studentUpdate.SchoolYear;

        await _studentCommandRepository.UpdateAsync(student);

        Console.WriteLine("End process Update Student!");
    }
}