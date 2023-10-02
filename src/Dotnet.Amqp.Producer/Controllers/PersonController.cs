using Bogus;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Dotnet.Amqp.Producer.Bus.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Amqp.Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonQueryRepository _personQueryRepository;
    private readonly IBusRabbitService _busRabbitService;
    private readonly string _createPersonQueue;
    private readonly string _updatePersonQueue;
    private readonly string _removePersonQueue;

    public PersonController(
        IPersonQueryRepository personQueryRepository,
        IBusRabbitService busRabbitService,
        IConfiguration configuration)
    {
        _personQueryRepository = personQueryRepository;
        _busRabbitService = busRabbitService;
        _createPersonQueue = configuration["Queue:Person:Create"] ?? throw new InvalidOperationException("Queue not found!");
        _updatePersonQueue = configuration["Queue:Person:Update"] ?? throw new InvalidOperationException("Queue not found!");
        _removePersonQueue = configuration["Queue:Person:Remove"] ?? throw new InvalidOperationException("Queue not found!");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var result = await _personQueryRepository.GetByIdAsync(id);

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var result = await _personQueryRepository.GetAllAsync();

        if (result != null)
            return Ok(result);
        else
            return NoContent();
    }

    [HttpPost]
    public ActionResult Create()
    {
        var person = new Faker<PersonEntity>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.BirthDate, f => f.Date.Between(new DateTime(1950, 01, 01), new DateTime(2010, 12, 31)))
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.Document, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.Address, f => new AddressEntity
            {
                StreetName = f.Address.StreetName(),
                ZipCode = f.Address.ZipCode(),
                Neighborhood = f.Address.StreetName(),
                Number = f.Phone.PhoneNumber("####"),
                State = f.Address.State(),
                Country = f.Address.Country(),
            })
            .Generate();

        _busRabbitService.Publish(person, _createPersonQueue);

        return Ok();
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id)
    {
        var person = new Faker<PersonEntity>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.BirthDate, f => f.Date.Between(new DateTime(1950, 01, 01), new DateTime(2010, 12, 31)))
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.Document, f => f.Phone.PhoneNumber("###########"))
            .RuleFor(u => u.Address, f => new AddressEntity
            {
                StreetName = f.Address.StreetName(),
                ZipCode = f.Address.ZipCode(),
                Neighborhood = f.Address.StreetName(),
                Number = f.Phone.PhoneNumber("####"),
                State = f.Address.State(),
                Country = f.Address.Country(),
            })
            .Generate();
        person.Id = id;

        _busRabbitService.Publish(person, _updatePersonQueue);

        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult Remove(int id)
    {
        var person = new PersonEntity(id);
        _busRabbitService.Publish(person, _removePersonQueue);

        return Ok();
    }
}