using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MonitorArchivos
{
    public partial class Form1 : Form
    {
        string Path = @"D:\Archivos";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fileSystemWatcher1.Path = Path;
            GetFiles();
        }
        private void GetFiles()
        {
            txtArchivos.Text = "";
            string[] lst = Directory.GetFiles(Path);
            foreach (var sfile in lst)
            {
                txtArchivos.Text += sfile + Environment.NewLine;
            }
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            GetFiles();
        }

        private void fileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            GetFiles();
        }
    }
}
