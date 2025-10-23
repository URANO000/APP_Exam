using Microsoft.AspNetCore.Mvc;
using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using System;

namespace Caso_Estudio_1.Controllers
{
    // CRUD Administrativo de Citas (lista, crea, edita, elimina, detalles)
    public class CitasAdminController : Controller
    {
        private readonly CitasData _data;

        public CitasAdminController(CitasData data)
        {
            _data = data;
        }

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
        public IActionResult Create(AddCitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Servicios = _data.ListarServiciosBasico();
                return View(model);
            }

            try
            {
                _data.RegistrarCita(model);
                TempData["Msg"] = "Cita creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al crear la cita: " + ex.Message);
                ViewBag.Servicios = _data.ListarServiciosBasico();
                return View(model);
            }
        }

        // GET: /CitasAdmin/Edit/5
        public IActionResult Edit(int id)
        {
            var vm = _data.ObtenerCitaParaEditar(id);
            if (vm == null) return NotFound();
            ViewBag.Servicios = _data.ListarServiciosBasico();
            ViewData["IdCita"] = id; // para el hidden del form
            return View(vm);
        }

        // POST: /CitasAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AddCitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Servicios = _data.ListarServiciosBasico();
                ViewData["IdCita"] = id;
                return View(model);
            }

            try
            {
                _data.ActualizarCita(id, model);
                TempData["Msg"] = $"Cita #{id} actualizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar: " + ex.Message);
                ViewBag.Servicios = _data.ListarServiciosBasico();
                ViewData["IdCita"] = id;
                return View(model);
            }
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
            try
            {
                _data.EliminarCita(id);
                TempData["Msg"] = $"Cita #{id} eliminada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar: " + ex.Message);
                var d = _data.BuscarCita(id);
                if (d == null) return RedirectToAction(nameof(Index));
                return View("Delete", d);
            }
        }
    }
}
