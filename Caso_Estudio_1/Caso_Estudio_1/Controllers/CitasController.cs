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
        public async Task<IActionResult> List(int? idServicio)
        {
            var citas = dbContext.Citas
                .Include(c => c.IdServicio);

            if (idServicio.HasValue)
            {
                ViewBag.ServicioId = idServicio.Value;
                var citasFiltradas = await citas
                    .Where(c => c.IdServicio == idServicio.Value)
                    .ToListAsync();
                return View(citasFiltradas);
            }

            var todasLasCitas = await citas.ToListAsync();
            return View(todasLasCitas);
        }

        //BUSCAR CITAS
        [HttpGet]
        public async Task<IActionResult> Buscar(int id)
        {
            var cita = await dbContext.Citas.FindAsync(id);

            if (cita == null)
            {
                TempData["Mensaje"] = "No se ha encontrado la cita, favor realice una!";
                return RedirectToAction("List");
            }

            return View("Detalles", cita);
        }

        //Detalles, this is gonna be usedby the search thingy
        [HttpGet]
        public IActionResult Detalles(Citas cita)
        {
            return View(cita);
        }


        //Finally the reservar stuff
        [HttpGet]
        public async Task<IActionResult> Reservar(int idServicio)
        {
            var servicio = await dbContext.Servicios.FindAsync(idServicio);

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

            //CÁLCULo

            decimal montoConIVA = (servicio.Monto * servicio.IVA) + servicio.Monto;

            var nuevaCita = new Citas
            {
                NombreDeLaPersona = viewModel.NombreDeLaPersona,
                Identificacion = viewModel.Identificacion,
                Telefono = viewModel.Telefono,
                Correo = viewModel.Correo,
                FechaNacimiento = viewModel.FechaNacimiento,
                Direccion = viewModel.Direccion,
                FechaDeLaCita = viewModel.FechaDeLaCita,
                FechaDeRegistro = DateTime.Now,
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
