using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AdminEmpleados.DAL;
using AdminEmpleados.BLL;

namespace AdminEmpleados.PL
{
    public partial class frmEmpleados : Form
    {
        byte[] imagenByte;
        public frmEmpleados()
        {
            InitializeComponent();
        }

        private void frmEmpleados_Load(object sender, EventArgs e)
        {
            DepartamentosDAL objDepartamentosDAL = new DepartamentosDAL();
            cbxDepartamento.DataSource = objDepartamentosDAL.MostrarDepartamentos().Tables[0];
            cbxDepartamento.DisplayMember = "departamento";
            cbxDepartamento.ValueMember = "Id";
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectorImagen = new OpenFileDialog();
            selectorImagen.Title = "Seleccionar imagen";

            if(selectorImagen.ShowDialog() ==DialogResult.OK)
            {
                picFoto.Image = Image.FromStream(selectorImagen.OpenFile());
                MemoryStream memoria = new MemoryStream();
                picFoto.Image.Save(memoria, System.Drawing.Imaging.ImageFormat.Png);

                imagenByte = memoria.ToArray();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            RecolectarDatos();
        }
        public void RecolectarDatos()
        {
            EmpleadosBLL objEmpleados = new EmpleadosBLL();
            int codigoEmpleado = 1;
            int.TryParse(txtId.Text, out codigoEmpleado);

            objEmpleados.Id = codigoEmpleado;
            objEmpleados.NombreEmpleado = txtNombre.Text;
            objEmpleados.PrimerApellido = txtPrimerApellido.Text;
            objEmpleados.SegundoApellido = txtSegundoApellido.Text;
            objEmpleados.Correo = txtCorreo.Text;

            int IdDepertamento = 0;
            int.TryParse(cbxDepartamento.SelectedValue.ToString(), out IdDepertamento);

            objEmpleados.Foto = imagenByte;
        }
    }
}
