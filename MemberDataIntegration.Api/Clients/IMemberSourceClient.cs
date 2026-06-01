using MemberDataIntegration.Api.Models.Dtos;

namespace MemberDataIntegration.Api.Clients;

public interface IMemberSourceClient
{
    Task<List<SourceMemberDto>> GetMembersAsync();
    Task<MemberPressTestResultDto> TestMeAsync();
    Task<MemberPressTestResultDto> TestMembersAsync();
};
