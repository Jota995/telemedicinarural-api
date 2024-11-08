using Medicina.DTOs;
using Medicina.Models;
using Medicina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Medicina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteRepository pacienteRepository;

        public PacienteController(PacienteRepository pacienteRepository)
        {
            this.pacienteRepository = pacienteRepository;
        }

        [HttpPost("")]
        public async Task<ActionResult> Post(InsertPacienteDTO pacienteDTO)
        {
            var paciente = new Paciente()
            {
                Nombre = pacienteDTO.Nombre,
                Email = pacienteDTO.Email,
                FechaNacimiento = pacienteDTO.FechaNacimiento.HasValue ? pacienteDTO.FechaNacimiento.Value : DateTime.Now,
                Genero = pacienteDTO.Genero,
                Nacionalidad = pacienteDTO.Nacionalidad,
                EstadoCivil = pacienteDTO.EstadoCivil,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };

            await pacienteRepository.InsertarPaciente(paciente);

            return Ok(paciente);
        }
    }
}
