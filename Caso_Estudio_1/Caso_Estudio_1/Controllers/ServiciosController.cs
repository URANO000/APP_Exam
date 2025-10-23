using Caso_Estudio_1.Data;
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
        public IActionResult Index()
        {
            return View();
        }
    }
}
