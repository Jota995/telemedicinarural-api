using Medicina.DTOs;
using Medicina.Models;
using Medicina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Xml.Linq;

namespace TelemedicinaRural.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorRepository doctorRepository;

        public DoctorController(DoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }


        [HttpGet("pulldata")]
        public async Task<IActionResult> PullData([FromQuery] DateTime? UpdatedAt = null, [FromQuery] int? Limit = null)
        {
            var data = await doctorRepository.Get(updatedAt: UpdatedAt, limit:Limit);

            var rxData = data.Select(x =>
                new RxDoctor()
                {
                    Id = x.Id.ToString(),
                    Nombre = x.Nombre,
                    FechaNacimiento = x.FechaNacimiento,
                    Genero = x.Genero,
                    Nacionalidad = x.Nacionalidad,
                    Especialidades = x.Especialidades,
                    IdsAgenda = x.IdsAgenda.Select(x => x.ToString()).ToList(),
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }
            );

            return Ok(new
            {
                documents = rxData,
                checkpoint = rxData.Select(x => x.UpdatedAt).Max()
            });
        }

        [HttpGet()]
        public async Task<List<Doctor>> Get()
        {
            return await doctorRepository.Get();
        }

        [HttpPost()]
        public async Task<ActionResult> Post(InsertDoctorDTO doctorDTO)
        {
            var doctor = new Doctor() 
            {
                Nombre = doctorDTO.Nombre,
                FechaNacimiento = doctorDTO.FechaNacimiento.HasValue ? doctorDTO.FechaNacimiento.Value : DateTime.Now,
                Genero = doctorDTO.Genero,
                EstadoCivil = doctorDTO.EstadoCivil,
                Nacionalidad = doctorDTO.Nacionalidad,
                Email = doctorDTO.Email,
                Especialidades = doctorDTO.Especialidades,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await doctorRepository.Insert(doctor);

            return Ok();
        }
    }
}
