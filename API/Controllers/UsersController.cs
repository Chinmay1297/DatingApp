using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService _photoService;
       
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) 
        {
            _photoService = photoService;
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

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync()) 
            {
                //return _mapper.Map<PhotoDto>(photo);
                return CreatedAtAction(nameof(GetUser), 
                new {username = user.UserName}, 
                _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("This is already your profile picture");

            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting main photo");
        }
    }
}