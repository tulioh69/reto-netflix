using System;
using System.Collections.Generic;
using System.Text;

namespace FundamentosCSharp.Models
{
    class Vino : Bebida, IBebidaAlcoholica
    {
        public int Alcohol { get; set; }
        public void MaxRecomendado()
        {
            Console.WriteLine("El maximo permitido es 3 copas");

        }
        public Vino(int Cantidad, string Nombre = "Vino") : base(Nombre, Cantidad)
        {

        }
    }
}
