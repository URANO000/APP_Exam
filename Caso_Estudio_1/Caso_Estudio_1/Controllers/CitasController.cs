using Microsoft.AspNetCore.Mvc;
using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using System.Linq;

namespace Caso_Estudio_1.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitasData _data;

        public CitasController(CitasData data)
        {
            _data = data;
        }

        // GET: /Citas
        [HttpGet]
        public IActionResult Index()
        {
            var servicios = _data.ListarServicios();
            return View(servicios);
        }

        // POST: /Citas (Búsqueda en la misma vista)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int? id)
        {
            var servicios = _data.ListarServicios();

            if (!id.HasValue || id <= 0)
            {
                TempData["Msg"] = "Por favor, ingrese un código de cita válido.";
                return View(servicios);
            }

            var cita = _data.BuscarCita(id.Value);
            if (cita == null)
            {
                TempData["Msg"] = "Estimado usuario, no se ha encontrado la cita. Por favor, verifique el código.";
                return View(servicios);
            }

            // Pasamos la cita encontrada para mostrarla en la misma vista
            ViewBag.Cita = cita;
            return View(servicios);
        }

        // GET: /Citas/Reservar/{id}
        [HttpGet]
        public IActionResult Reservar(int id)
        {
            var servicio = _data.ListarServicios().FirstOrDefault(x => x.Id == id);
            if (servicio == null) return NotFound();

            var vm = new AddCitaViewModel
            {
                IdServicio = servicio.Id,
                NombreServicio = servicio.Nombre,
                Especialista = servicio.Especialista,
                EspecialidadTexto = servicio.EspecialidadTexto,
                Monto = servicio.Monto,
                IVA = servicio.IVA
            };

            return View(vm);
        }

        // POST: /Citas/Reservar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reservar(AddCitaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _data.RegistrarCita(model);
            TempData["Msg"] = "Cita registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Citas/Details/{id}
        [HttpGet]
        public IActionResult Details(int id)
        {
            var cita = _data.BuscarCita(id);
            if (cita == null)
                return NotFound();

            return View(cita);
        }
    }
}
