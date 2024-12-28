using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class UserRequestAttachment
{
    public int Id { get; set; }

    public int UserRequestId { get; set; }

    public string Title { get; set; }

    public string Contents { get; set; }

    public string ImageUrl { get; set; }

    public string MetaData { get; set; }

    public virtual UserRequest UserRequest { get; set; }
}
