using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvexplorer.shared.Models;

public class AuthenticatedUserClaims
{
    public required string Username { get; set; }
    public required string TenantId { get; set; }
    public required string Email { get; set; }
    public string? Name { get; set; }
}