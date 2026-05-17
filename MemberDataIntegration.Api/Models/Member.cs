using System.ComponentModel.DataAnnotations;

namespace MemberDataIntegration.Api.Models;

public class Member
{
    public int Id { get; set; } // DB 用

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }

    public bool IsMemberPress { get; set; }
    public bool IsMailchimp { get; set; }
    public bool IsAwardForce { get; set; }
}