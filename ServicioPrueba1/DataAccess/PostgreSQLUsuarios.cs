using System;
using System.Collections.Generic;
using System.ServiceModel;
using Npgsql;
using ServicioPrueba1.DataAccess; 

namespace ServicioPrueba1
{
    public class PostgreSQLUsuarios
    {
        private PostgreSQLConexion conexion; 

        public PostgreSQLUsuarios()
        {
            conexion = new PostgreSQLConexion();
        }

        public List<Usuario> ObtenerUsuarios(string ultimaFechaConsulta)
        {
            List<Usuario> listaUsuarios = new List<Usuario>();

            using (var conn = conexion.ObtenerConexion())
            {
                string query;

                // 🚀 Primera carga: Solo traer usuarios activos
                if (string.IsNullOrEmpty(ultimaFechaConsulta))
                {
                    query = @"
                    SELECT id, nombre, apellidop, apellidom, correo, fechanac, fecharegistro, activo, fechamod
                    FROM usuarios 
                    WHERE activo = TRUE";
                }
                else
                {
                    // 🚀 Cargas posteriores: Obtener TODOS los usuarios modificados (activos e inactivos)
                    query = @"
                    SELECT id, nombre, apellidop, apellidom, correo, fechanac, fecharegistro, activo, fechamod
                    FROM usuarios 
                    WHERE fechamod > @ultimaFechaConsulta";
                }

                DateTime? fechaConsulta = null;
                if (!string.IsNullOrEmpty(ultimaFechaConsulta))
                {
                    if (DateTime.TryParse(ultimaFechaConsulta, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime parsedDate))
                    {
                        fechaConsulta = parsedDate.ToUniversalTime();  // Convertir a UTC antes de comparar
                    }
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (fechaConsulta.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ultimaFechaConsulta", fechaConsulta.Value);
                    }

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
                                Correo = reader.GetString(4),
                                FechaNac = reader.GetDateTime(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                FechaRegistro = reader.GetDateTime(6).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                Activo = reader.GetBoolean(7),
                                FechaMod = reader.GetDateTime(8).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                            });
                        }
                    }
                }
            }
            return listaUsuarios;
        }


        public bool AgregarUsuario(Usuario nuevoUsuario)
        {
            try
            {
                DateTime fechaNac = nuevoUsuario.GetFechaNac().ToUniversalTime();
                DateTime fechaRegistro = DateTime.UtcNow;
                DateTime fechaMod = DateTime.UtcNow;

                using (var conn = conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO usuarios (nombre, apellidop, apellidom, correo, fechanac, fecharegistro, activo, fechamod) 
                              VALUES (@nombre, @apellidop, @apellidom, @correo, @fechanac, @fecharegistro, @activo, @fechamod)
                              RETURNING id;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoUsuario.Nombre);
                        cmd.Parameters.AddWithValue("@apellidop", nuevoUsuario.ApellidoP);
                        cmd.Parameters.AddWithValue("@apellidom", nuevoUsuario.ApellidoM);
                        cmd.Parameters.AddWithValue("@correo", nuevoUsuario.Correo);
                        cmd.Parameters.AddWithValue("@fechanac", fechaNac);
                        cmd.Parameters.AddWithValue("@fecharegistro", fechaRegistro);
                        cmd.Parameters.AddWithValue("@activo", nuevoUsuario.Activo);
                        cmd.Parameters.AddWithValue("@fechamod", fechaMod);

                        int usuarioID = (int)cmd.ExecuteScalar(); // Obtener ID del usuario insertado

                        if (usuarioID > 0)
                        {
                            // ✅ Crear objeto UsuarioLog
                            UsuarioLog usuarioLog = new UsuarioLog
                            {
                                UsuarioID = usuarioID,
                                Nombre = nuevoUsuario.Nombre,
                                ApellidoP = nuevoUsuario.ApellidoP,
                                ApellidoM = nuevoUsuario.ApellidoM,
                                Correo = nuevoUsuario.Correo,
                                Accion = "INSERT",
                                Fecha = DateTime.UtcNow.ToString("o") // Formato ISO 8601
                            };

                            // ✅ Registrar en logs
                            PostgreSQLUsuariosLogs log = new PostgreSQLUsuariosLogs();
                            log.RegistrarLog(usuarioID, "INSERT", usuarioLog);
                        }


                        return usuarioID > 0;
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
                using (var conn = conexion.ObtenerConexion()) // Obtener la conexión
                {
                    string query = @"UPDATE usuarios 
                       SET nombre = @nombre, 
                           apellidop = @apellidop, 
                           apellidom = @apellidom,
                           correo = @correo,
                           fechanac = @fechanac,
                           activo = @activo,
                           FechaMod = NOW()
                       WHERE id = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.ID);
                        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@apellidop", usuario.ApellidoP);
                        cmd.Parameters.AddWithValue("@apellidom", usuario.ApellidoM);
                        cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("@fechanac", usuario.GetFechaNac());
                        cmd.Parameters.AddWithValue("@activo", usuario.Activo);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // ✅ Crear objeto UsuarioLog
                            UsuarioLog usuarioLog = new UsuarioLog
                            {
                                UsuarioID = usuario.ID,
                                Nombre = usuario.Nombre,
                                ApellidoP = usuario.ApellidoP,
                                ApellidoM = usuario.ApellidoM,
                                Correo = usuario.Correo,
                                Accion = "UPDATE",
                                Fecha = DateTime.UtcNow.ToString("o") // Formato ISO 8601
                            };

                            // ✅ Registrar en logs
                            PostgreSQLUsuariosLogs log = new PostgreSQLUsuariosLogs();
                            log.RegistrarLog(usuario.ID, "UPDATE", usuarioLog);
                        }

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
                using (var conn = conexion.ObtenerConexion()) // Obtener la conexión
                {
                    string query = @"UPDATE usuarios 
                     SET Activo = FALSE, 
                         FechaMod = NOW() 
                     WHERE id = @usuarioID";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioID", usuarioID);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // ✅ Obtener datos del usuario antes de "eliminarlo"
                            Usuario usuarioEliminado = ObtenerUsuarioPorID(usuarioID);

                            if (usuarioEliminado != null)
                            {
                                // ✅ Crear objeto UsuarioLog
                                UsuarioLog usuarioLog = new UsuarioLog
                                {
                                    UsuarioID = usuarioEliminado.ID,
                                    Nombre = usuarioEliminado.Nombre,
                                    ApellidoP = usuarioEliminado.ApellidoP,
                                    ApellidoM = usuarioEliminado.ApellidoM,
                                    Correo = usuarioEliminado.Correo,
                                    Accion = "DELETE",
                                    Fecha = DateTime.UtcNow.ToString("o") // Formato ISO 8601
                                };

                                // ✅ Registrar en logs
                                PostgreSQLUsuariosLogs log = new PostgreSQLUsuariosLogs();
                                log.RegistrarLog(usuarioID, "DELETE", usuarioLog);
                            }
                        }

                        return filasAfectadas > 0; // Retorna true si se modificó al menos una fila
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al desactivar usuario: " + ex.Message);
                throw new FaultException("Error en la base de datos: " + ex.Message);
            }
        }

        // ✅ Método auxiliar para obtener un usuario por ID antes de eliminarlo
        private Usuario ObtenerUsuarioPorID(int usuarioID)
        {
            Usuario usuario = null;

            try
            {
                using (var conn = conexion.ObtenerConexion())
                {
                    string query = @"SELECT id, nombre, apellidop, apellidom, correo, activo 
                     FROM usuarios 
                     WHERE id = @usuarioID";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuarioID", usuarioID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = new Usuario
                                {
                                    ID = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    ApellidoP = reader.GetString(2),
                                    ApellidoM = reader.GetString(3),
                                    Correo = reader.GetString(4),
                                    Activo = reader.GetBoolean(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener usuario por ID: " + ex.Message);
            }

            return usuario;
        }


    }
}
