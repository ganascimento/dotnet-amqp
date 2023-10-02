using Dotnet.Amqp.Core.Entities.Base;

namespace Dotnet.Amqp.Core.Entities;

public class AddressEntity : BaseEntity
{
    public required string StreetName { get; set; }
    public required string ZipCode { get; set; }
    public string? Neighborhood { get; set; }
    public string? Number { get; set; }
    public required string State { get; set; }
    public required string Country { get; set; }
}