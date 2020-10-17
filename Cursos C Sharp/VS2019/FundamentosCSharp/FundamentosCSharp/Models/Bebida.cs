using System;
using System.Collections.Generic;
using System.Text;

namespace FundamentosCSharp.Models
{
    class Bebida
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }

        public Bebida(string Nombre, int Cantidad)
        {
            this.Nombre = Nombre;
            this.Cantidad = Cantidad;
        }

        public void Beberse(int CusntoBebio)
        {
            this.Cantidad -= CusntoBebio;
        }
    }
}
