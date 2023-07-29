namespace kvexplorer.shared.Models;

public class AuthenticatedUserClaims
{
    public string Username { get; set; }
    public string TenantId { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
}