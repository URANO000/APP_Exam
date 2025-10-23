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
              
        public IActionResult Index()
        {
            var servicios = _data.ListarServicios();
            return View(servicios);
        }
                
        [HttpGet]
        public IActionResult Buscar() => View();

        
        [HttpPost]
        public IActionResult Buscar(int id)
        {
            var cita = _data.BuscarCita(id);
            if (cita == null)
            {
                TempData["Msg"] = "Estimado usuario, no se ha encontrado la cita, favor realice una.";
                return RedirectToAction(nameof(Index));
            }
            return View("Details", cita);
        }
                
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

        // PROCESA LA RESERVA
        [HttpPost]
        public IActionResult Reservar(AddCitaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _data.RegistrarCita(model);
            TempData["Msg"] = "Cita registrada correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
