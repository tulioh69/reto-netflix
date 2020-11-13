using System;
using System.Collections.Generic;
using System.Text;

namespace TestEjemplo
{
    public class FactorialService
    {
        public int Calcular(int number)
        { 
            if(number ==0)
            {
                return 1;
            }
            int result = 1;
            for(int index = 1; index <=number; index++)
            {
                result *= index;
            }
            return result;

        }

    }
}
