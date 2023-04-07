using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> _usermanager;
        
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> usermanager, ITokenService tokenService, IMapper mapper)
        {
            _usermanager = usermanager;
            _mapper = mapper;
            _tokenService = tokenService;
            
        }

        [HttpPost("register")] //POST: api/account/register?username=xyz&password=pwd123
        //public async Task<ActionResult<AppUser>> Register(string username, string password)   //querystring gets automatically binded to function parameters
        
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is already taken!");
            
            var user = _mapper.Map<AppUser>(registerDto);

            
            
            
            user.UserName = registerDto.Username.ToLower();
         
            

            // _context.Users.Add(user);
            // await _context.SaveChangesAsync(); //to actually save those changes to DB
            var result = await _usermanager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _usermanager.AddToRoleAsync(user, "Member");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _usermanager.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x=> x.UserName == loginDto.UserName);

            if(user==null)
            {
                return Unauthorized("Invalid Username");
            }

            var result = await _usermanager.CheckPasswordAsync(user, loginDto.Password);

            if(!result) return Unauthorized("Invalid Password");


            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _usermanager.Users.AnyAsync(x => x.UserName == username);
        }
    }
}