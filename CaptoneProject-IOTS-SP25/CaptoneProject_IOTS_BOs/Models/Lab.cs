using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Lab
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<LabDetail> LabDetails { get; set; } = new List<LabDetail>();

    public virtual ICollection<MaterialGroup> MaterialGroups { get; set; } = new List<MaterialGroup>();
}
