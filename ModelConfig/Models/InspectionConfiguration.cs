using System;

namespace ModelConfig.Models;

public class InspectionConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string InspectionType { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public InspectionConfiguration Clone()
    {
        return new InspectionConfiguration
        {
            Id = Guid.NewGuid(),
            Name = Name,
            InspectionType = InspectionType,
            Notes = Notes
        };
    }
}
