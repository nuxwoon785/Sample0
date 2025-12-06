using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelConfig.Models;

public class MasterConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string ModelCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<CameraConfiguration> Cameras { get; set; } = new();

    public MasterConfiguration CloneWithNewModel(string modelCode, string? name = null)
    {
        return new MasterConfiguration
        {
            Id = Guid.NewGuid(),
            Name = name ?? Name,
            ModelCode = modelCode,
            Description = Description,
            Cameras = Cameras.Select(camera => camera.Clone()).ToList()
        };
    }
}
