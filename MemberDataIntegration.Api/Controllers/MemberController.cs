using MemberDataIntegration.Api.Models;
using MemberDataIntegration.Api.Models.Dtos;
using MemberDataIntegration.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemberDataIntegration.Api.Controllers;

[ApiController]
[Route("members")]
public class MemberController : ControllerBase
{
    private readonly MemberService _service;

    public MemberController(MemberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SourceMemberDto>>> GetMembers()
    {
        var members = await _service.GetAllMembersAsync();

        return Ok(members);
    }

    [HttpGet("db")]
    public async Task<ActionResult<List<Member>>> GetMembersFromDatabase()
    {
        var members = await _service.GetMembersFromDatabaseAsync();

        return Ok(members);
    }

    [HttpPost("import")]
    public async Task<ActionResult<int>> ImportMembers()
    {
        var importedCount = await _service.ImportMembersAsync();

        return Ok(importedCount);
    }
}
