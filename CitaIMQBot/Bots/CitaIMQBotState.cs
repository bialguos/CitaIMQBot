using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Bots
{
    public class CitaIMQBotState
    {
        public string Nombre { get; set; }

        public int NumeroTarjeta { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public string Agenda { get; set; }
        public string Especialidad { get; set; }

        public List<string> Agendasposibles { get; set; }

        public ProvinciaEnum Provincia { get; set; }

        public DateTime FechaBusqueda { get; set; }


        public CitaIMQBotState()
        {
            Agendasposibles = new List<string>();
        }
    }

    public enum ProvinciaEnum
    {
        Bizkaia=1,
        Araba=2,
        Gipuzkoa=3
    }
}
