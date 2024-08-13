using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Models.Security;
using MyFinances.WebApi.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _users;
        private readonly IMapper _mapper;

        public UserController(IUser user, IMapper mapper)
        {
            _users = user;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            IEnumerable<User> users = await _users.GetAllAsync();
            bool usersFound = users != null && users.Any();
            if (usersFound)
            {
                IEnumerable<UserDTO> usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);
                return Ok(usersDTO);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            User user = await _users.GetByIdAsync(id);
            if (user != null)
            {
                UserDTO userDTO = _mapper.Map<UserDTO>(user);
                return Ok(userDTO);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> SignUp([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                User userCreated = await _users.CreateAsync(user);
                if (userCreated != null)
                {
                    return new CreatedAtActionResult(
                        "GetById",
                        "User",
                        new { id = userCreated.Id },
                        userCreated
                    );
                }
                else
                {
                    return BadRequest("The email provided is already in use. Please enter a different email...");
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserAuth user)
        {
            if (ModelState.IsValid)
            {
                UserToken userToken = await _users.Login(user);

                if (userToken != null)
                {
                    return Ok(userToken);
                }
            }

            return NotFound("Incorrect password and/or email...");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Modify(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest($"The provided ID does not match the user ID to be modified.{System.Environment.NewLine}Please try again.");
            }

            User userModified = await _users.ModifyAsync(id, user);
            return Ok(userModified);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User user = await _users.DeleteAsync(id);
            if (user != null) return Ok(user);
            return NotFound("No user exists with the requested ID...");
        }
    }
}
