using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServicioPrueba1
{
	public class UsuarioLogs : IUsuarioLogs
	{
		private PostgreSQLUsuariosLogs dblogs = new PostgreSQLUsuariosLogs();
        public List<UsuarioLog> ObtenerHistorialUsuarios()
        {
            return dblogs.ObtenerHistorialUsuarios();
        }
    }
}
