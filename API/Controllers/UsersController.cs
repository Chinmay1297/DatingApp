using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]   //api/users
    [Authorize]
    public class UsersController : BaseAPIController  //since baseapiController has [ApiController] attribute
                                                      //we dont need to apply that here (same for [route])
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
       
        public UsersController(IUserRepository userRepository, IMapper mapper) 
        {
            _mapper = mapper;
            _userRepository = userRepository;
             
        }

        //[AllowAnonymous] //dont use this at controller level, cuz then you cant override it at api endpoint level and use authorize there
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() //ActionResult gives us the ability to return http responses (200 OK, bat request, etc)
        {
            // var users = await _userRepository.GetUsersAsync();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            // return Ok(usersToReturn);

            return Ok(await _userRepository.GetMembersAsync());
        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     return Ok( await _userRepository.GetUserByIdAsync(id));
            
        // }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }
    }
}