using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServicioPrueba1
{
    public class Usuarios : IUsuarios
    {
        private PostgreSQLUsuarios dbHelper = new PostgreSQLUsuarios();
        public List<Usuario> ObtenerUsuarios(string ultimaFechaConsulta)
        {
            return dbHelper.ObtenerUsuarios(ultimaFechaConsulta);
        }

        public bool AgregarUsuario(Usuario nuevoUsuario)
        {
            try
            {
                return dbHelper.AgregarUsuario(nuevoUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Usuarios.svc -> AgregarUsuario: " + ex.Message);
                throw new FaultException("Error en el servicio: " + ex.Message);
            }
        }

        public bool EditarUsuario(Usuario usuario)
        {
            try
            {
                return dbHelper.EditarUsuario(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Usuarios.svc -> EditarUsuario: " + ex.Message);
                throw new FaultException("Error en el servicio: " + ex.Message);
            }
        }

        public bool EliminarUsuario(int usuarioID)
        {
            try
            {
                return dbHelper.EliminarUsuario(usuarioID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Usuarios.svc -> EliminarUsuario: " + ex.Message);
                throw new FaultException("Error en el servicio: " + ex.Message);
            }
        }


    }
}
