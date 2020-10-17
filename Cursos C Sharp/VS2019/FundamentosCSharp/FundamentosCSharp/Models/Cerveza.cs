using System;
using System.Collections.Generic;
using System.Text;

namespace FundamentosCSharp.Models
{
    class Cerveza : Bebida, IBebidaAlcoholica
    {
        public int Alcohol { get; set; }
        public string Marca { get; set; }
        public void MaxRecomendado()
        {
        Console.WriteLine("El maximo permitido es 10 botellas");

        }
        public Cerveza(int Cantidad, string Nombre="Cerveza") : base(Nombre,Cantidad)
        {

        }
    }
}
