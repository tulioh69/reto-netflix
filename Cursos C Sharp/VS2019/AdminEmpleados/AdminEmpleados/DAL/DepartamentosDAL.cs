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

            SqlCommand SQLComando = new SqlCommand("INSERT INTO Departamentos VALUES(@departamento)");
            SQLComando.Parameters.Add("@Departamento", SqlDbType.VarChar).Value = oDepartamentosBLL.Departamento;
            return conexion.ejecutarComandoSinRetornoDatos(SQLComando);
            //return conexion.ejecutarComandoSinRetornoDatos("INSERT INTO Departamentos (departamento) VALUES('"+oDepartamentosBLL.Departamento +"')");
        }
        public int Modificar(DepartamentoBLL oDepartamentosBLL)
        {
            conexion.ejecutarComandoSinRetornoDatos("UPDATE Departamentos SET departamento='"+oDepartamentosBLL.Departamento+"'"+
                "WHERE Id=" + oDepartamentosBLL.Id);
            return 1;
        }
        public int Eliminar(DepartamentoBLL oDepartamentosBLL)
        {
            conexion.ejecutarComandoSinRetornoDatos("DELETE Departamentos WHERE Id=" + oDepartamentosBLL.Id);
            return 1;
        }
        public DataSet MostrarDepartamentos()
        {
            SqlCommand sentencia = new SqlCommand("Select * from Departamentos");
            return conexion.EjecutarSentencia(sentencia);
        }
    }
}
