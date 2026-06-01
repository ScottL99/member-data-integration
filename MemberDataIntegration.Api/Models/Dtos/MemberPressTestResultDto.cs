namespace MemberDataIntegration.Api.Models.Dtos;

public class MemberPressTestResultDto
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string BodyPreview { get; set; } = string.Empty;
}
