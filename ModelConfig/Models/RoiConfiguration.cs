using System.Collections.Generic;

namespace ModelConfig.Models;

public class RoiConfiguration
{
    public string? SourceImagePath { get; set; }

    public List<RoiAnnotation> Rois { get; set; } = new();
}
