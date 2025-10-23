using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caso_Estudio_1.Models.Entities
{

        [Table("CITAS")]
        public class Citas
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(150)]
            public string NombreDeLaPersona { get; set; }

            [Required]
            [StringLength(30)]
            public string Identificacion { get; set; }

            [Required]
            [StringLength(10)]
            public string Telefono { get; set; }

            [Required]
            [StringLength(50)]
            public string Correo { get; set; }

            [Required]
            public DateTime FechaNacimiento { get; set; }

            [Required]
            [StringLength(200)]
            public string Direccion { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal MontoTotal { get; set; }

            [Required]

            public DateTime FechaDeLaCita { get; set; }

            [Required]
            public DateTime FechaDeRegistro { get; set; }

            [Required]
            public int IdServicio { get; set; }

            [ForeignKey("IdServicio")]
            public virtual Servicios Servicio { get; set; }

    }
    
}
