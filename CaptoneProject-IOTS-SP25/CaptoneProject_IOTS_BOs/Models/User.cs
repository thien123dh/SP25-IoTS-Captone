using System;
using System.Collections.Generic;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int IsActive { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
