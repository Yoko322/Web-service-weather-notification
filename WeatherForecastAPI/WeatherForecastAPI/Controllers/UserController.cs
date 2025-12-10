using Microsoft.AspNetCore.Mvc;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static List<UserInfo> _users = new();

        [HttpGet("info")]
        public ActionResult<IEnumerable<UserInfo>> GetAllUsers()
        {
            return Ok(_users);
        }

        [HttpPost("info")]
        public ActionResult<UserInfo> AddUser([FromBody] UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.Name) || string.IsNullOrEmpty(userInfo.Email))
            {
                return BadRequest("Name and Email are required");
            }

            userInfo.Id = _users.Count + 1;
            userInfo.RegistrationDate = DateTime.Now;
            _users.Add(userInfo);

            return CreatedAtAction(nameof(GetAllUsers), userInfo);
        }

        [HttpGet("info/{id}")]
        public ActionResult<UserInfo> GetUserById(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            return Ok(user);
        }
    }
}