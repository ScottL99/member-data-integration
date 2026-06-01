using System.ComponentModel.DataAnnotations;

namespace MemberDataIntegration.Api.Models;

public class Member
{
    public int Id { get; set; } // DB 用

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? MemberPressName { get; set; }
    public string? MailchimpName { get; set; }
    public string? AwardForceName { get; set; }

    public string? Phone { get; set; }

    public bool IsMemberPress { get; set; }
    public bool IsMailchimp { get; set; }
    public bool IsAwardForce { get; set; }
}
