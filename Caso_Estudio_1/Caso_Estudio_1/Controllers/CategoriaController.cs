using Caso_Estudio_1.Data;
using Caso_Estudio_1.Models;
using Caso_Estudio_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caso_Estudio_1.Controllers
{
    public class CategoriaController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public CategoriaController(ApplicationDbContext dbContext)
        {
            //Inject the application db context
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoriaViewModel viewModel)
        {
            //Create the categoria entity
            var categoria = new Categoria
            {
                //Extract and assign the values from the viewModel
                nombre = viewModel.nombre,
                detalle = viewModel.detalle
            };


            await dbContext.Categorias.AddAsync(categoria);
            //Save the changes
            await dbContext.SaveChangesAsync();  //Whatever isn't saved yet, will be saved to the database
            return RedirectToAction("List", "Categorias");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var categoria = await dbContext.Categorias.ToListAsync();

            return View(categoria);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await dbContext.Categorias.FindAsync(id);
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Categoria viewModel)
        {
            var categoria = await dbContext.Categorias.FindAsync(viewModel.categoriaId);

            //Checking if the category exists -- I am using this method to FILL the slots with the info of the matching id
            if(categoria != null)
            {
                categoria.nombre = viewModel.nombre;
                categoria.detalle = viewModel.detalle;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Categoria");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await dbContext.Categorias.FirstOrDefaultAsync(x => x.categoriaId == id);

            return View(categoria);


        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await dbContext.Categorias.FindAsync(id);

            if (categoria != null) { 
                dbContext.Categorias.Remove(categoria);

                //Finally save changes to the db
                await dbContext.SaveChangesAsync();
            }

            //Now I can redirect to list!
            return RedirectToAction("List", "Categoria");
        }
    }
}
