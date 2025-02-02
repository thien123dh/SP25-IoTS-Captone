using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class ProductRequest
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? ProductType { get; set; }

    public string? Remark { get; set; }

    public int? ActionBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public int? Status { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual ProductRequestStatus StatusNavigation { get; set; }

    public virtual User UpdatedByNavigation { get; set; }
}
