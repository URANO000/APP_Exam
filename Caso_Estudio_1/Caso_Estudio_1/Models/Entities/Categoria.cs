using System.ComponentModel.DataAnnotations.Schema;

namespace Caso_Estudio_1.Models.Entities
{
    //I want to map these to the tables I created already
    [Table("CATEGORIA")]
    public class Categoria
    {
        public int categoriaId { get; set; }
        public string nombre { get; set; }
        public string detalle { get; set; }

    }
}

