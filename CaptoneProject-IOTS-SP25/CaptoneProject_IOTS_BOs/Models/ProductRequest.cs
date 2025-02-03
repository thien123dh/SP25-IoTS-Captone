using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class ProductRequest
{
    public int Id { get; set; }

    [Column("material_id")]
    [ForeignKey(nameof(Material))]
    public int? MaterialId { set; get; }

    [Column("material_group_id")]
    [ForeignKey(nameof(MaterialGroup))]
    public int? MaterialGroupId { set; get; }

    [Column("lab_id")]
    [ForeignKey(nameof(Lab))]
    public int? LabId { set; get; }
    public int? ProductId { get; set; }

    public int? ProductType { get; set; }

    public string? Remark { get; set; }

    public int? ActionBy { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    public int? UpdatedBy { get; set; }

    public int? Status { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual ProductRequestStatus StatusNavigation { get; set; }

    public virtual User UpdatedByNavigation { get; set; }

    public virtual Material MaterialNavigation { set; get; }

    public virtual MaterialGroup MaterialGroupNavigation { set; get; }

    public virtual Lab LabNavigation { set; get; }
}
