using Medicina.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TelemedicinaRural.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
       

        /*public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("ListarUsuarios")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList(); // Recupera todos los usuarios de la base de datos
            var result = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email
            }).ToList();

            return Ok(result); // Retorna la lista de usuarios en formato JSON
        }

        [HttpGet("ListarPorId")]

        public async Task<IActionResult> GetUserById(Guid id)
        {
            // Buscar usuario por ID
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new { message = "User not found." }); // Retorna 404 si no se encuentra el usuario
            }

            // Retorna los detalles del usuario
            var result = new
            {
                user.Id,
                user.UserName,
                user.Email
            };

            return Ok(result); // Retorna el usuario en formato JSON
        }

        [HttpGet("ListarPorToken")]
        public async Task<IActionResult> GetUserByToken()
        {
            // Obtener el ID del usuario desde el token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Buscar usuario por ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Retornar detalles del usuario
            var result = new
            {
                user.Id,
                user.UserName,
                user.Email
            };

            return Ok(result); // Retorna el usuario en formato JSON
        }*/
    }
}
