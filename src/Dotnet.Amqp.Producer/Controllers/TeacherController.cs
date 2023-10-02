using Dotnet.Amqp.Core.Dtos.Teacher;
using Dotnet.Amqp.Core.Interfaces;
using Dotnet.Amqp.Producer.Bus.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Amqp.Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherQueryRepository _teacherQueryRepository;
    private readonly IKafkaService _kafkaService;
    private readonly string _createTeacherTopic;
    private readonly string _updateTeacherTopic;
    private readonly string _removeTeacherTopic;

    public TeacherController(
        ITeacherQueryRepository teacherQueryRepository,
        IKafkaService kafkaService,
        IConfiguration configuration)
    {
        _teacherQueryRepository = teacherQueryRepository;
        _kafkaService = kafkaService;
        _createTeacherTopic = configuration["Kafka:Topic:Teacher:create"] ?? throw new InvalidOperationException("Create topic not found!");
        _updateTeacherTopic = configuration["Kafka:Topic:Teacher:update"] ?? throw new InvalidOperationException("Update topic not found!");
        _removeTeacherTopic = configuration["Kafka:Topic:Teacher:remove"] ?? throw new InvalidOperationException("Remove topic not found!");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var result = await _teacherQueryRepository.GetByIdAsync(id);

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var result = await _teacherQueryRepository.GetAllAsync();

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpPost("{personId}")]
    public ActionResult Create(int personId)
    {
        var dto = new CreateTeacherDto
        {
            PersonId = personId,
            Subject = "Math"
        };

        _kafkaService.Produce(_createTeacherTopic, dto);

        return Ok();
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id)
    {
        var dto = new UpdateTeacherDto
        {
            Id = id,
            Subject = "Portuguese"
        };

        _kafkaService.Produce(_updateTeacherTopic, dto);

        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var dto = new RemoveTeacherDto
        {
            Id = id
        };

        _kafkaService.Produce(_removeTeacherTopic, dto);

        return Ok();
    }
}