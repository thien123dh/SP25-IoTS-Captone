using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class ActivityLog
{
    public int Id { get; set; }

    public int? EntityId { get; set; }

    public int? EntityType { get; set; }

    public string Title { get; set; }

    public string Contents { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string Metadata { get; set; }
}
