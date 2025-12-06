using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelConfig.Models;

public class CameraConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public List<InspectionConfiguration> Inspections { get; set; } = new();

    public CameraConfiguration Clone()
    {
        return new CameraConfiguration
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Location = Location,
            Inspections = Inspections.Select(inspection => inspection.Clone()).ToList()
        };
    }
}
