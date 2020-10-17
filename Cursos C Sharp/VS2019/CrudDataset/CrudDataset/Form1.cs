using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrudDataset
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Refresh();
        }
        private void Refresh()
        {
            dsCrudTableAdapters.peopleTableAdapter ta = new dsCrudTableAdapters.peopleTableAdapter();
            dsCrud.peopleDataTable dt = ta.GetData();
            dataGridView1.DataSource = dt;

        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmPeople frm = new FrmPeople();
            frm.ShowDialog();
            Refresh();
        }
        private int? GetId()
        {
            try
            {
                return int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
            }
            catch
            {
                return null;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            int? Id = GetId();
            if (Id != null)
            {

                FrmPeople frm = new FrmPeople(Id);
                frm.ShowDialog();
                Refresh();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int? Id = GetId();
            if (Id != null)
            {
                dsCrudTableAdapters.peopleTableAdapter ta = new dsCrudTableAdapters.peopleTableAdapter();
                ta.Remove((int)Id);
                Refresh();
            }
        }
    }
}
