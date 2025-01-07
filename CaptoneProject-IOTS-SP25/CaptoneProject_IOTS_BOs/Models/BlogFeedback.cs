using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class BlogFeedback
{
    public int Id { get; set; }

    public int? BlogId { get; set; }

    public string Comment { get; set; }

    public decimal? Rating { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Blog Blog { get; set; }

    public virtual User CreatedByNavigation { get; set; }
}
