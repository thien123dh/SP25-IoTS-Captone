using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class MaterialGroup
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? CategoryId { get; set; }

    public int? LabId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual MaterialGroupCategory Category { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual Lab Lab { get; set; }

    public virtual ICollection<MaterialGroupItem> MaterialGroupItems { get; set; } = new List<MaterialGroupItem>();

    public virtual User UpdatedByNavigation { get; set; }
}
