using System;
using System.Web;

namespace ServicioPrueba1
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Agregar los encabezados para permitir solicitudes desde tu frontend
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "http://192.168.15.188:3010"); 

            // Si la solicitud es un preflight (OPTIONS), respondemos con los encabezados necesarios
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //  HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE, GET, OPTIONS");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000"); // Tiempo de vida de la respuesta CORS
                HttpContext.Current.Response.End(); // Finaliza la solicitud OPTIONS
            }
        }


        protected void Application_Start(object sender, EventArgs e) { }

        protected void Session_Start(object sender, EventArgs e) { }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) { }

        protected void Application_Error(object sender, EventArgs e) { }

        protected void Session_End(object sender, EventArgs e) { }

        protected void Application_End(object sender, EventArgs e) { }
    }
}
