using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class MaterialCategory
{
    public int Id { get; set; }

    public string Label { get; set; }

    public int? Orders { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
