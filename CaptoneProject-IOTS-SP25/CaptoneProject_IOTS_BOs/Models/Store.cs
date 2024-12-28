using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int OwnerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string IsActive { get; set; }

    public virtual User Owner { get; set; }
}
