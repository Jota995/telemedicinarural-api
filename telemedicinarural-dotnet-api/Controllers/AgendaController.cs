using Medicina.DTOs;
using Medicina.Models;
using Medicina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Medicina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaController : ControllerBase
    {
        private readonly AgendaRepository agendaRepo;
        private readonly DoctorRepository doctorRepo;

        public AgendaController(AgendaRepository agenda,DoctorRepository doctor)
        {
            agendaRepo = agenda;
            doctorRepo = doctor;
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]AgendaInsertDTO agendaDTO)
        {
            var agenda = new AgendaMedica()
            {
                Fecha = agendaDTO.Fecha,
                Estado = agendaDTO.Estado,
                Especialidad = agendaDTO.Especialidad,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IdDoctor = ObjectId.Parse(agendaDTO.IdDoctor),
            };

            var result = await agendaRepo.Insert(agenda);

            var doctor = await doctorRepo.GetOne(agendaDTO.IdDoctor);

            await doctorRepo.InsertAgenda(doctor.Id.ToString(), result.Id.ToString());

            return NoContent();
        } 
    }
}
