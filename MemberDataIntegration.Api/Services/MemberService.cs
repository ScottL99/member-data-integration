using MemberDataIntegration.Api.Clients;
using MemberDataIntegration.Api.Data;
using MemberDataIntegration.Api.Models;
using MemberDataIntegration.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MemberDataIntegration.Api.Services;

public class MemberService
{
    private readonly IMemberSourceClient _client;
    private readonly AppDbContext _dbContext;

    public MemberService(IMemberSourceClient client, AppDbContext dbContext)
    {
        _client = client;
        _dbContext = dbContext;
    }

    public async Task<List<SourceMemberDto>> GetAllMembersAsync()
    {
        return await _client.GetMembersAsync();
    }

    public async Task<MemberPressTestResultDto> TestMemberPressMeAsync()
    {
        return await _client.TestMeAsync();
    }

    public async Task<MemberPressTestResultDto> TestMemberPressMembersAsync()
    {
        return await _client.TestMembersAsync();
    }

    public async Task<List<Member>> GetMembersFromDatabaseAsync()
    {
        return await _dbContext.Members.ToListAsync();
    }

    public async Task<int> ImportMembersAsync()
    {
        var sourceMembers = await _client.GetMembersAsync();

        foreach (var sourceMember in sourceMembers)
        {
            var existingMember = await _dbContext.Members.FirstOrDefaultAsync(member =>
                member.Email == sourceMember.Email
            );

            if (existingMember == null)
            {
                var member = new Member
                {
                    Email = sourceMember.Email,
                    Phone = sourceMember.Phone,
                };

                ApplySourceData(member, sourceMember);

                _dbContext.Members.Add(member);
            }
            else
            {
                ApplySourceData(existingMember, sourceMember);
            }
        }

        await _dbContext.SaveChangesAsync();

        return sourceMembers.Count;
    }

    private static void ApplySourceData(Member member, SourceMemberDto sourceMember)
    {
        if (!string.IsNullOrWhiteSpace(sourceMember.Phone))
        {
            member.Phone = sourceMember.Phone;
        }

        if (sourceMember.Source == "MemberPress")
        {
            member.IsMemberPress = true;
            member.MemberPressName = sourceMember.Name;
        }
        else if (sourceMember.Source == "Mailchimp")
        {
            member.IsMailchimp = true;
            member.MailchimpName = sourceMember.Name;
        }
        else if (sourceMember.Source == "AwardForce")
        {
            member.IsAwardForce = true;
            member.AwardForceName = sourceMember.Name;
        }
    }
}
