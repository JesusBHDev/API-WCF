using System;
using System.Globalization;
using System.Runtime.Serialization;

[DataContract]
public class Usuario
{
    [DataMember]
    public int ID { get; set; }

    [DataMember]
    public string Nombre { get; set; }

    [DataMember]
    public string ApellidoP { get; set; }

    [DataMember]
    public string ApellidoM { get; set; }

    [DataMember]
    public string Correo { get; set; }

    [DataMember]
    public string FechaNac { get; set; }

    [DataMember]
    public string FechaRegistro { get; set; }

    [DataMember]
    public bool Activo { get; set; }

    [DataMember]
    public string FechaMod { get; set; }

    public DateTime GetFechaNac()
    {
        if (FechaNac.StartsWith("/Date("))
        {
            string timestamp = FechaNac.Replace("/Date(", "").Replace(")/", "");
            long milliseconds = long.Parse(timestamp);
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
        }
        else
        {
            return DateTime.Parse(FechaNac, null, DateTimeStyles.RoundtripKind);
        }
    }

    public DateTime GetFechaRegistro()
    {
        if (FechaRegistro.StartsWith("/Date("))
        {
            string timestamp = FechaRegistro.Replace("/Date(", "").Replace(")/", "");
            long milliseconds = long.Parse(timestamp);
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
        }
        else
        {
            return DateTime.Parse(FechaRegistro, null, DateTimeStyles.RoundtripKind);
        }
    }

    public DateTime GetFechaMod()
    {
        if (FechaMod.StartsWith("/Date("))
        {
            string timestamp = FechaMod.Replace("/Date(", "").Replace(")/", "");
            long milliseconds = long.Parse(timestamp);
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
        }
        else
        {
            return DateTime.Parse(FechaMod, null, DateTimeStyles.RoundtripKind);
        }
    }
}

