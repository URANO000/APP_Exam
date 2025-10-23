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

        // Constructor que obtiene la cadena de conexión desde appsettings.json
        public CitasData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Contexto");
        }

        // ✅ 1. LISTAR SERVICIOS DISPONIBLES
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
                            Nombre = dr["Nombre"].ToString()!,
                            Especialista = dr["Especialista"].ToString()!,
                            Especialidad = Convert.ToInt32(dr["Especialidad"]),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            Clinica = dr["Clinica"].ToString()!,
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

        // ✅ 3. BUSCAR UNA CITA POR ID
        public CitaDetailsViewModel? BuscarCita(int id)
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
            return null;
        }

        // ✅ 4. LISTAR TODAS LAS CITAS
        public List<CitaDetailsViewModel> ListarCitas()
        {
            var list = new List<CitaDetailsViewModel>();
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var sql = @"SELECT c.Id, c.NombreDeLaPersona, c.Telefono, c.Correo, c.Identificacion,
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

        // ✅ 5. OBTENER CITA PARA EDITAR
        public AddCitaViewModel? ObtenerCitaParaEditar(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var sql = @"SELECT c.*, s.Nombre AS NombreServicio, s.Especialista, s.Especialidad, s.Monto, s.IVA
                            FROM CITAS c INNER JOIN SERVICIOS s ON c.IdServicio = s.Id
                            WHERE c.Id=@Id";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return null;
                        return new AddCitaViewModel
                        {
                            IdServicio = Convert.ToInt32(dr["IdServicio"]),
                            NombreServicio = dr["NombreServicio"].ToString()!,
                            Especialista = dr["Especialista"].ToString()!,
                            EspecialidadTexto = ((int)dr["Especialidad"]) switch
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

        // ✅ 6. ACTUALIZAR UNA CITA
        public void ActualizarCita(int id, AddCitaViewModel m)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                decimal monto, iva;
                var getServ = new SqlCommand("SELECT Monto, IVA FROM SERVICIOS WHERE Id=@S", con);
                getServ.Parameters.AddWithValue("@S", m.IdServicio);
                using (var dr = getServ.ExecuteReader())
                {
                    if (!dr.Read()) throw new Exception("Servicio no encontrado");
                    monto = Convert.ToDecimal(dr["Monto"]);
                    iva = Convert.ToDecimal(dr["IVA"]);
                }

                var total = monto + (monto * (iva / 100m));

                var sql = @"UPDATE CITAS SET
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
                    cmd.Parameters.AddWithValue("@MontoTotal", total);
                    cmd.Parameters.AddWithValue("@FechaDeLaCita", m.FechaDeLaCita);
                    cmd.Parameters.AddWithValue("@IdServicio", m.IdServicio);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ✅ 7. ELIMINAR UNA CITA
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

        // ✅ 8. LISTA BÁSICA DE SERVICIOS PARA COMBOBOX
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
