using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Caso_Estudio_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Caso_Estudio_1.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ServiciosController(ApplicationDbContext dbConext)
        {
            this.dbContext = dbConext;
            
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddServiciosViewModel viewModel)
        {
            var servicios = new Servicios
            {
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Monto = viewModel.Monto,
                IVA = viewModel.IVA,
                Especialidad = viewModel.Especialidad,
                Especialista = viewModel.Especialista,
                Clinica = viewModel.Clinica,
                FechaDeRegistro = DateTime.Now,
                Estado = viewModel.Estado

            };

            await dbContext.Servicios.AddAsync(servicios);
            //Save the changes
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List, Servicios");
        }

        //LIST
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var servicios = await dbContext.Servicios.ToListAsync();
            return View(servicios);
        }
        //EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var servicios = await dbContext.Servicios.FindAsync(id);
            return View(servicios);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Servicios viewModel)
        {
            var servicios = await dbContext.Servicios.FindAsync(viewModel.Id);

            //Checking if exists
            if(servicios != null)
            {
                servicios.Nombre = viewModel.Nombre;
                servicios.Descripcion = viewModel.Descripcion;
                servicios.Monto = viewModel.Monto;
                servicios.IVA = viewModel.IVA;
                servicios.Especialidad = viewModel.Especialidad;
                servicios.Especialista = viewModel.Especialista;
                servicios.Clinica = viewModel.Clinica;
                servicios.FechaDeRegistro = DateTime.Now;
                servicios.Estado = viewModel.Estado;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Servicios");
        }

        //DELETE
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicios = await dbContext.Servicios.FirstOrDefaultAsync(x => x.Id == id);
            return View(servicios);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicios= await dbContext.Servicios.FindAsync(id);

            if (servicios != null)
            {
                dbContext.Servicios.Remove(servicios);

                //Finally save changes to the db
                await dbContext.SaveChangesAsync();
            }

            //Now I can redirect to list!
            return RedirectToAction("List", "Servicios");
        }



    }
}
