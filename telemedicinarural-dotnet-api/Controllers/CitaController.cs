using brevo_csharp.Model;
using Medicina.DTOs;
using Medicina.Models;
using Medicina.Repository;
using Medicina.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace TelemedicinaRural.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CitaController : ControllerBase
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CitaRepository citaRepository;
        private readonly EmailService emailService;
        private readonly PacienteRepository pacienteRepository;
        private readonly DoctorRepository doctorRepository;

        public CitaController( UserManager<ApplicationUser> userManager, CitaRepository citaRepository, EmailService emailService,PacienteRepository pacienteRepository, DoctorRepository doctorRepository)
        {
           
            _userManager = userManager;
            this.citaRepository = citaRepository;
            this.emailService = emailService;
            this.pacienteRepository = pacienteRepository;
            this.doctorRepository = doctorRepository;
        }

        [HttpGet("pulldata")]
        public async Task<IActionResult> GetData([FromQuery] DateTime? UpdatedAt = null, [FromQuery] int? Limit = null)
        {
            try
            {
                var data = await citaRepository.Get(updatedAt: UpdatedAt, limit: Limit);

                var rxData = data.Select(x =>
                    new RxCita()
                    {
                        Id = x.Id.ToString(),
                        IdPaciente = x.IdPaciente.ToString(),
                        idDoctor = x.IdDoctor.ToString(),
                        Especialidad = x.Especialidad,
                        Fecha = x.FechaCita,
                        Estado = x.Estado,
                        Motivo = x.Motivo,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }
                ).ToList();

                return Ok(new
                {
                    documents = rxData,
                    checkpoint = rxData.Any() ? rxData?.Max(x => x.UpdatedAt) : null,
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("push")]
        public async Task<IActionResult> push([FromBody] List<RxCita> rxCitas)
        {
            var citas = rxCitas.Select(x => new Cita()
            {
                Id = ObjectId.Parse(x.Id),
                IdPaciente = ObjectId.Parse(x.IdPaciente),
                IdDoctor = ObjectId.Parse(x.idDoctor),
                Especialidad = x.Especialidad,
                FechaCita = x.Fecha,
                Estado = x.Estado,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToList();

            foreach (var cita in citas)
            {
                try
                {
                    var existeCita = await citaRepository.Get(IdCita: cita.Id.ToString());

                    if (!existeCita.Any())
                    {
                        await citaRepository.agendarCita(cita);

                        var paciente = await pacienteRepository.ObtenerPaciente(cita.IdPaciente.ToString());
                        var doctor = await doctorRepository.GetOne(cita.IdDoctor.ToString());

                        cita.paciente = paciente;
                        cita.doctor = doctor;

                        string fechaCita = cita.FechaCita.ToLocalTime().ToString("dd/MM/yyyy");
                        string horaCita = cita.FechaCita.ToLocalTime().ToString("HH:mm");
                        string cuerpoCorreoHtml = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif; color: #333;'>
                                <h2>Confirmación de Cita Médica</h2>
                                <p>Estimado/a {cita.paciente.Nombre},</p>

                                <p>Su cita médica ha sido confirmada. A continuación, encontrará los detalles de su cita:</p>
                
                                <table style='border-collapse: collapse; width: 100%; max-width: 600px;'>
                                    <tr>
                                        <td style='padding: 8px; border: 1px solid #ddd; font-weight: bold;'>Fecha de la cita:</td>
                                        <td style='padding: 8px; border: 1px solid #ddd;'>{fechaCita}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px; border: 1px solid #ddd; font-weight: bold;'>Hora de la cita:</td>
                                        <td style='padding: 8px; border: 1px solid #ddd;'>{horaCita}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px; border: 1px solid #ddd; font-weight: bold;'>Doctor:</td>
                                        <td style='padding: 8px; border: 1px solid #ddd;'>Dr./Dra. {cita.doctor.Nombre}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px; border: 1px solid #ddd; font-weight: bold;'>Especialidad:</td>
                                        <td style='padding: 8px; border: 1px solid #ddd;'>{cita.Especialidad}</td>
                                    </tr>
                                </table>

                                <p>Atentamente,</p>
                                <p><strong>Su equipo médico</strong></p>
                            </body>
                            </html>
                        ";
                        await emailService.EnviarCorreoConfirmacionCita(cita.paciente.Email, "Su cita medica ha sido confirmada", cuerpoCorreoHtml);
                    }
                    else
                    {
                        await citaRepository.Update(cita);
                    }

                    
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return Ok(Enumerable.Empty<string>());
        }

        // POST: api/cita
        [HttpPost ("AgendarCita")]
        public async Task<IActionResult> AgendarCita([FromBody] AgendarCitaDTO citaDTO)
        {
            var cita = new Cita()
            {
                IdPaciente = ObjectId.Parse(citaDTO.IdPaciente),
                IdDoctor = ObjectId.Parse(citaDTO.IdDoctor),
                FechaCita = citaDTO.Fecha,
                Especialidad = citaDTO.Especialidad,
                Motivo = citaDTO.Motivo,
                Estado = "Pendiente",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Inicio = null,
                Fin = null,
            };

            await citaRepository.agendarCita(cita);

            return Ok(cita);
        }

    
        /*
        // GET: api/cita/mis-citas
        [HttpGet("MisCitas")]
        public async Task<IActionResult> ObtenerMisCitas()
        {
            // Obtener el ID del usuario desde el token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            // Obtener las citas del usuario autenticado
            var citas = await _citasCollection.Find(c => c.IdPaciente == userId).ToListAsync();

            return Ok(citas);
        }

        [HttpGet("ListaDeCitas")]
        //[Authorize(Roles = "Administrador")]  // Solo los administradores pueden acceder
        public async Task<IActionResult> ObtenerTodasLasCitas()
        {
            // Obtener todas las citas en la base de datos
            var citas = await _citasCollection.Find(c => true).ToListAsync();
            var result = citas.Select(citas => new
            {
                citas.Motivo
            }).ToList();

            return Ok(result);
        }*/
    }
}


