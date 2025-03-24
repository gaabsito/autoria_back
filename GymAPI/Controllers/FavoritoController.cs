using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GymAPI.Models;
using GymAPI.Services;
using GymAPI.DTOs;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación para todas las acciones
    public class FavoritoController : ControllerBase
    {
        private readonly IFavoritoService _service;
        private readonly IEntrenamientoService _entrenamientoService;

        public FavoritoController(IFavoritoService service, IEntrenamientoService entrenamientoService)
        {
            _service = service;
            _entrenamientoService = entrenamientoService;
        }

        // GET: api/Favorito
        [HttpGet]
        public async Task<ActionResult<List<FavoritoDTO>>> GetFavoritos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var favoritos = await _service.GetByUsuarioAsync(userId);
            var favoritosDTO = favoritos.Select(f => new FavoritoDTO
            {
                UsuarioID = f.UsuarioID,
                EntrenamientoID = f.EntrenamientoID,
                FechaAgregado = f.FechaAgregado,
                TituloEntrenamiento = f.Entrenamiento?.Titulo,
                ImagenEntrenamiento = f.Entrenamiento?.ImagenURL,
                DificultadEntrenamiento = f.Entrenamiento?.Dificultad
            }).ToList();

            return Ok(favoritosDTO);
        }

        // GET: api/Favorito/exists/5
        [HttpGet("exists/{entrenamientoId}")]
        public async Task<ActionResult<bool>> CheckFavorito(int entrenamientoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var exists = await _service.ExistsAsync(userId, entrenamientoId);
            return Ok(exists);
        }

        // POST: api/Favorito
        [HttpPost]
        public async Task<IActionResult> AddFavorito([FromBody] FavoritoCreateDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Verificar que el entrenamiento existe
            var entrenamiento = await _entrenamientoService.GetByIdAsync(dto.EntrenamientoID);
            if (entrenamiento == null)
            {
                return NotFound("Entrenamiento no encontrado");
            }

            // Verificar si ya está agregado a favoritos
            var exists = await _service.ExistsAsync(userId, dto.EntrenamientoID);
            if (exists)
            {
                return BadRequest("Este entrenamiento ya está en tus favoritos");
            }

            var favorito = new Favorito
            {
                UsuarioID = userId,
                EntrenamientoID = dto.EntrenamientoID,
                FechaAgregado = DateTime.UtcNow
            };

            await _service.AddAsync(favorito);
            return CreatedAtAction(nameof(CheckFavorito), new { entrenamientoId = dto.EntrenamientoID }, true);
        }

        // DELETE: api/Favorito/5
        [HttpDelete("{entrenamientoId}")]
        public async Task<IActionResult> RemoveFavorito(int entrenamientoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Verificar si existe en favoritos
            var exists = await _service.ExistsAsync(userId, entrenamientoId);
            if (!exists)
            {
                return NotFound("Este entrenamiento no está en tus favoritos");
            }

            await _service.RemoveAsync(userId, entrenamientoId);
            return NoContent();
        }
    }
}