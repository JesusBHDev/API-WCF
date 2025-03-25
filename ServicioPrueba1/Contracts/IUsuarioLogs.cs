using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServicioPrueba1
{
	// NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IUsuarioLogs" en el código y en el archivo de configuración a la vez.
	[ServiceContract]
	public interface IUsuarioLogs
	{
        [OperationContract]
        [WebGet(UriTemplate = "/ObtenerHistorialUsuarios",
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<UsuarioLog> ObtenerHistorialUsuarios();
    }
}
