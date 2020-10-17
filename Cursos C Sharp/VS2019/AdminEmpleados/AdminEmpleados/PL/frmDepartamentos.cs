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
            LlenarGrid();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //instruccion GUI

            ConexionDAL conexion = new ConexionDAL();
            MessageBox.Show("Conectado..." );
            // clase DAL departamentos ... objeto que tiene la inforacion del GUI
            oDepartamentosDAL.Agregar(RecuperarInformacion());
            LlenarGrid();
        }
        private DepartamentoBLL RecuperarInformacion()
        {
            DepartamentoBLL oDepartamentoBLL = new DepartamentoBLL();
            int Id = 0; int.TryParse(txtId.Text, out Id);
            oDepartamentoBLL.Id = Id;
            oDepartamentoBLL.Departamento = txtDepartamento.Text;
            return oDepartamentoBLL;
        }

        private void Seleccionar(object sender, DataGridViewCellMouseEventArgs e)
        {
            int indice = e.RowIndex;
            txtId.Text = dgvDepartamentos.Rows[indice].Cells[0].Value.ToString();
            txtDepartamento.Text = dgvDepartamentos.Rows[indice].Cells[1].Value.ToString();
         }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            oDepartamentosDAL.Eliminar(RecuperarInformacion());
            LlenarGrid();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            oDepartamentosDAL.Modificar(RecuperarInformacion());
            LlenarGrid();
        }
        public void LlenarGrid()
        {
            dgvDepartamentos.DataSource = oDepartamentosDAL.MostrarDepartamentos().Tables[0];
        }
    }
}
