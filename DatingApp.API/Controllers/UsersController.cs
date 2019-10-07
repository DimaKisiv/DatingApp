using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(await _repo.GetUsers()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(_mapper.Map<UserForDetailedDto>(await _repo.GetUser(id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var userFromRepo = await _repo.GetUser(id);
                _mapper.Map(userForUpdateDto, userFromRepo);
                if (await _repo.SaveAll())
                return NoContent();

                throw new Exception($"Updating user {id} failed on save");
        }

    }
}