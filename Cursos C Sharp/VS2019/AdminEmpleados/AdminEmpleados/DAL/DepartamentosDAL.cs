using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using AdminEmpleados.BLL;

namespace AdminEmpleados.DAL
{
    class DepartamentosDAL
    {
        ConexionDAL conexion;
        public DepartamentosDAL()
        {
            conexion = new ConexionDAL();
        }
        public bool Agregar( DepartamentoBLL oDepartamentosBLL)
        {
            return conexion.ejecutarComandoSinRetornoDatos("INSERT INTO Departamentos (departamento) VALUES('"+oDepartamentosBLL.Departamento +"')");
        }
        public DataSet MostrarDepartamentos()
        {
            SqlCommand sentencia = new SqlCommand("Select * from Departamentos");
            return conexion.EjecutarSentencia(sentencia);
        }
    }
}
