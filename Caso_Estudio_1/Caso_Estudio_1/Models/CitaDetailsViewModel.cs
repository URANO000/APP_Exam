using System;

namespace Caso_Estudio_1.Models
{
    public class CitaDetailsViewModel
    {
        public int Id { get; set; }
        public string NombreDeLaPersona { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string NombreServicio { get; set; }
        public string EspecialidadTexto { get; set; }
        public string Especialista { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaDeLaCita { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public string Clinica { get; set; }
    }
}
