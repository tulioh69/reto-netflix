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
            LimpiarEntradas();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // clase DAL departamentos ... objeto que tiene la inforacion del GUI
            oDepartamentosDAL.Agregar(RecuperarInformacion());
            LlenarGrid();
            LimpiarEntradas();
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
            dgvDepartamentos.ClearSelection();
            if (indice >=0)
            {
                txtId.Text = dgvDepartamentos.Rows[indice].Cells[0].Value.ToString();
                txtDepartamento.Text = dgvDepartamentos.Rows[indice].Cells[1].Value.ToString();

                btnAgregar.Enabled = false;
                btnModificar.Enabled = true;
                btnBorrar.Enabled = true;
                btnCancelar.Enabled = true;
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            oDepartamentosDAL.Eliminar(RecuperarInformacion());
            LlenarGrid();
            LimpiarEntradas();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            oDepartamentosDAL.Modificar(RecuperarInformacion());
            LlenarGrid();
            LimpiarEntradas();
        }
        public void LlenarGrid()
        {
            dgvDepartamentos.DataSource = oDepartamentosDAL.MostrarDepartamentos().Tables[0];
            dgvDepartamentos.Columns[0].HeaderText = "Id";
            dgvDepartamentos.Columns[1].HeaderText = "Nombre Departamento";
        }
        public void LimpiarEntradas()
        {
            txtId.Text = "";
            txtDepartamento.Text = "";

            btnAgregar.Enabled = true;
            btnModificar.Enabled = false;
            btnBorrar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarEntradas();
        }
    }
}
