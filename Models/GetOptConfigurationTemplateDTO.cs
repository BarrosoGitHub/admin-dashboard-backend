namespace OPTConfigurator.Models;

public class GetOptConfigurationTemplateDTO
{
    public required string StationId { get; set; }
    public required int WorkstationId { get; set; }
    public required string NetworkSegment { get; set; }
    public required string Company { get; set; }
    public required string Country { get; set; }
}
