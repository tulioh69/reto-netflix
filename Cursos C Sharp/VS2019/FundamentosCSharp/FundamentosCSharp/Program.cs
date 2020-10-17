using System;
using System.Collections.Generic;
using FundamentosCSharp.Models;

namespace FundamentosCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
                       
            CervezaBD cervezaBD = new CervezaBD();
            //insertamos datos

            //{
            //    Cerveza cerveza = new Cerveza(15, "Quilmes");
            //    cerveza.Marca = "Quilmes";
            //    cerveza.Alcohol = 6;
            //    cervezaBD.Add(cerveza);
            //}

            {
                cervezaBD.Delete(4);
            }
            //obtener todas las cervezas
            var cervezas = cervezaBD.Get();
            foreach(var cerveza in cervezas)
            {
                Console.WriteLine(cerveza.Nombre);
            }
        }
    }
}
