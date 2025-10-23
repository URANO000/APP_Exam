using Microsoft.AspNetCore.Mvc;
using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;

namespace Caso_Estudio_1.Controllers
{
    public class CitasAdminController : Controller
    {
        private readonly CitasData _data;
        public CitasAdminController(CitasData data) => _data = data;

        // GET: /CitasAdmin
        public IActionResult Index()
        {
            var citas = _data.ListarCitas();
            return View(citas);
        }

        // GET: /CitasAdmin/Details/5
        public IActionResult Details(int id)
        {
            var cita = _data.BuscarCita(id);
            if (cita == null) return NotFound();
            return View(cita);
        }

        // GET: /CitasAdmin/Create
        public IActionResult Create()
        {
            ViewBag.Servicios = _data.ListarServiciosBasico();
            return View(new AddCitaViewModel());
        }

        // POST: /CitasAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddCitaViewModel m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Servicios = _data.ListarServiciosBasico();
                return View(m);
            }
            _data.RegistrarCita(m);
            return RedirectToAction(nameof(Index));
        }

        // GET: /CitasAdmin/Edit/5
        public IActionResult Edit(int id)
        {
            var vm = _data.ObtenerCitaParaEditar(id);
            if (vm == null) return NotFound();
            ViewBag.Servicios = _data.ListarServiciosBasico();
            // guardamos el id en route/hidden mediante ViewData
            ViewData["IdCita"] = id;
            return View(vm);
        }

        // POST: /CitasAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AddCitaViewModel m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Servicios = _data.ListarServiciosBasico();
                ViewData["IdCita"] = id;
                return View(m);
            }
            _data.ActualizarCita(id, m);
            return RedirectToAction(nameof(Index));
        }

        // GET: /CitasAdmin/Delete/5
        public IActionResult Delete(int id)
        {
            var d = _data.BuscarCita(id);
            if (d == null) return NotFound();
            return View(d);
        }

        // POST: /CitasAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _data.EliminarCita(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
