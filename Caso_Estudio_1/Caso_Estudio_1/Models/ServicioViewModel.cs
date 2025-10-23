namespace Caso_Estudio_1.Models
{
    public class ServicioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Especialidad { get; set; }
        public string Especialista { get; set; }
        public decimal Monto { get; set; }
        public string Clinica { get; set; }
        public decimal IVA { get; set; }

        public string EspecialidadTexto => Especialidad switch
        {
            1 => "Medicina General",
            2 => "Imagenología",
            3 => "Microbiología",
            _ => "N/D"
        };
    }
}
