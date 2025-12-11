using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace ModelConfig.Models;

public enum RoiClassification
{
    Ok,
    Ng
}

public class RoiAnnotation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public RoiClassification Classification { get; set; } = RoiClassification.Ok;

    public RectangleF Bounds { get; set; }
}
