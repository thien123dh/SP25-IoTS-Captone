using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class UserRequest
{
    public int Id { get; set; }

    public string Email { get; set; }

    public int? RoleId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public string OtpCode { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public int Status { get; set; }

    public string? Remark { get; set; }

    public Role? Role { get; set; }

    public virtual UserRequestStatus? StatusNavigation { get; set; }

    public virtual ICollection<UserRequestAttachment>? UserRequestAttachments { get; set; } = new List<UserRequestAttachment>();
}
