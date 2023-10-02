using Bogus;
using Dotnet.Amqp.Core.Dtos.Student;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Dotnet.Amqp.Producer.Bus.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Amqp.Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentQueryRepository _studentQueryRepository;
    private readonly IBusMassTransitService _busMassTransitService;
    private readonly IPublishMassTransitService<UpdateStudentDto> _publishUpdateStudent;
    private readonly IPublishMassTransitService<RemoveStudentDto> _publishRemoveStudent;
    private readonly string _createStudentQueue;

    public StudentController(
        IStudentQueryRepository studentQueryRepository,
        IBusMassTransitService busMassTransitService,
        IPublishMassTransitService<UpdateStudentDto> publishUpdateStudent,
        IPublishMassTransitService<RemoveStudentDto> publishRemoveStudent,
        IConfiguration configuration)
    {
        _studentQueryRepository = studentQueryRepository;
        _busMassTransitService = busMassTransitService;
        _publishUpdateStudent = publishUpdateStudent;
        _publishRemoveStudent = publishRemoveStudent;
        _createStudentQueue = configuration["Queue:Student:Create"] ?? throw new InvalidOperationException("Queue not found!");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var result = await _studentQueryRepository.GetByIdAsync(id);

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var result = await _studentQueryRepository.GetAllAsync();

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpPost("{personId}")]
    public ActionResult Create(int personId)
    {
        var student = new Faker<StudentEntity>()
            .RuleFor(u => u.SchoolDocument, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.SchoolYear, new Random().Next(1, 9))
            .Generate();

        student.PersonId = personId;

        _busMassTransitService.Send(student, _createStudentQueue);

        return Ok();
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id)
    {
        var student = new Faker<UpdateStudentDto>()
            .RuleFor(u => u.SchoolDocument, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.SchoolYear, new Random().Next(1, 9))
            .Generate();

        student.Id = id;

        _publishUpdateStudent.Publish(student);

        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var student = new RemoveStudentDto { Id = id };
        _publishRemoveStudent.Publish(student);

        return Ok();
    }
}