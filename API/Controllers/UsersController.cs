using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using AutoMapper;
using API.DTOs;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;

    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetMembersAsync();

        return Ok(users);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {   
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    // ASP.NET Core will automatically bind this JSON data to a MemberUpdateDto instance, althought the original json contains > 10 properties
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    // the claim (nameid) is specified at the time the token was created
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user == null) return NotFound();

        _mapper.Map(memberUpdateDto, user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }
}
