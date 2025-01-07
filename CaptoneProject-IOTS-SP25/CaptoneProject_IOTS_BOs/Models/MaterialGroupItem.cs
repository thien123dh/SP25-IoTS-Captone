using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class MaterialGroupItem
{
    public int Id { get; set; }

    public int? MaterialId { get; set; }

    public int? GroupId { get; set; }

    public virtual MaterialGroup Group { get; set; }

    public virtual Material Material { get; set; }
}
