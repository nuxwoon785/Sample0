using System.Text.Json.Serialization;
using Avalonia;

namespace RoiManager.Models;

public class RoiEntry
{
    public string Name { get; set; } = string.Empty;

    public bool IsOk { get; set; }

    public Rect Region { get; set; }

    [JsonIgnore]
    public string Display => $"{Name} - {(IsOk ? "OK" : "NG")}";

    public string ToDisplayString()
    {
        return $"이름: {Name}\n판정: {(IsOk ? "OK" : "NG")}\n위치: X={Region.X:F0}, Y={Region.Y:F0}, W={Region.Width:F0}, H={Region.Height:F0}";
    }
}
