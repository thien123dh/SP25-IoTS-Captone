using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string Email { get; set; }

    public string Fullname { get; set; }

    public string? Password { get; set; } = "";

    public string? Phone { get; set; }

    public string? Address { get; set; }

    [Column("gender")]
    public int? Gender { set; get; } = 1;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    public int IsActive { get; set; }

    [MaxLength(1000)]
    [Column("image_url")]
    public string? ImageURL { set; get; }

    public virtual ICollection<Blog>? BlogCreatedByNavigations { get; set; } = new List<Blog>();

    public virtual ICollection<BlogFeedback>? BlogFeedbacks { get; set; } = new List<BlogFeedback>();

    public virtual ICollection<Blog>? BlogUpdatedByNavigations { get; set; } = new List<Blog>();

    public virtual ICollection<Material>? MaterialCreatedByNavigations { get; set; } = new List<Material>();

    public virtual ICollection<MaterialGroup>? MaterialGroupCreatedByNavigations { get; set; } = new List<MaterialGroup>();

    public virtual ICollection<MaterialGroup>? MaterialGroupUpdatedByNavigations { get; set; } = new List<MaterialGroup>();

    public virtual ICollection<Material>? MaterialUpdatedByNavigations { get; set; } = new List<Material>();

    public virtual ICollection<ProductRequest>? ProductRequestCreatedByNavigations { get; set; } = new List<ProductRequest>();

    public virtual ICollection<ProductRequest>? ProductRequestUpdatedByNavigations { get; set; } = new List<ProductRequest>();

    public virtual ICollection<Store>? Stores { get; set; } = new List<Store>();

    public virtual ICollection<UserRole>? UserRoles { get; set; } = new List<UserRole>();
}
