using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caso_Estudio_1.Models.Entities
{
        [Table("SERVICIOS")]
        public class Servicios
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Nombre { get; set; }

            [Required]
            [StringLength(200)]
            public string Descripcion { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal Monto { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal IVA { get; set; }

            [Required]
            public int Especialidad { get; set; }

            [Required]
            [StringLength(200)]
            public string Especialista { get; set; }

            [Required]
            [StringLength(100)]
            public string Clinica { get; set; }

            [Required]
            public DateTime FechaDeRegistro { get; set; }

            public DateTime? FechaDeModificacion { get; set; }

            public bool? Estado { get; set; }
        }
    
}