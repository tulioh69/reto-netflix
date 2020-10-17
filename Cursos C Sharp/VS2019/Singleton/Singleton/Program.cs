using System;

namespace Singleton
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Singleton.Instance.mensaje);
            Singleton.Instance.mensaje = "Hola marte";
            Console.WriteLine(Singleton.Instance.mensaje);
        }
    
    }
}
