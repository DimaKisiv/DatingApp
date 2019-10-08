using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _repo;
        private readonly IConfiguration _config;

        public IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repo = repo;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserForRegister userParam)
        {
            userParam.username = userParam.username.ToLower();

            if (await _repo.UserExists(userParam.username))
                return BadRequest("User already exists");

            User newUserBeforeSave = _mapper.Map<User>(userParam);
            User newUserAfterSave = await _repo.Register(newUserBeforeSave, userParam.password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(newUserAfterSave);
            return CreatedAtRoute("GetUser", new { controller = "Users", id = userToReturn.ID },
             userToReturn);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserForRegister userParam)
        {
            User userFromDb = await _repo.Login(userParam.username.ToLower(), userParam.password);

            if (userFromDb == null)
                return Unauthorized();

            Claim[] claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromDb.ID.ToString()),
                new Claim(ClaimTypes.Name, userFromDb.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromDb);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}