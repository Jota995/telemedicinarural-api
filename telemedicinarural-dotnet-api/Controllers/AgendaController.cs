using Medicina.DTOs;
using Medicina.Models;
using Medicina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;

namespace TelemedicinaRural.Controllers
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

        [HttpGet("pulldata")]
        public async Task<IActionResult> PullData([FromQuery] DateTime? UpdatedAt = null, [FromQuery] int? Limit = null, [FromQuery] string? Estado = null)
        {

            var data = await agendaRepo.GetAll(updatedAt:UpdatedAt, limit:Limit, estado:Estado);

            var rxData = data
                .Select(x =>
                    new RxAgendaMedica()
                    {
                        Id = x.Id.ToString(),
                        IdDoctor = x.IdDoctor.ToString(),
                        Especialidad = x.Especialidad,
                        Fecha = x.Fecha.ToLocalTime(),
                        Estado = x.Estado,
                        CreatedAt = x.CreatedAt.ToUniversalTime(),
                        UpdatedAt = x.UpdatedAt.ToLocalTime(),
                    }
                ).ToList();

            return Ok(new
            {
                documents = rxData,
                checkpoint = rxData.Select(x => x.UpdatedAt).Max()
            });
        }

        [HttpPost("pushdata")]
        public async Task<List<RxAgendaMedica>> PushData([FromBody]List<RxAgendaMedica> rxData)
        {

            List<RxAgendaMedica> agendasNoProcesadas = new List<RxAgendaMedica>();


            foreach (var rxagenda in rxData)
            {
                try
                {
                    var agenda = new AgendaMedica()
                    {
                        Id = ObjectId.Parse(rxagenda.Id),
                        IdDoctor = ObjectId.Parse(rxagenda.IdDoctor),
                        Especialidad = rxagenda.Especialidad,
                        Fecha = rxagenda.Fecha,
                        Estado = rxagenda.Estado,
                        CreatedAt = rxagenda.CreatedAt,
                        UpdatedAt = rxagenda.UpdatedAt
                    };

                    var result = await agendaRepo.Insert(agenda);
                    var doctor = await doctorRepo.GetOne(agenda.IdDoctor.ToString());
                    await doctorRepo.InsertAgenda(doctor.Id.ToString(), result.Id.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    agendasNoProcesadas.Add(rxagenda);
                    continue;
                }
            }

            return agendasNoProcesadas;
        }


        [HttpGet("")]
        public async Task<List<AgendaMedica>> Get()
        {
            var results = await agendaRepo.GetAll();

            return results;
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
