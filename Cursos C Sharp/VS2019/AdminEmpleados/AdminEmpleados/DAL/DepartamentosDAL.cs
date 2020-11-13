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
        public bool Modificar(DepartamentoBLL oDepartamentosBLL)
        {
            SqlCommand SQLComando = new SqlCommand("UPDATE Departamentos SET departamento=@departamento WHERE Id=@Id");
            SQLComando.Parameters.Add("@Id", SqlDbType.Int).Value = oDepartamentosBLL.Id;
            SQLComando.Parameters.Add("@Departamento", SqlDbType.VarChar).Value = oDepartamentosBLL.Departamento;
            return conexion.ejecutarComandoSinRetornoDatos(SQLComando);
        }
        public bool Eliminar(DepartamentoBLL oDepartamentosBLL)
        {
            SqlCommand SQLComando = new SqlCommand("DELETE Departamentos WHERE Id=@Id");
            SQLComando.Parameters.Add("@Id", SqlDbType.Int).Value = oDepartamentosBLL.Id;
            return conexion.ejecutarComandoSinRetornoDatos(SQLComando);
        }
        public DataSet MostrarDepartamentos()
        {
            SqlCommand sentencia = new SqlCommand("Select * from Departamentos");
            return conexion.EjecutarSentencia(sentencia);
        }
    }
}
