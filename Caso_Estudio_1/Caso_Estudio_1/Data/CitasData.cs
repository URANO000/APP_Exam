using System;
using System.Collections.Generic;
using System.Data; // <- para SqlDbType
using Microsoft.Data.SqlClient;
using Caso_Estudio_1.Models;
using Microsoft.Extensions.Configuration;

namespace Caso_Estudio_1.Data
{
    public class CitasData
    {
        private readonly string _connectionString;

        public CitasData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Contexto");
        }

        public List<ServicioViewModel> ListarServicios()
        {
            var lista = new List<ServicioViewModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                const string query = "SELECT * FROM SERVICIOS WHERE Estado = 1";

                using (var cmd = new SqlCommand(query, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var especialidad = Convert.ToInt32(dr["Especialidad"]);
                        lista.Add(new ServicioViewModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nombre = dr["Nombre"].ToString() ?? string.Empty,
                            Especialista = dr["Especialista"].ToString() ?? string.Empty,
                            Especialidad = especialidad,
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            Clinica = dr["Clinica"].ToString() ?? string.Empty,
                            IVA = Convert.ToDecimal(dr["IVA"])
                        });

                    }
                }
            }
            return lista;
        }

        public void RegistrarCita(AddCitaViewModel model)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                const string query = @"
                INSERT INTO CITAS 
                (NombreDeLaPersona, Identificacion, Telefono, Correo, FechaNacimiento, Direccion, 
                 MontoTotal, FechaDeLaCita, FechaDeRegistro, IdServicio)
                VALUES 
                (@Nombre, @Identificacion, @Telefono, @Correo, @FechaNacimiento, @Direccion, 
                 @MontoTotal, @FechaDeLaCita, GETDATE(), @IdServicio)";

                using (var cmd = new SqlCommand(query, con))
                {
                    var total = model.Monto + (model.Monto * (model.IVA / 100m));

                    cmd.Parameters.AddWithValue("@Nombre", model.NombreDeLaPersona);
                    cmd.Parameters.AddWithValue("@Identificacion", model.Identificacion);
                    cmd.Parameters.AddWithValue("@Telefono", model.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@Direccion", model.Direccion);

                    // Tipar decimal explícitamente
                    var pTotal = cmd.Parameters.Add("@MontoTotal", SqlDbType.Decimal);
                    pTotal.Precision = 18; pTotal.Scale = 2; pTotal.Value = total;

                    cmd.Parameters.AddWithValue("@FechaDeLaCita", model.FechaDeLaCita);
                    cmd.Parameters.AddWithValue("@IdServicio", model.IdServicio);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public CitaDetailsViewModel? BuscarCita(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                const string query = @"
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
                            var esp = Convert.ToInt32(dr["Especialidad"]);
                            return new CitaDetailsViewModel
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                NombreDeLaPersona = dr["NombreDeLaPersona"].ToString()!,
                                Telefono = dr["Telefono"].ToString()!,
                                Correo = dr["Correo"].ToString()!,
                                Identificacion = dr["Identificacion"].ToString()!,
                                FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                                Direccion = dr["Direccion"].ToString()!,
                                NombreServicio = dr["NombreServicio"].ToString()!,
                                Especialista = dr["Especialista"].ToString()!,
                                Clinica = dr["Clinica"].ToString()!,
                                EspecialidadTexto = esp switch
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
            return null;
        }

        public List<CitaDetailsViewModel> ListarCitas()
        {
            var list = new List<CitaDetailsViewModel>();
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                const string sql = @"SELECT c.Id, c.NombreDeLaPersona, c.Telefono, c.Correo, c.Identificacion,
                                            c.FechaDeLaCita, c.MontoTotal,
                                            s.Nombre AS NombreServicio
                                     FROM CITAS c INNER JOIN SERVICIOS s ON c.IdServicio = s.Id
                                     ORDER BY c.Id DESC";
                using (var cmd = new SqlCommand(sql, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CitaDetailsViewModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            NombreDeLaPersona = dr["NombreDeLaPersona"].ToString()!,
                            Telefono = dr["Telefono"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Identificacion = dr["Identificacion"].ToString()!,
                            FechaDeLaCita = Convert.ToDateTime(dr["FechaDeLaCita"]),
                            MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                            NombreServicio = dr["NombreServicio"].ToString()!
                        });
                    }
                }
            }
            return list;
        }

        public AddCitaViewModel? ObtenerCitaParaEditar(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                const string sql = @"SELECT c.*, s.Nombre AS NombreServicio, s.Especialista, s.Especialidad, s.Monto, s.IVA
                                     FROM CITAS c INNER JOIN SERVICIOS s ON c.IdServicio = s.Id
                                     WHERE c.Id=@Id";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return null;
                        var esp = Convert.ToInt32(dr["Especialidad"]);
                        return new AddCitaViewModel
                        {
                            IdServicio = Convert.ToInt32(dr["IdServicio"]),
                            NombreServicio = dr["NombreServicio"].ToString()!,
                            Especialista = dr["Especialista"].ToString()!,
                            EspecialidadTexto = esp switch
                            {
                                1 => "Medicina General",
                                2 => "Imagenología",
                                3 => "Microbiología",
                                _ => "N/D"
                            },
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            IVA = Convert.ToDecimal(dr["IVA"]),
                            NombreDeLaPersona = dr["NombreDeLaPersona"].ToString()!,
                            Identificacion = dr["Identificacion"].ToString()!,
                            Telefono = dr["Telefono"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                            Direccion = dr["Direccion"].ToString()!,
                            FechaDeLaCita = Convert.ToDateTime(dr["FechaDeLaCita"])
                        };
                    }
                }
            }
        }

        public void ActualizarCita(int id, AddCitaViewModel m)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                decimal monto, iva;
                using (var getServ = new SqlCommand("SELECT Monto, IVA FROM SERVICIOS WHERE Id=@S", con))
                {
                    getServ.Parameters.AddWithValue("@S", m.IdServicio);
                    using (var dr = getServ.ExecuteReader())
                    {
                        if (!dr.Read()) throw new Exception("Servicio no encontrado");
                        monto = Convert.ToDecimal(dr["Monto"]);
                        iva = Convert.ToDecimal(dr["IVA"]);
                    }
                }

                var total = monto + (monto * (iva / 100m));

                const string sql = @"UPDATE CITAS SET
                                        NombreDeLaPersona=@Nombre, Identificacion=@Identificacion, Telefono=@Telefono,
                                        Correo=@Correo, FechaNacimiento=@FechaNacimiento, Direccion=@Direccion,
                                        MontoTotal=@MontoTotal, FechaDeLaCita=@FechaDeLaCita, IdServicio=@IdServicio
                                     WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", m.NombreDeLaPersona);
                    cmd.Parameters.AddWithValue("@Identificacion", m.Identificacion);
                    cmd.Parameters.AddWithValue("@Telefono", m.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", m.Correo);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", m.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@Direccion", m.Direccion);

                    var pTotal = cmd.Parameters.Add("@MontoTotal", SqlDbType.Decimal);
                    pTotal.Precision = 18; pTotal.Scale = 2; pTotal.Value = total;

                    cmd.Parameters.AddWithValue("@FechaDeLaCita", m.FechaDeLaCita);
                    cmd.Parameters.AddWithValue("@IdServicio", m.IdServicio);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EliminarCita(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("DELETE FROM CITAS WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ServicioViewModel> ListarServiciosBasico()
        {
            var list = new List<ServicioViewModel>();
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT Id, Nombre FROM SERVICIOS WHERE Estado=1 ORDER BY Nombre", con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ServicioViewModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nombre = dr["Nombre"].ToString()!
                        });
                    }
                }
            }
            return list;
        }
    }
}
