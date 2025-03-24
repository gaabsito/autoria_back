using GymAPI.DTOs;
using GymAPI.Models;
using GymAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedidaCorporalController : ControllerBase
    {
        private readonly IMedidaCorporalService _service;

        public MedidaCorporalController(IMedidaCorporalService service)
        {
            _service = service;
        }

        // GET: api/MedidaCorporal
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<MedidaCorporalDTO>>>> GetMedidas()
        {
            // Obtenemos el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<List<MedidaCorporalDTO>>
                {
                    Success = false,
                    Message = "Usuario no autenticado"
                });
            }

            var medidas = await _service.GetAllByUsuarioAsync(userId);
            
            var medidasDTO = medidas.Select(m => new MedidaCorporalDTO
            {
                MedidaID = m.MedidaID,
                UsuarioID = m.UsuarioID,
                FechaRegistro = m.FechaRegistro,
                Peso = m.Peso,
                Altura = m.Altura,
                IMC = m.Altura.HasValue ? (m.Peso / (m.Altura.Value / 100 * m.Altura.Value / 100)) : null
            }).ToList();

            return Ok(new ApiResponse<List<MedidaCorporalDTO>>
            {
                Success = true,
                Data = medidasDTO
            });
        }

        // GET: api/MedidaCorporal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MedidaCorporalDTO>>> GetMedida(int id)
        {
            var medida = await _service.GetByIdAsync(id);
            
            if (medida == null)
            {
                return NotFound(new ApiResponse<MedidaCorporalDTO>
                {
                    Success = false,
                    Message = "Medida no encontrada"
                });
            }

            // Verificar que la medida pertenezca al usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || medida.UsuarioID != userId)
            {
                return Forbid();
            }

            var medidaDTO = new MedidaCorporalDTO
            {
                MedidaID = medida.MedidaID,
                UsuarioID = medida.UsuarioID,
                FechaRegistro = medida.FechaRegistro,
                Peso = medida.Peso,
                Altura = medida.Altura,
                IMC = medida.Altura.HasValue ? (medida.Peso / (medida.Altura.Value / 100 * medida.Altura.Value / 100)) : null
            };

            return Ok(new ApiResponse<MedidaCorporalDTO>
            {
                Success = true,
                Data = medidaDTO
            });
        }

        // POST: api/MedidaCorporal
        [HttpPost]
        public async Task<ActionResult<ApiResponse<MedidaCorporalDTO>>> CreateMedida(MedidaCorporalCreateDTO dto)
        {
            // Obtenemos el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<MedidaCorporalDTO>
                {
                    Success = false,
                    Message = "Usuario no autenticado"
                });
            }

            var medida = new MedidaCorporal
            {
                UsuarioID = userId,
                FechaRegistro = DateTime.UtcNow,
                Peso = dto.Peso,
                Altura = dto.Altura
            };

            int id = await _service.AddAsync(medida);
            
            // Recargamos la medida para asegurarnos de tener todos los datos
            var createdMedida = await _service.GetByIdAsync(id);
            
            var medidaDTO = new MedidaCorporalDTO
            {
                MedidaID = createdMedida.MedidaID,
                UsuarioID = createdMedida.UsuarioID,
                FechaRegistro = createdMedida.FechaRegistro,
                Peso = createdMedida.Peso,
                Altura = createdMedida.Altura,
                IMC = createdMedida.Altura.HasValue ? (createdMedida.Peso / (createdMedida.Altura.Value / 100 * createdMedida.Altura.Value / 100)) : null
            };

            return Ok(new ApiResponse<MedidaCorporalDTO>
            {
                Success = true,
                Data = medidaDTO,
                Message = "Medida registrada correctamente"
            });
        }

        // PUT: api/MedidaCorporal/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateMedida(int id, MedidaCorporalUpdateDTO dto)
        {
            var medida = await _service.GetByIdAsync(id);
            
            if (medida == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Medida no encontrada"
                });
            }

            // Verificar que la medida pertenezca al usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || medida.UsuarioID != userId)
            {
                return Forbid();
            }

            // Actualizar solo los campos proporcionados
            if (dto.Peso.HasValue)
                medida.Peso = dto.Peso.Value;
            
            if (dto.Altura.HasValue)
                medida.Altura = dto.Altura;

            await _service.UpdateAsync(medida);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Medida actualizada correctamente"
            });
        }

        // DELETE: api/MedidaCorporal/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMedida(int id)
        {
            var medida = await _service.GetByIdAsync(id);
            
            if (medida == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Medida no encontrada"
                });
            }

            // Verificar que la medida pertenezca al usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || medida.UsuarioID != userId)
            {
                return Forbid();
            }

            await _service.DeleteAsync(id);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Medida eliminada correctamente"
            });
        }
        
        // GET: api/MedidaCorporal/progreso
        [HttpGet("progreso")]
        public async Task<ActionResult<ApiResponse<object>>> GetProgreso()
        {
            // Obtenemos el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Usuario no autenticado"
                });
            }

            var medidas = await _service.GetAllByUsuarioAsync(userId);
            
            if (medidas.Count < 2)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Se necesitan al menos dos medidas para calcular el progreso",
                    Data = new { suficientesDatos = false }
                });
            }

            // Ordenamos las medidas por fecha (más antigua a más reciente)
            var medidasOrdenadas = medidas.OrderBy(m => m.FechaRegistro).ToList();
            var primeraMediada = medidasOrdenadas.First();
            var ultimaMediada = medidasOrdenadas.Last();

            // Calculamos diferencias
            var diferenciaPeso = ultimaMediada.Peso - primeraMediada.Peso;

            // Calculamos IMC para ambas medidas si hay altura
            float? imcInicial = null;
            float? imcActual = null;
            float? diferenciaIMC = null;

            if (primeraMediada.Altura.HasValue)
            {
                imcInicial = primeraMediada.Peso / (primeraMediada.Altura.Value / 100 * primeraMediada.Altura.Value / 100);
            }
            
            if (ultimaMediada.Altura.HasValue)
            {
                imcActual = ultimaMediada.Peso / (ultimaMediada.Altura.Value / 100 * ultimaMediada.Altura.Value / 100);
            }
            
            if (imcInicial.HasValue && imcActual.HasValue)
            {
                diferenciaIMC = imcActual.Value - imcInicial.Value;
            }

            // Calculamos días transcurridos
            var diasTranscurridos = (ultimaMediada.FechaRegistro - primeraMediada.FechaRegistro).Days;

            // Calculamos progreso
            var resultado = new
            {
                suficientesDatos = true,
                fechaInicial = primeraMediada.FechaRegistro,
                fechaActual = ultimaMediada.FechaRegistro,
                diasTranscurridos,
                pesoInicial = primeraMediada.Peso,
                pesoActual = ultimaMediada.Peso,
                diferenciaPeso,
                porcentajeCambioPeso = (diferenciaPeso / primeraMediada.Peso) * 100,
                imcInicial,
                imcActual,
                diferenciaIMC,
                tendenciaPeso = medidasOrdenadas.Count > 2 ? CalcularTendencia(medidasOrdenadas.Select(m => m.Peso).ToList()) : null
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = resultado
            });
        }

        // Método auxiliar para calcular la tendencia (pendiente de la línea de regresión)
        private double? CalcularTendencia(List<float> valores)
        {
            int n = valores.Count;
            if (n < 3) return null;

            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumXX = 0;

            for (int i = 0; i < n; i++)
            {
                sumX += i;
                sumY += valores[i];
                sumXY += i * valores[i];
                sumXX += i * i;
            }

            // Calcular pendiente (m) de la línea de regresión lineal
            double m = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
            return m;
        }
        
        // GET: api/MedidaCorporal/datos-grafico
        [HttpGet("datos-grafico")]
        public async Task<ActionResult<ApiResponse<object>>> GetDatosGrafico()
        {
            // Obtenemos el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Usuario no autenticado"
                });
            }

            var medidas = await _service.GetAllByUsuarioAsync(userId);
            
            if (medidas.Count == 0)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "No hay datos para mostrar",
                    Data = new { 
                        fechas = new List<string>(),
                        pesos = new List<float>(),
                        imcs = new List<float?>()
                    }
                });
            }

            // Ordenamos las medidas por fecha (más antigua a más reciente)
            var medidasOrdenadas = medidas.OrderBy(m => m.FechaRegistro).ToList();
            
            // Preparamos los datos para el gráfico
            var fechas = medidasOrdenadas.Select(m => m.FechaRegistro.ToString("dd/MM/yyyy")).ToList();
            var pesos = medidasOrdenadas.Select(m => m.Peso).ToList();
            var imcs = medidasOrdenadas.Select(m => m.Altura.HasValue 
                ? (float?)(m.Peso / (m.Altura.Value / 100 * m.Altura.Value / 100)) 
                : null).ToList();

            var resultado = new
            {
                fechas,
                pesos,
                imcs
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = resultado
            });
        }

        // GET: api/MedidaCorporal/latest
        [HttpGet("latest")]
        public async Task<ActionResult<ApiResponse<MedidaCorporalDTO>>> GetLatestMedida()
        {
            // Obtenemos el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<MedidaCorporalDTO>
                {
                    Success = false,
                    Message = "Usuario no autenticado"
                });
            }

            var medida = await _service.GetLatestByUsuarioAsync(userId);
            
            if (medida == null)
            {
                return NotFound(new ApiResponse<MedidaCorporalDTO>
                {
                    Success = false,
                    Message = "No hay medidas registradas"
                });
            }

            var medidaDTO = new MedidaCorporalDTO
            {
                MedidaID = medida.MedidaID,
                UsuarioID = medida.UsuarioID,
                FechaRegistro = medida.FechaRegistro,
                Peso = medida.Peso,
                Altura = medida.Altura,
                IMC = medida.Altura.HasValue ? (medida.Peso / (medida.Altura.Value / 100 * medida.Altura.Value / 100)) : null
            };

            return Ok(new ApiResponse<MedidaCorporalDTO>
            {
                Success = true,
                Data = medidaDTO
            });
        }
    }
}