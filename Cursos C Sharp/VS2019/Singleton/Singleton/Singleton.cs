using System;
using System.Collections.Generic;
using System.Text;

namespace Singleton
{
    public class Singleton
    {
        private static Singleton instance = null;
        public string mensaje = "";
        protected Singleton() 
        {
            mensaje = "Hola mundo";
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new Singleton();

                return instance;
            }
        }
    }
}
