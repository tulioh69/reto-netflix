using System;
using System.Collections.Generic;
using System.Text;
using TestEjemplo;
using Xunit;

namespace TestingExampleTest
{
    public class FactorialServiceTest
    {
        [Fact]
        public void TestFactorialZero()
        {
            //arrange
            FactorialService service = new FactorialService();

            //act
            int result =service.Calcular(0);

            //assert
            Assert.Equal(1, result);

        }
        [Fact]
        public void TestFactorialFive()
        {
            FactorialService service = new FactorialService();
            int result = service.Calcular(5);

            Assert.Equal(120, result);

        }

    }
}
