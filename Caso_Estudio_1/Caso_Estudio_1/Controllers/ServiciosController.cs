using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Caso_Estudio_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            return RedirectToAction("List, Categorias");
        }
    }
}
