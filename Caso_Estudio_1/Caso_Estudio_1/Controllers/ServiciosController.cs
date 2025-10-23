using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpPost]
        //public async Task<IActionResult> Add(AddServiciosViewModel viewModel)
        //{
        //    var servicios = new Servicio
        //}
    }
}
