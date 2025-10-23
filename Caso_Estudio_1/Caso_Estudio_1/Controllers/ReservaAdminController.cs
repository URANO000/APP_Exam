using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Caso_Estudio_1.Controllers
{
    public class ReservaAdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReservaAdminController(ApplicationDbContext db) => _db = db;

        // GET: /ReservaAdmin/List
        // GET: /ReservaAdmin/List?idServicio=3
        [HttpGet]
        public IActionResult List(int? idServicio)
        {
            // Para el filtro (dropdown opcional en la vista)
            ViewBag.Servicios = _db.Servicios
                                   .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Nombre} (Id {s.Id})" })
                                   .OrderBy(s => s.Text)
                                   .ToList();

            var q = from c in _db.Citas
                    join s in _db.Servicios on c.IdServicio equals s.Id
                    select new CitaAdminDto
                    {
                        Id = c.Id,
                        NombreDeLaPersona = c.NombreDeLaPersona,
                        Telefono = c.Telefono,
                        Correo = c.Correo,
                        Identificacion = c.Identificacion,
                        MontoTotal = c.MontoTotal,
                        FechaNacimiento = c.FechaNacimiento,
                        FechaDeLaCita = c.FechaDeLaCita,
                        FechaDeRegistro = c.FechaDeRegistro,
                        IdServicio = c.IdServicio,
                        NombreServicio = s.Nombre,
                        Especialista = s.Especialista,
                        Clinica = s.Clinica,
                        EspecialidadEnProsa = s.Especialidad == 1 ? "Medicina general"
                                             : s.Especialidad == 2 ? "Imagenología"
                                             : s.Especialidad == 3 ? "Microbiología"
                                             : "Desconocida"
                    };

            if (idServicio.HasValue && idServicio.Value > 0)
                q = q.Where(x => x.IdServicio == idServicio.Value);

            var modelo = q.OrderByDescending(x => x.FechaDeRegistro).ToList();
            ViewBag.IdServicio = idServicio;
            return View(modelo);
        }
    }
}
