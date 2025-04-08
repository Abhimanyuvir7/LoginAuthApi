using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAuthApi.Models;

public partial class User
{
    public int AutId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }

    [ForeignKey("autRoleId")]
    public int RoleId { get; set; }

    public string Email { get; set; }

    public bool? IsActive { get; set; }

    public virtual Role Role { get; set; }
}
