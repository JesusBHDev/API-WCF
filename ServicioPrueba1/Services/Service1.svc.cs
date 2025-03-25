using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServicioPrueba1
{
    public class Service1 : IService1
	{
        private static List<string> listaPersonas = new List<string>();

        public string ValidarVoto(string nombre, int edad)
        {
            if (edad >= 18)
                return $"✅ {nombre}, tienes {edad} años y puedes votar.";
            else
                return $"❌ {nombre}, tienes {edad} años y NO puedes votar. Te faltan {18 - edad} años.";
        }

        public string RegistrarPersona(string nombre, int edad)
        {
            string persona = $"{nombre} - {edad} años";
            listaPersonas.Add(persona);
            return $"👤 {nombre} ha sido registrado correctamente.";
        }

        public List<string> ObtenerPersonasRegistradas()
        {
            return listaPersonas;
        }
    }
}
