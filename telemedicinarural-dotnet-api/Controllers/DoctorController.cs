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
    public class DoctorController : ControllerBase
    {
        private readonly DoctorRepository doctorRepository;

        public DoctorController(DoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
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
