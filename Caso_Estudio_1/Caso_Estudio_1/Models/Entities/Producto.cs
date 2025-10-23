using System.ComponentModel.DataAnnotations.Schema;

namespace Caso_Estudio_1.Models.Entities
{
    [Table("PRODUCTO")]
    public class Producto
    {
        public int productoId { get; set; }
        public string nombre { get; set; }
        public double precio { get; set; }
        public int categoriaId { get; set; }

    }
}
