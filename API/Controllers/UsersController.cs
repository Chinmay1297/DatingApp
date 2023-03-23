using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   //api/users
    public class UsersController : ControllerBase
    {
       
        private readonly DataContext _context;

        public UsersController(DataContext context) //Injecting Datacontext service to this controller
        {
            _context = context;
            
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() //ActionResult gives us the ability to return http responses (200 OK, bat request, etc)
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {
            var users = _context.Users.Find(id);
            return users;
        }
    }
}