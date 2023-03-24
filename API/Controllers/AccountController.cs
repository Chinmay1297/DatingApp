using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AppUser>> Register(string username, string password)   //querystring gets automatically binded to function parameters
        {
            using var hmac = new HMACSHA512(); //variables declared with using will be collected by garbage collector/ [dispose() will be called] once its of no use
            
            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); //to actually save those changes to DB

            return user;

        }
    }
}