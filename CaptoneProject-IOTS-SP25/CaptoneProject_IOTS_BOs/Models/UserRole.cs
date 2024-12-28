using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public virtual Role Role { get; set; }

    public virtual User User { get; set; }
}
