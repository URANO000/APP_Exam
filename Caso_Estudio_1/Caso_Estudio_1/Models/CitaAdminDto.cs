namespace Caso_Estudio_1.Models
{
    public class CitaAdminDto
    {
        public int Id { get; set; }
        public string NombreDeLaPersona { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Identificacion { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaDeLaCita { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string EspecialidadEnProsa { get; set; }
        public string Especialista { get; set; }
        public string Clinica { get; set; }
    }
}
