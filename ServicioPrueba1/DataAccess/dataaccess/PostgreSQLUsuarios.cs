using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using Npgsql;
namespace ServicioPrueba1
{
    public class PostgreSQLUsuarios
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConnectionXD"].ConnectionString;

        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, nombre, apellidop, apellidom, fecharegistro, activo FROM usuarios";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaUsuarios.Add(new Usuario
                        {
                            ID = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            ApellidoP = reader.GetString(2),
                            ApellidoM = reader.GetString(3),
                            FechaRegistro = reader.GetDateTime(4).ToString("yyyy-MM-ddTHH:mm:ss"), // Conversión a string en formato ISO 8601
                            Activo = reader.GetBoolean(5)
                        });
                    }
                }
            }
            return listaUsuarios;
        }

        public bool AgregarUsuario(Usuario nuevoUsuario)
        {
            try
            {
                DateTime fechaRegistro = nuevoUsuario.GetFechaRegistro(); // Convertimos manualmente

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO usuarios (nombre, apellidop, apellidom, fecharegistro, activo) 
                            VALUES (@nombre, @apellidop, @apellidom, @fecharegistro, @activo)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoUsuario.Nombre);
                        cmd.Parameters.AddWithValue("@apellidop", nuevoUsuario.ApellidoP);
                        cmd.Parameters.AddWithValue("@apellidom", nuevoUsuario.ApellidoM);
                        cmd.Parameters.AddWithValue("@fecharegistro", fechaRegistro);
                        cmd.Parameters.AddWithValue("@activo", nuevoUsuario.Activo);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar usuario: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }
        }

        public bool EditarUsuario(Usuario usuario)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE usuarios 
                             SET nombre = @nombre, 
                                 apellidop = @apellidop, 
                                 apellidom = @apellidom, 
                                 fecharegistro = @fecharegistro,
                                 activo = @activo
                             WHERE id = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.ID);
                        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@apellidop", usuario.ApellidoP);
                        cmd.Parameters.AddWithValue("@apellidom", usuario.ApellidoM);
                        cmd.Parameters.AddWithValue("@fecharegistro", usuario.GetFechaRegistro()); // Convertir fecha
                        cmd.Parameters.AddWithValue("@activo", usuario.Activo);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0; // Devuelve true si al menos una fila se modificó
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar usuario: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }
        }

        public bool EliminarUsuario(int usuarioID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"DELETE FROM usuarios WHERE id = @usuarioID";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioID", usuarioID);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }
        }


    }
}