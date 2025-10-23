using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Caso_Estudio_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caso_Estudio_1.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CitasController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //;ISTAR CITAS
        [HttpGet]
        public async Task<IActionResult> List(int? searchId)
        {
            var citasQuery = dbContext.Citas.Include(c => c.Servicio).AsQueryable();

            if (searchId.HasValue)
            {
                citasQuery = citasQuery.Where(c => c.Id == searchId.Value);
                if (!citasQuery.Any())
                {
                    TempData["Mensaje"] = "No se ha encontrado la cita, favor realice una!";
                }
            }

            var citas = await citasQuery.ToListAsync();
            return View(citas);
        }


        // GET: /Citas/Buscar/5
        //[HttpGet("Citas/Buscar/{id}")]
        //public async Task<IActionResult> Buscar(int id)
        //{
        //    var cita = await dbContext.Citas.Include(c => c.Servicio).FirstOrDefaultAsync(c => c.Id == id);

        //    if (cita == null)
        //    {
        //        TempData["Mensaje"] = "No se ha encontrado la cita, favor realice una!";
        //        return RedirectToAction("List");
        //    }

        //    return View("Detalles", cita);
        //}

        // GET: /Citas/Detalles/5
        [HttpGet("Citas/Detalles/{id}")]
        public async Task<IActionResult> Detalles(int id)
        {
            var cita = await dbContext.Citas.Include(c => c.Servicio).FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                TempData["Mensaje"] = "Estimado usuario, no se ha encontrado la cita, favor realice una....";
                return RedirectToAction("List");
            }

            return View(cita);
        }


        //Finally the reservar stuff
        [HttpGet]
        public async Task<IActionResult> Reservar(int id)
        {
            var servicio = await dbContext.Servicios.FindAsync(id);

            if (servicio == null)
            {
                TempData["Mensaje"] = "No se encontró el servicio para reservar...";
                return RedirectToAction("List", "Servicios");
            }

            var viewModel = new AddCitaViewModel
            {
                IdServicio = servicio.Id,
                FechaDeLaCita = DateTime.Now.AddDays(1)
            };

            ViewBag.Servicio = servicio;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reservar(AddCitaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Recargar info del servicio si hay errores de validación
                ViewBag.Servicio = await dbContext.Servicios.FindAsync(viewModel.IdServicio);
                return View(viewModel);
            }

            var servicio = await dbContext.Servicios.FindAsync(viewModel.IdServicio);
            if (servicio == null)
            {
                TempData["Mensaje"] = "El servicio seleccionado no existe...";
                return RedirectToAction("List", "Servicios");
            }

            // Validate date ranges for SQL datetime
            DateTime fechaCita = viewModel.FechaDeLaCita < new DateTime(1753, 1, 1)
                                 ? new DateTime(1753, 1, 1)
                                 : viewModel.FechaDeLaCita;

            DateTime fechaRegistro = DateTime.Now;
            if (fechaRegistro < new DateTime(1753, 1, 1))
                fechaRegistro = new DateTime(1753, 1, 1);

            // Calculate total
            decimal montoConIVA = (servicio.Monto * servicio.IVA) + servicio.Monto;

            var nuevaCita = new Citas
            {
                NombreDeLaPersona = viewModel.NombreDeLaPersona,
                Identificacion = viewModel.Identificacion,
                Telefono = viewModel.Telefono,
                Correo = viewModel.Correo,
                FechaNacimiento = viewModel.FechaNacimiento < new DateTime(1753, 1, 1)
                                  ? new DateTime(1753, 1, 1)
                                  : viewModel.FechaNacimiento,
                Direccion = viewModel.Direccion,
                FechaDeLaCita = fechaCita,
                FechaDeRegistro = fechaRegistro,
                IdServicio = viewModel.IdServicio,
                MontoTotal = montoConIVA
            };

            await dbContext.Citas.AddAsync(nuevaCita);
            await dbContext.SaveChangesAsync();

            TempData["Mensaje"] = "Cita registrada exitosamente!!!";
            return RedirectToAction("List");
        }





    }
}
