using MemberDataIntegration.Api.Data;
using MemberDataIntegration.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemberDataIntegration.Api.Controllers;

[ApiController]
[Route("members")]
public class MemberController : ControllerBase
{
    private readonly AppDbContext _context;

    public MemberController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Member>>> GetMembers()
    {
        var members = await _context.Members.ToListAsync();

        return Ok(members);
    }
}