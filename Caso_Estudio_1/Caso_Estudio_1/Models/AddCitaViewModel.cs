using System;
using System.ComponentModel.DataAnnotations;

namespace Caso_Estudio_1.Models
{
    public class AddCitaViewModel
    {
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string EspecialidadTexto { get; set; }
        public string Especialista { get; set; }
        public decimal Monto { get; set; }
        public decimal IVA { get; set; }

        [Required, StringLength(150)] public string NombreDeLaPersona { get; set; }
        [Required, StringLength(30)] public string Identificacion { get; set; }
        [Required, StringLength(10)] public string Telefono { get; set; }
        [Required, EmailAddress, StringLength(50)] public string Correo { get; set; }
        [Required] public DateTime FechaNacimiento { get; set; }
        [Required, StringLength(200)] public string Direccion { get; set; }
        [Required] public DateTime FechaDeLaCita { get; set; }

        public decimal MontoTotal => Monto + (Monto * (IVA / 100m));
    }
}

