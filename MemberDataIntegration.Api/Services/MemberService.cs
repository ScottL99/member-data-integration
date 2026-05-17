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
                    FirstName = sourceMember.FirstName,
                    LastName = sourceMember.LastName,
                    Phone = sourceMember.Phone,
                    IsMemberPress = sourceMember.Source == "MemberPress",
                };

                _dbContext.Members.Add(member);
            }
            else if (sourceMember.Source == "MemberPress")
            {
                existingMember.IsMemberPress = true;
            }
        }

        await _dbContext.SaveChangesAsync();

        return sourceMembers.Count;
    }
}
