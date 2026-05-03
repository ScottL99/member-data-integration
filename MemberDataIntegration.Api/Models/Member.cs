using System.ComponentModel.DataAnnotations;

namespace MemberDataIntegration.Api.Models;

public class Member
{
    public int Id { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public bool IsMemberPress { get; set; }
    public bool IsMailchimp { get; set; }
    public bool IsAwardForce { get; set; }
}