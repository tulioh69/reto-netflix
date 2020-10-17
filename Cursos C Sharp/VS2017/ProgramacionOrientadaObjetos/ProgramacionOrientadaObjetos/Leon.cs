using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramacionOrientadaObjetos
{
    public class Leon : Carnivoro
    {
        public string ColorCabello { get; set; }
        private int VelocidadDefecto = 20;

        public int Velocidad
        {
            get
            {
                return VelocidadDefecto;
            }
        }
        public override string GetNombre()
        {
            return "Soy un león llamado: " + Nombre;

        }
        public Leon(string Nombre) : this()
        {
            this.Nombre = Nombre;
        }

        public Leon()
        {
            if (this.Nombre == null || !this.Nombre.Equals(""))
                this.Nombre = "León";

            Console.WriteLine("carga de datos de la base de datos");
        }
        public Leon(string nombre, int velocidad)
        {
            Console.WriteLine("nombre" + Nombre + "veocidad" + velocidad);

        }
        public void Correr()
        {
            Console.WriteLine("Corriendo: " + VelocidadDefecto);
        }
        public void Correr(int velocidad)
        {
            Console.WriteLine("Corriendo: " + velocidad);
        }
        
    }
}
