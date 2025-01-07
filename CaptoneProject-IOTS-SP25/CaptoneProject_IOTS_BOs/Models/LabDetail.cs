using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class LabDetail
{
    public int Id { get; set; }

    public int? LabId { get; set; }

    public string Title { get; set; }

    public string LabContent { get; set; }

    public string ImageUrl { get; set; }

    public string VideoUrl { get; set; }

    public string MetaData { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Lab Lab { get; set; }
}
