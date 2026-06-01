namespace MemberDataIntegration.Api.Models.Dtos;

public class SourceMemberDto
{
    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string Source { get; set; } = string.Empty;
}
