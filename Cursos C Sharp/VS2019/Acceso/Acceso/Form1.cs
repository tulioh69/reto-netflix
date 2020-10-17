using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Acceso.Models;

namespace Acceso
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string sPas = Encrypt.GetSHA256(txtPassword.Text.Trim());
            using (windowsformEntities db = new windowsformEntities())
            {
                var lst = from d in db.user where d.email == txtUser.Text && d.password == sPas select d;
                if (lst.Count() >0 )
                {
                    this.Hide();
                    FrmMain frm = new FrmMain();
                    frm.FormClosed += (s, args) => this.Close();
                    frm.Show();
                }
                else
                {
                    MessageBox.Show("Usuario no Existe");
                }
            }
        }
    }
}
