using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ServicioPrueba1
{
    [DataContract]
    public class UsuarioLog
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int UsuarioID { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string ApellidoP { get; set; }
        [DataMember]
        public string ApellidoM { get; set; }
        [DataMember]
        public string Correo { get; set; }
        [DataMember]
        public string Accion { get; set; }
       
        [DataMember]
        public string Fecha { get; set; }

        public DateTime GetFechalog()
        {
            if (Fecha.StartsWith("/Date("))
            {
                string timestamp = Fecha.Replace("/Date(", "").Replace(")/", "");
                long milliseconds = long.Parse(timestamp);
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
            }
            else
            {
                return DateTime.Parse(Fecha, null, DateTimeStyles.RoundtripKind);
            }
        }
    }
}