using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class MaterialGroupCategory
{
    public int Id { get; set; }

    public string Label { get; set; }

    public int? Orders { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<MaterialGroup> MaterialGroups { get; set; } = new List<MaterialGroup>();
}
