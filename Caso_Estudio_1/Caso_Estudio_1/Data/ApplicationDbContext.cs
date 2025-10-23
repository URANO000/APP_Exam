using Caso_Estudio_1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Caso_Estudio_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        //Entities are gonna go here, we are referencing the tables
        public DbSet<Citas> Citas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
