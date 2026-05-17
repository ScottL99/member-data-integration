namespace MemberDataIntegration.Api.Models.Dtos;

public class SourceMemberDto
{
    public string Email { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string Source { get; set; } = string.Empty;
}
