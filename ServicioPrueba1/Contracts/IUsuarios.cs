using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServicioPrueba1
{
    [ServiceContract]
    public interface IUsuarios
    {
        [OperationContract]
        [WebGet(UriTemplate = "/ObtenerUsuarios?ultimaFechaConsulta={ultimaFechaConsulta}",
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<Usuario> ObtenerUsuarios(string ultimaFechaConsulta);


        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/AgregarUsuario",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool AgregarUsuario(Usuario nuevoUsuario);

        [OperationContract]
        [WebInvoke(Method ="POST",
            UriTemplate = "/ActualizarUsuario",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool EditarUsuario(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/EliminarUsuario",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool EliminarUsuario(int usuarioID);

    }
}
