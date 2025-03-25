using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace ServicioPrueba1
{
	[ServiceContract]
	public interface IService1
	{
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/ValidarVoto",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string ValidarVoto(string nombre, int edad);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/RegistrarPersona",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string RegistrarPersona(string nombre, int edad);

        [OperationContract]
        [WebGet(UriTemplate = "/ObtenerPersonasRegistradas",
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<string> ObtenerPersonasRegistradas();

    }

}
