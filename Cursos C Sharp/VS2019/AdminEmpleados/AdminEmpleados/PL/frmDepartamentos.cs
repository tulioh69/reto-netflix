using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminEmpleados.BLL;
using AdminEmpleados.DAL;

namespace AdminEmpleados.PL
{
    public partial class frmDepartamentos : Form
    {
        DepartamentosDAL oDepartamentosDAL;
        public frmDepartamentos()
        {
            oDepartamentosDAL = new DepartamentosDAL();
            InitializeComponent();
            dgvDepartamentos.DataSource = oDepartamentosDAL.MostrarDepartamentos().Tables[0];
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //instruccion GUI

            ConexionDAL conexion = new ConexionDAL();
            MessageBox.Show("Conectado..." );
            // clase DAL departamentos ... objeto que tiene la inforacion del GUI
            oDepartamentosDAL.Agregar(RecuperarInformacion());
        }
        private DepartamentoBLL RecuperarInformacion()
        {
            DepartamentoBLL oDepartamentoBLL = new DepartamentoBLL();
            int Id = 0; int.TryParse(txtId.Text, out Id);
            oDepartamentoBLL.Id = Id;
            oDepartamentoBLL.Departamento = txtDepartamento.Text;
            return oDepartamentoBLL;
        }
    }
}
