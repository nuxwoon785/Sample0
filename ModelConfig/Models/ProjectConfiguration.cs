using System.Collections.Generic;

namespace ModelConfig.Models;

public class ProjectConfiguration
{
    public List<MasterConfiguration> Masters { get; set; } = new();
}
