using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class BlogAttachment
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int? BlogId { get; set; }

    public string BlogAttachmentContent { get; set; }

    public string ImageUrl { get; set; }

    public string VideoUrl { get; set; }

    public string MetaData { get; set; }

    public virtual Blog Blog { get; set; }
}
