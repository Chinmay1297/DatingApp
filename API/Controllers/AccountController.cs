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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")] //POST: api/account/register?username=xyz&password=pwd123
        //public async Task<ActionResult<AppUser>> Register(string username, string password)   //querystring gets automatically binded to function parameters
        
        public async Task<ActionResult<JsonObject>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is already taken!");

            using var hmac = new HMACSHA512(); //variables declared with using will be collected by garbage collector/ [dispose() will be called] once its of no use
            
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); //to actually save those changes to DB

            var obj = new JsonObject();
            obj["userid"] = user.Id;
            obj["username"] = user.UserName;
            return obj;

        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}