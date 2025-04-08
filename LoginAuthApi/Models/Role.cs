using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LoginAuthApi.Models;

public partial class Role
{
    [Key]
    public int AutRoleId { get; set; }

    public string RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
