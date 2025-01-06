using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class BlogsCategory
{
    public int Id { get; set; }

    public string Label { get; set; }

    public int? Orders { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
