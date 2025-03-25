using Npgsql;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using ServicioPrueba1.DataAccess;

namespace ServicioPrueba1
{
    public class PostgreSQLUsuariosLogs
    {
        private PostgreSQLConexion conexion;

        public PostgreSQLUsuariosLogs()
        {
            conexion = new PostgreSQLConexion();
        }

        // ✅ Método para registrar logs de usuarios
        public void RegistrarLog(int usuarioID, string accion, UsuarioLog usuariol)
        {
            try
            {
                DateTime fecha = usuariol.GetFechalog().ToUniversalTime();
                using (var conn = conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO usuarios_logs (usuario_id, nombre, apellidop, apellidom, correo, accion, fecha) 
                                     VALUES (@usuario_id, @nombre, @apellidop, @apellidom, @correo, @accion, @fecha);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario_id", usuarioID);
                        cmd.Parameters.AddWithValue("@nombre", usuariol.Nombre);
                        cmd.Parameters.AddWithValue("@apellidop", usuariol.ApellidoP);
                        cmd.Parameters.AddWithValue("@apellidom", usuariol.ApellidoM);
                        cmd.Parameters.AddWithValue("@correo", usuariol.Correo);
                        cmd.Parameters.AddWithValue("@accion", accion);
                        cmd.Parameters.AddWithValue("@fecha", DateTime.UtcNow);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar log de usuario: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }
        }

        // ✅ Método para obtener el historial de movimientos de usuarios
        public List<UsuarioLog> ObtenerHistorialUsuarios()
        {
            List<UsuarioLog> historial = new List<UsuarioLog>();

            try
            {
                using (var conn = conexion.ObtenerConexion())
                {
                    string query = @"SELECT id, usuario_id, nombre, apellidop, apellidom, correo, accion, fecha 
                                     FROM usuarios_logs";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historial.Add(new UsuarioLog
                            {
                                ID = reader.GetInt32(0),
                                UsuarioID = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                ApellidoP = reader.GetString(3),
                                ApellidoM = reader.GetString(4),
                                Correo = reader.GetString(5),
                                Accion = reader.GetString(6),
                                Fecha = reader.GetDateTime(7).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener historial de usuarios: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }

            return historial;
        }
    }
}
