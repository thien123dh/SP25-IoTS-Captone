using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Label { get; set; }

    [JsonIgnore]
    public int? Orders { get; set; }

    [JsonIgnore]
    public int IsActive { get; set; }

    [JsonIgnore]
    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();

    [JsonIgnore]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
