using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Caso_Estudio_1.Models;
using Microsoft.Extensions.Configuration;

namespace Caso_Estudio_1.Data
{
    public class CitasData
    {
        private readonly string _connectionString;

        // Constructor: lee la cadena "Contexto" del appsettings.json
        public CitasData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Contexto");
        }

        // ✅ 1. LISTAR SERVICIOS ACTIVOS
        public List<ServicioViewModel> ListarServicios()
        {
            var lista = new List<ServicioViewModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT * FROM SERVICIOS WHERE Estado = 1";

                using (var cmd = new SqlCommand(query, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ServicioViewModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            Especialista = dr["Especialista"].ToString(),
                            Especialidad = Convert.ToInt32(dr["Especialidad"]),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            Clinica = dr["Clinica"].ToString(),
                            IVA = Convert.ToDecimal(dr["IVA"])
                        });
                    }
                }
            }
            return lista;
        }

        // ✅ 2. REGISTRAR UNA NUEVA CITA
        public void RegistrarCita(AddCitaViewModel model)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                INSERT INTO CITAS 
                (NombreDeLaPersona, Identificacion, Telefono, Correo, FechaNacimiento, Direccion, MontoTotal, FechaDeLaCita, IdServicio)
                VALUES (@Nombre, @Identificacion, @Telefono, @Correo, @FechaNacimiento, @Direccion, @MontoTotal, @FechaDeLaCita, @IdServicio)";

                using (var cmd = new SqlCommand(query, con))
                {
                    // Calcula el monto total según el IVA
                    decimal montoTotal = model.Monto + (model.Monto * (model.IVA / 100m));

                    cmd.Parameters.AddWithValue("@Nombre", model.NombreDeLaPersona);
                    cmd.Parameters.AddWithValue("@Identificacion", model.Identificacion);
                    cmd.Parameters.AddWithValue("@Telefono", model.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@Direccion", model.Direccion);
                    cmd.Parameters.AddWithValue("@MontoTotal", montoTotal);
                    cmd.Parameters.AddWithValue("@FechaDeLaCita", model.FechaDeLaCita);
                    cmd.Parameters.AddWithValue("@IdServicio", model.IdServicio);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ✅ 3. BUSCAR CITA POR ID
        public CitaDetailsViewModel BuscarCita(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    SELECT c.*, s.Nombre AS NombreServicio, s.Especialista, s.Clinica, s.Especialidad
                    FROM CITAS c 
                    INNER JOIN SERVICIOS s ON c.IdServicio = s.Id
                    WHERE c.Id = @Id";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Construye el objeto CitaDetailsViewModel con toda la información
                            return new CitaDetailsViewModel
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                NombreDeLaPersona = dr["NombreDeLaPersona"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Identificacion = dr["Identificacion"].ToString(),
                                FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                                Direccion = dr["Direccion"].ToString(),
                                NombreServicio = dr["NombreServicio"].ToString(),
                                Especialista = dr["Especialista"].ToString(),
                                Clinica = dr["Clinica"].ToString(),
                                EspecialidadTexto = ((int)dr["Especialidad"]) switch
                                {
                                    1 => "Medicina General",
                                    2 => "Imagenología",
                                    3 => "Microbiología",
                                    _ => "N/D"
                                },
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                                FechaDeLaCita = Convert.ToDateTime(dr["FechaDeLaCita"]),
                                FechaDeRegistro = Convert.ToDateTime(dr["FechaDeRegistro"])
                            };
                        }
                    }
                }
            }
            // Si no encuentra la cita, retorna null
            return null;
        }
    }
}
