using LumenWorks.Framework.IO.Csv;
using Microsoft.Office.Interop.Excel;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities.Network;

namespace CompensacionV01
{
    public partial class CompAut : Form
    {
        //txt fechas
        private string sabado = "Saturday";
        private string domingo = "Sunday";
        private string lunes = "Monday";
        //fch comp
        private DateTime fchComp;
        //fch actual
        private DateTime fchActual;
        //retornos funciones
        private long cr = -1;
        private string msj = "";
        //crypto
        string k = null;
        string vi = null;
        //err out
        string err;
        private LoggerBlock loggerBlock;
        public CompAut()
        {
            //Enterprise Logging
            loggerBlock = new LoggerBlock();
            Thread.Sleep(500);

            InitializeComponent();
            txtBoxLogs.AutoSize = true;
            //para permitir que hilos usen los controles
            CheckForIllegalCrossThreadCalls = false;
            loadConfig();

            dtpCompFechActual.Value = DateTime.Now;
            DateTime fchComp = DateTime.Now;
            calcularFechaCompensacion(dtpCompFechActual.Value, ref fchComp, ref msj);
            dtpComp.Value = fchComp;
            //
            txtBDBorrarRegCoincid.Text = "octopus_pci";//octopus_pci_comp
            txtTablaPadre.Text = "Trx_Bco_Austro";
            txtTablaHija.Text = "Trx_Bco_Austro_" + dtpComp.Value.ToString("yyMMdd");
        }
        private void btnCopiarArchivo_Click(object sender, EventArgs e)
        {
            msj = "";
            string pathSrc = ConfigurationManager.AppSettings["pathFldrCompensacion"];
            string fileNameSrc = "test.txt";
            string pathDst = pathSrc + "copia";
            copiarArchivo(pathSrc, fileNameSrc, ref cr, ref msj, pathDst);

        }
        private void copiarArchivo(string pathSrc, string fileNameSrc, ref long codigoRetorno, ref string mensaje, string pathDst = null, string fileNameDst = null)
        {

            string src = Path.Combine(pathSrc, fileNameSrc);

            long lengthSrc = 0;
            string tamSrc = " (0) ";
            long lengthDst = 0;
            string tamDst = " (0)";
            if (File.Exists(src))
            {
                lengthSrc = new System.IO.FileInfo(src).Length;
                tamSrc = " (" + sizeSuffix(lengthSrc) + ")";
            }

            if (pathDst == null && fileNameDst == null && File.Exists(src)) // intento de copiar un archivo con el mismo nombre en la misma carpeta
            {
                codigoRetorno = 444; mensaje = "WARN El archivo: " + src + tamSrc + ", ya existe en: " + pathSrc;
                escribirTraceLog(TraceEventType.Warning, mensaje);
                return;
            }
            if (pathDst == null) { pathDst = pathSrc; }
            if (fileNameDst == null) { fileNameDst = fileNameSrc; }

            string dst = pathDst + "\\" + fileNameDst;
            escribirTraceLog(TraceEventType.Information, " Copiando: " + src + tamSrc + ", en: " + dst);

            if (File.Exists(dst))
            {
                lengthDst = new System.IO.FileInfo(dst).Length;
                tamDst = " (" + sizeSuffix(lengthDst) + ")";
            }
            else
            {
                lengthDst = lengthSrc;
                tamDst = tamSrc;
            }

            try
            {
                if (File.Exists(src)) // existe archivo fuente a copiar
                {
                    if (!Directory.Exists(pathDst)) // NO existe path destino
                    {
                        escribirTraceLog(TraceEventType.Warning, "WARN No existe destino: " + pathDst);
                        escribirTraceLog(TraceEventType.Information, " Creando " + pathDst);
                        Directory.CreateDirectory(pathDst);
                        escribirTraceLog(TraceEventType.Information, "EXITO " + pathDst + " Creado");
                    }

                    if (File.Exists(dst)) // existe archivo en el destino
                    {
                        escribirTraceLog(TraceEventType.Warning, "WARN El archivo: " + fileNameDst + tamDst + " existe en el destino: " + pathDst);
                        //preguntar si deseo reemplazar el existente en el destino
                        DialogResult r = MessageBox.Show("¿" + "Desea reemplazar el archivo " + dst + tamDst + " por " + src + tamSrc + "?", "Confirmar reemplazo", MessageBoxButtons.YesNoCancel);
                        if (r == DialogResult.Yes)
                        {
                            escribirTraceLog(TraceEventType.Warning, "WARN Se reemplazara el archivo " + dst + tamDst + " por " + src + tamSrc);
                            File.Copy(src, dst, true);

                            codigoRetorno = 0; mensaje = "EXITO Archivo " + dst + tamDst + " a sido reemplazado por " + src + tamSrc;
                            escribirTraceLog(TraceEventType.Verbose, mensaje);
                        }
                        else if (r == DialogResult.No)
                        {
                            codigoRetorno = 111; mensaje = " NO Se reemplazo el archivo " + dst + tamDst + " por " + src + tamSrc;
                            escribirTraceLog(TraceEventType.Warning, mensaje);
                        }
                        else if (r == DialogResult.Cancel)
                        {
                            codigoRetorno = 112; mensaje = " Se cancelo la operacion reemplazar el archivo " + dst + tamDst + " por " + src + tamSrc;
                            escribirTraceLog(TraceEventType.Warning, mensaje);
                        }
                        else
                        {

                        }
                    }
                    else // No existe al archivo a copiar en el destino
                    {
                        File.Copy(src, dst);

                        codigoRetorno = 0; mensaje = "EXITO Archivo " + src + tamSrc + " a sido copiado en: " + dst + tamDst;
                        escribirTraceLog(TraceEventType.Information, mensaje);
                    }
                }
                else
                {
                    codigoRetorno = 222; mensaje = "ERROR No existe el archivo a copiar: " + src + tamSrc;
                    escribirTraceLog(TraceEventType.Error, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
                return;
            }
        }
        private void obtenerArchivosEnPath(string path, ref int numArchs, ref FileInfo[] nombreArchs)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                numArchs = 0;
                if (Directory.Exists(path))
                {
                    nombreArchs = di.GetFiles();
                    numArchs = nombreArchs.Length;
                }
                else
                {
                    nombreArchs = null;
                }
                msj = "[ " + numArchs + " ]" + " Archivos en path " + path;
                escribirTraceLog(TraceEventType.Information, msj);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void crearCarpeta(string path, string folderName, ref string pathCarpetaCreada, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string absolutePath = Path.Combine(path, folderName);
                escribirTraceLog(TraceEventType.Information, " Creando Directorio: " + absolutePath);
                if (Directory.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "WARN Directorio ya Existe: " + absolutePath;
                    pathCarpetaCreada = absolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(absolutePath);
                    codigoRetorno = 0; mensaje = "EXITO directorio creado: " + absolutePath;
                    pathCarpetaCreada = di.FullName;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }

            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                pathCarpetaCreada = null;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private void copiarCarpeta(string pathSrc, string folderNameSrc, string pathDst, string folderNameDst, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string srcAbsolutePath = Path.Combine(pathSrc, folderNameSrc);
                string dstAbsolutePath = Path.Combine(pathDst, folderNameDst);

                escribirTraceLog(TraceEventType.Information, " Copiando Directorio, DE: " + srcAbsolutePath + ", A: " + dstAbsolutePath);
                if (Directory.Exists(dstAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "WARN Directorio con el nombre " + dstAbsolutePath + " ya Existe";
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    Directory.CreateDirectory(dstAbsolutePath);
                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(srcAbsolutePath, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(srcAbsolutePath, dstAbsolutePath));
                    }
                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(srcAbsolutePath, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(srcAbsolutePath, dstAbsolutePath), true);
                    }
                    codigoRetorno = 0; mensaje = "EXITO Carpeta copiada DE: " + srcAbsolutePath + " A: " + dstAbsolutePath;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private void renombrarCarpeta(string path, string folderName, string newFolderName, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string oldAbsolutePath = Path.Combine(path, folderName);
                string newAbsolutePath = Path.Combine(path, newFolderName);

                escribirTraceLog(TraceEventType.Information, " Renombrando Directorio, DE: " + oldAbsolutePath + ", A: " + newAbsolutePath);
                if (Directory.Exists(newAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "WARN nombre nuevo del Directorio ya Existe: " + newAbsolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    if (!Directory.Exists(oldAbsolutePath))
                    {
                        codigoRetorno = 111; mensaje = "ERROR Directorio a renombrar NO Existe: " + oldAbsolutePath + ", Creando";
                        escribirTraceLog(TraceEventType.Information, mensaje);
                        string pathDirCre = "";
                        crearCarpeta(path, folderName, ref pathDirCre, ref cr, ref msj);
                    }
                    Directory.Move(oldAbsolutePath, newAbsolutePath);
                    codigoRetorno = 0; mensaje = "EXITO directorio " + oldAbsolutePath + " renombrado A: " + newAbsolutePath;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private bool isDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        private void eliminarCarpeta(string path, string folderName, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string absolutePath = Path.Combine(path, folderName);
                escribirTraceLog(TraceEventType.Information, " Eliminando Directorio: " + absolutePath);
                if (!Directory.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Directorio NO Existe: " + absolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    if (isDirectoryEmpty(absolutePath))
                    {
                        Directory.Delete(absolutePath);
                        codigoRetorno = 0; mensaje = "EXITO directorio eliminado: " + absolutePath;
                        escribirTraceLog(TraceEventType.Information, mensaje);
                    }
                    else
                    {
                        DialogResult r = MessageBox.Show("La carpeta " + absolutePath + " no esta vacia,  ¿Desea eliminar TODO su contenido?", "Confirmar", MessageBoxButtons.YesNoCancel);
                        if (r == DialogResult.Yes)
                        {
                            escribirTraceLog(TraceEventType.Warning, "WARN Se eliminara la carpeta " + absolutePath);
                            Directory.Delete(absolutePath, true);
                            codigoRetorno = 0; mensaje = "EXITO Carpeta Eliminada: " + absolutePath;
                            escribirTraceLog(TraceEventType.Information, mensaje);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private void crearArchivo(string path, string fileName, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string absolutePath = Path.Combine(path, fileName);
                escribirTraceLog(TraceEventType.Information, " Creando Archivo: " + absolutePath);
                if (File.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "WARN Archivo ya Existe: " + absolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                }
                else
                {
                    File.Create(absolutePath).Close();
                    codigoRetorno = 0; mensaje = "EXITO Archivo creado: " + absolutePath;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private void renombrarArchivo(string path, string fileName, string newFileName, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string oldAbsolutePath = Path.Combine(path, fileName);
                string newAbsolutePath = Path.Combine(path, newFileName);

                escribirTraceLog(TraceEventType.Information, " Renombrando Archivo, DE: " + oldAbsolutePath + ", A: " + newAbsolutePath);
                if (File.Exists(newAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "WARN Archivo ya Existe: " + newAbsolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    File.Move(oldAbsolutePath, newAbsolutePath);
                    codigoRetorno = 0; mensaje = "EXITO archivo renombrado A: " + newAbsolutePath;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        private void eliminarArchivo(string path, string fileName, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string absolutePath = Path.Combine(path, fileName);
                escribirTraceLog(TraceEventType.Information, " Eliminando Archivo: " + absolutePath);
                if (!File.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Archivo NO Existe: " + absolutePath;
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                else
                {
                    File.Delete(absolutePath);
                    codigoRetorno = 0; mensaje = "EXITO Archivo Eliminado: " + absolutePath;
                    escribirTraceLog(TraceEventType.Information, mensaje);
                }
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        public void comprimirArchivosEnZip(string pathNombreZip, FileInfo[] listArchivos, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                // Crear o sobreescribir el archivo zip
                FileStream outZipFile = File.Create(pathNombreZip); outZipFile.Close();
                //agregar archivos al .ZIP
                if (listArchivos == null || listArchivos.Length == 0)
                {
                    codigoRetorno = 444;
                    mensaje = "ERROR Nombre de zip o ruta para crear zip vacias";
                    return;
                }
                using (var zipArchive = ZipFile.Open(pathNombreZip, ZipArchiveMode.Update))
                {
                    foreach (FileInfo fileInfo in listArchivos)
                    {
                        zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                }
                long lengthSrc = new System.IO.FileInfo(pathNombreZip).Length;
                string tamSrc = " (" + sizeSuffix(lengthSrc) + ")";

                mensaje = "EXITO " + listArchivos.Count() + " Archivos comprimidos en " + pathNombreZip + tamSrc;
                escribirTraceLog(TraceEventType.Information, mensaje);
                codigoRetorno = 0;
            }
            catch (Exception ex)
            {
                codigoRetorno = 666; mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        public void extraerArchivosDeZip(string pathSrcNombreZip, string pathDstParaExtraer, ref string mensaje)
        {
            try
            {

                if (!File.Exists(pathSrcNombreZip))
                {
                    mensaje = "ERROR Archivo comprimido " + pathSrcNombreZip + " NO existe";
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                using (var zipArchive = ZipFile.Open(pathSrcNombreZip, ZipArchiveMode.Update))
                {
                    zipArchive.ExtractToDirectory(pathDstParaExtraer);
                }
                mensaje = "EXITO Archivos extraidos en " + pathDstParaExtraer + ", desde " + pathSrcNombreZip;
                escribirTraceLog(TraceEventType.Information, mensaje);
            }
            catch (Exception ex)
            {
                mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
        }
        public void comprimirCarpetaEnZip(string pathCarpetaParaZipear, ref long codigoRetorno, ref string mensaje, string pathNombreZip = null)
        {

            if (pathCarpetaParaZipear == null || !Directory.Exists(pathCarpetaParaZipear))
            {
                codigoRetorno = 444;
                mensaje = "ERROR Directorio a comprimir NO existe " + pathCarpetaParaZipear;
                escribirTraceLog(TraceEventType.Warning, mensaje);
                return;
            }
            if (pathNombreZip == null)
            {
                DirectoryInfo infoDir = new DirectoryInfo(pathCarpetaParaZipear);
                string currentFileName = infoDir.FullName;
                string nombreRel = infoDir.Name;
                string padre = System.IO.Directory.GetParent(pathCarpetaParaZipear).FullName;
                string nombreCrea = nombreRel + ".zip";
                pathNombreZip = System.IO.Path.Combine(padre, nombreCrea);
            }

            DirectoryInfo pathParaZip = new DirectoryInfo(pathCarpetaParaZipear);
            // Crear o sobreescribir el archivo zip
            FileStream outZipFile = File.Create(pathNombreZip); outZipFile.Close();
            //agregar archivos al .ZIP desde el directorio
            using (var zipArchive = ZipFile.Open(pathNombreZip, ZipArchiveMode.Update))
            {
                foreach (FileInfo fileInfo in pathParaZip.GetFiles())
                {
                    if (fileInfo.Extension != ".zip")
                    {
                        zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                }
            }
            codigoRetorno = 0;
            mensaje = "EXITO Carpeta comprimida";
            escribirTraceLog(TraceEventType.Information, mensaje);
        }

        public void eliminarArchivosZIPDeDirectorio(string directorio, ref long codigoRetorno, ref string mensaje)
        {
            try
            {
                if (System.IO.Directory.Exists(directorio))
                {
                    System.IO.DirectoryInfo directoryinfo = new System.IO.DirectoryInfo(directorio);
                    System.IO.DirectoryInfo[] directInfoList = directoryinfo.GetDirectories();
                    foreach (System.IO.DirectoryInfo dirInfo in directInfoList)
                    {
                        System.IO.Directory.Delete(dirInfo.FullName, true);
                    }
                    System.IO.FileInfo[] fileInfo = directoryinfo.GetFiles();
                    foreach (System.IO.FileInfo fileinfo in fileInfo)
                    {
                        System.IO.File.Delete(fileinfo.FullName);
                    }
                }
                else
                {
                    codigoRetorno = 444;
                    mensaje = "ERROR Directorio de zip o ruta no existe";
                    escribirTraceLog(TraceEventType.Warning, mensaje);
                    return;
                }
                codigoRetorno = 0;
            }
            catch (Exception ex)
            {
                codigoRetorno = 666;
                mensaje = ex.Message;
                escribirTraceLog(TraceEventType.Information, mensaje);
            }
        }

        private bool RemoteServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            //return sslPolicyErrors == SslPolicyErrors.None || !this.validarcertificadodigital;
        }

        //correodominio=null
        //correonombreusuario = azure_a82b80688e8c3267671082f24e5297e9@azure.com
        //correopassword = 040994.-stewoz
        //correoportsmtp = 587
        //correorequiereautentificacion = 1
        //correosmtphost = smtp.sendgrid.net
        //correousarssl = 1
        //correovalidarcertificadodigital = 0
        public void enviarCorreo(string from, string to, string cc, string subject, string body, ArrayList attachments, string smtpHost, int portSmtp, bool usarssl, bool validarcertificadodigital, bool requiereautentificacion, string dominio, string nombreusuario, string password, ref long codigoRetorno, ref string mensaje)
        {

            mensaje = " Enviando correo DE: " + from + ", PARA: " + to;
            escribirTraceLog(TraceEventType.Information, mensaje);
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(from.Trim()))
            {
                codigoRetorno = 444;
                mensaje = "ERROR: remitente no puede ser vacio";
                escribirTraceLog(TraceEventType.Warning, mensaje);
                return;
            }
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(to.Trim()))
            {
                codigoRetorno = 445;
                mensaje = "ERROR: Destinatario no puede ser vacio";
                escribirTraceLog(TraceEventType.Warning, mensaje);
                return;
            }
            MailMessage mailMessage;
            MailMessage message = mailMessage = new MailMessage();
            try
            {
                if (usarssl)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(
                            (sender, certificate, chain, sslPolicyErrors)
                                =>
                            { return sslPolicyErrors == SslPolicyErrors.None || !validarcertificadodigital; }
                        );
                }
                message.From = new MailAddress(from);
                //
                string str = to;
                char[] chArray = new char[1] { ';' };
                foreach (string addresses in str.Split(chArray))
                {
                    message.To.Add(addresses);
                }
                //
                string strCC = cc;
                char[] chArrayCC = new char[1] { ';' };
                foreach (string addressesCC in strCC.Split(chArray))
                {
                    message.CC.Add(addressesCC);
                }
                //
                message.Subject = subject.Trim();
                message.Priority = MailPriority.Normal;
                message.IsBodyHtml = false;
                message.Body = body;
                if (attachments != null)
                {
                    for (int index = 0; index < attachments.Count; ++index)
                    {
                        if ((Attachment)attachments[index] != null)
                        {
                            message.Attachments.Add((Attachment)attachments[index]);
                        }
                    }
                }
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpHost;
                if ((uint)portSmtp > 0U)
                {
                    smtpClient.Port = portSmtp;
                }
                smtpClient.EnableSsl = usarssl;
                if (requiereautentificacion && !string.IsNullOrEmpty(nombreusuario))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = string.IsNullOrEmpty(dominio) ? (ICredentialsByHost)new NetworkCredential(nombreusuario, password) : (ICredentialsByHost)new NetworkCredential(nombreusuario, password, dominio);
                }
                smtpClient.Send(message);
                codigoRetorno = 0;
                mensaje = "Correo Enviado";
                escribirTraceLog(TraceEventType.Information, mensaje);
            }
            catch (Exception ex)
            {
                for (int index = message.Attachments.Count - 1; index >= 0; --index)
                {
                    message.Attachments.RemoveAt(index);
                }
                codigoRetorno = 666;
                mensaje = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, mensaje);
            }
            finally
            {
                if (usarssl)
                {
                    ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)null;
                }
            }
        }

        static readonly string[] sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string sizeSuffix(Int64 value, int decimalPlaces = 0)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + sizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, sizeSuffixes[mag]);
        }
        private void loadConfig()
        {
            try
            {

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void escribirTraceLog(TraceEventType traceEventType, string message)
        {
            loggerBlock.getLogWriter.Write(message, "General", 5, 2000, traceEventType, "Compensacion");
            //add log to textbox
            //if (message.Length > 0) { txtBoxLogs.AppendText(DateTime.Now.ToString() + " " + message); txtBoxLogs.AppendText(Environment.NewLine); txtBoxLogs.AppendText(Environment.NewLine); }
            if (message.Length > 0)
            {
                string msjShow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " " + message;
                if (traceEventType == TraceEventType.Information
                    || traceEventType == TraceEventType.Verbose)
                {
                    appendText(msjShow, Color.DarkGreen);
                }
                else if (traceEventType == TraceEventType.Error
                    || traceEventType == TraceEventType.Critical
                    )
                {
                    appendText(msjShow, Color.DarkRed);
                }
                else if (traceEventType == TraceEventType.Warning)
                {
                    appendText(msjShow, Color.DarkOrange);
                }
                else
                {
                    appendText(msjShow, Color.Brown);
                }
                appendText(Environment.NewLine, Color.White);
                appendText(Environment.NewLine, Color.White);
            }
        }
        public void appendText(string text, Color color)
        {
            txtBoxLogs.SelectionStart = txtBoxLogs.TextLength;
            txtBoxLogs.SelectionLength = 0;

            txtBoxLogs.SelectionColor = color;
            txtBoxLogs.AppendText(text);
            txtBoxLogs.SelectionColor = txtBoxLogs.ForeColor;
            // set the current caret position to the end
            txtBoxLogs.SelectionStart = txtBoxLogs.Text.Length;
            // scroll it automatically
            txtBoxLogs.ScrollToCaret();
        }
        private void btnCrearCarpeta_Click(object sender, EventArgs e)
        {
            escribirTraceLog(TraceEventType.Information, "Creando carpeta");
            msj = "";
            string pathCarpetaCreada = "";
            crearCarpeta("D:\\compensacion\\", "carpeta", ref pathCarpetaCreada, ref cr, ref msj);

        }

        private void btnRenombrarCarpeta_Click(object sender, EventArgs e)
        {
            msj = "";
            Random rnd = new Random();
            int num = rnd.Next(1, 2);  // creates a number between 1 and 2
            renombrarCarpeta("D:\\compensacion\\", "carpeta", "carpeta" + num.ToString(), ref cr, ref msj);

        }

        private void btnRenombrarArchivo_Click(object sender, EventArgs e)
        {
            msj = "";
            Random rnd = new Random();
            int num = rnd.Next(1, 2);  // creates a number between 1 and 2
            renombrarArchivo("D:\\compensacion\\", "test.txt", "test" + num.ToString() + ".txt", ref cr, ref msj);

        }

        private void btnCreandoArchivo_Click(object sender, EventArgs e)
        {
            msj = "";
            crearArchivo("D:\\compensacion\\", "test.txt", ref cr, ref msj);

        }

        private void btnComprimirCarpeta_Click(object sender, EventArgs e)
        {
            msj = "";
            comprimirCarpetaEnZip("D:\\compensacion\\carpeta1", ref cr, ref msj);

        }

        private void btnCopiarCarpeta_Click(object sender, EventArgs e)
        {
            msj = "";
            copiarCarpeta("D:\\compensacion\\", "carpeta1", "D:\\compensacion\\", "carpeta1Copiada", ref cr, ref msj);

        }

        private void btnEliminarArchivo_Click(object sender, EventArgs e)
        {
            msj = "";
            eliminarArchivo("D:\\compensacion\\", "test.txt", ref cr, ref msj);

        }

        private void btnEliminarCarpeta_Click(object sender, EventArgs e)
        {
            msj = "";
            eliminarCarpeta("D:\\compensacion\\", "carpeta1", ref cr, ref msj);

        }

        private void btnEnviarCorreo_Click(object sender, EventArgs e)
        {
            enviarMailConAdjunto();
        }
        private void enviarMailConAdjunto()
        {
            //dominio=null
            //nombreusuario = azure_a82b80688e8c3267671082f24e5297e9@azure.com
            //password = 040994.-stewoz
            //portsmtp = 587
            //requiereautentificacion = 1
            //smtphost = smtp.sendgrid.net
            //usarssl = 1
            //validarcertificadodigital = 0
            msj = "";
            ArrayList adjuntos = new ArrayList();
            string from = ConfigurationManager.AppSettings["from"];
            string to = ConfigurationManager.AppSettings["to"];//separar con ";"
            string cc = ConfigurationManager.AppSettings["cc"];//separar con ";"

            string subject = "Compensacion del " + dtpComp.Value.ToString("dd-MM-yyyy") + " !!!";
            string body = "Estimados, se adjunta resultado de la compensacion del " + dtpComp.Value.ToString("dd-MM-yyyy") + "\nSaludos Cordiales,\nMarco Tulio Romero.";
            string smtpHost = ConfigurationManager.AppSettings["smtpHost"]; //"smtp.sendgrid.net",
            int port = int.Parse(ConfigurationManager.AppSettings["port"]); //587
            string dominio = ConfigurationManager.AppSettings["dominio"];
            bool usarSSL = bool.Parse(ConfigurationManager.AppSettings["usarSSL"]);
            bool validarCertificadoDigital = bool.Parse(ConfigurationManager.AppSettings["validarCertificadoDigital"]);
            bool requiereAutentificacion = bool.Parse(ConfigurationManager.AppSettings["requiereAutentificacion"]);
            string user = ConfigurationManager.AppSettings["user"];////azure_a82b80688e8c3267671082f24e5297e9@azure.com
            string pwd = ConfigurationManager.AppSettings["pwd"];//"040994.-stewoz"
            // Create  the file attachment for this email message.
            string fileAUSTP = "D:\\Compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp" + "\\AUSTP" + "\\CompensacionAUSTP" + dtpComp.Value.ToString("yyMMdd") + ConfigurationManager.AppSettings["extArchivoComprimido"].ToString();
            string fileAUSTPC = "D:\\Compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp" + "\\AUSTPC" + "\\CompensacionAUSTPC" + dtpComp.Value.ToString("yyMMdd") + ConfigurationManager.AppSettings["extArchivoComprimido"].ToString();

            Attachment attachmentAUSTP = new Attachment(fileAUSTP, MediaTypeNames.Application.Zip);
            attachmentAUSTP.Name = new FileInfo(fileAUSTP).Name;
            // Add time stamp information for the file.
            ContentDisposition dispositionAUSTP = attachmentAUSTP.ContentDisposition;
            dispositionAUSTP.CreationDate = System.IO.File.GetCreationTime(fileAUSTP);
            dispositionAUSTP.ModificationDate = System.IO.File.GetLastWriteTime(fileAUSTP);
            dispositionAUSTP.ReadDate = System.IO.File.GetLastAccessTime(fileAUSTP);
            //
            Attachment attachmentAUSTPC = new Attachment(fileAUSTPC, MediaTypeNames.Application.Zip);
            attachmentAUSTPC.Name = new FileInfo(fileAUSTPC).Name;
            // Add time stamp information for the file.
            ContentDisposition dispositionAUSTPC = attachmentAUSTPC.ContentDisposition;
            dispositionAUSTPC.CreationDate = System.IO.File.GetCreationTime(fileAUSTP);
            dispositionAUSTPC.ModificationDate = System.IO.File.GetLastWriteTime(fileAUSTP);
            dispositionAUSTPC.ReadDate = System.IO.File.GetLastAccessTime(fileAUSTP);
            // Add the file attachment to this email message.
            adjuntos.Add(attachmentAUSTP);
            adjuntos.Add(attachmentAUSTPC);
            enviarCorreo(from, to, cc, subject, body, adjuntos, smtpHost, port, usarSSL, validarCertificadoDigital, requiereAutentificacion, dominio, user, pwd, ref cr, ref msj);
        }

        private void tabCtrlEjecComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tabSelected = tabCtrlEjecComp.SelectedTab.Text;
            if (tabSelected == "tabPage5")
            {
                dtpCompFechActual.Value = DateTime.Now;
                DateTime fchComp = DateTime.Now;
                calcularFechaCompensacion(dtpCompFechActual.Value, ref fchComp, ref msj);
                dtpComp.Value = fchComp;
            }
        }

        private void validarFechasSeleccionadas(DateTime fchCompensacion, DateTime fchActual, ref bool valido, ref string msj)
        {
            try
            {
                if (fchCompensacion > fchActual)
                {
                    valido = false;
                    msj = "ERROR la fecha compensacion es mayor a la fecha actual";
                }
                //
                if (fchActual.DayOfWeek.ToString() == sabado)
                {
                    valido = false;
                    msj = "ERROR el dia actual es sabado";
                }
                if (fchActual.DayOfWeek.ToString() == domingo)
                {
                    valido = false;
                    msj = "ERROR el dia actual es domingo";
                }
                //
                if (fchCompensacion.DayOfWeek.ToString() == sabado)
                {
                    valido = false;
                    msj = "ERROR el dia de la compensacion es sabado";
                }
                if (fchCompensacion.DayOfWeek.ToString() == domingo)
                {
                    valido = false;
                    msj = "ERROR el dia de la compensacion es domingo";
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
            }
        }
        private void calcularFechaCompensacion(DateTime fchActual, ref DateTime fchComp, ref string msj)
        {
            try
            {
                if (fchActual.DayOfWeek.ToString() == sabado)
                {
                    fchComp = fchActual.AddDays(-1);
                }
                else if (fchActual.DayOfWeek.ToString() == domingo)
                {
                    fchComp = fchActual.AddDays(-2);
                }
                else if (fchActual.DayOfWeek.ToString() == lunes)
                {
                    fchComp = fchActual.AddDays(-3);
                }
                else
                {
                    fchComp = fchActual.AddDays(-1);
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
            }
        }
        private void xlsxToTextDelimitedTab(string absolutePathSrcXlsx, string absolutePathDstTxt, ref string msj)
        {
            Microsoft.Office.Interop.Excel.Application myExcel = null;
            Microsoft.Office.Interop.Excel.Workbook myWorkbook = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;
            try
            {

                absolutePathSrcXlsx = Path.GetFullPath(absolutePathSrcXlsx);
                absolutePathDstTxt = Path.GetFullPath(absolutePathDstTxt);
                //string pathSrcXlsx = "D:\\compensacion\\2019-11-05\\gvwResultados01.xlsx";
                //string pathDstTxt = "D:\\compensacion\\2019-11-05\\gvwResultados01.tx;
                myExcel = new Microsoft.Office.Interop.Excel.Application();

                //System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US",false);
                //customCulture.NumberFormat.NumberDecimalSeparator = ",";
                //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

                myExcel.Workbooks.Open(absolutePathSrcXlsx);
                myWorkbook = myExcel.ActiveWorkbook;

                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)myWorkbook.Worksheets[1];

                Range rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1];
                rg.EntireColumn.NumberFormat = "DD/MM/YYYY";

                rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[21, 21];
                rg.EntireColumn.NumberFormat = "DD/MM/YYYY";

                rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[7, 7];
                rg.EntireColumn.NumberFormat = "General";

                //myWorkbook.SaveAs(pathDstTxt, Microsoft.Office.Interop.Excel.XlFileFormat.xlTextWindows,null,null, false, false, 
                //    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, 
                //    Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true);

                myWorkbook.SaveAs(absolutePathDstTxt, Microsoft.Office.Interop.Excel.XlFileFormat.xlTextWindows, null, null, false, false,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    null, true);

                myWorkbook.Close(false);
                myExcel.Quit();
                escribirTraceLog(TraceEventType.Information, "EXITO excel interop XLSX a TXT ejecutado: SRC: " + absolutePathSrcXlsx + ", DST: " + absolutePathDstTxt);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
                if (myWorkbook != null) { myWorkbook.Close(false); }
                if (myExcel != null) { myExcel.Quit(); }
            }
        }
        private void correrNuevoProceso(string fileName, string arguments, ref int idProcess, ref Process procesoCreado, ref string msj)
        {
            try
            {
                //string fileName = "cmd.exe";
                //string arguments = "/C notepad";
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden; //para ocultar la ventana cmd
                startInfo.FileName = fileName;
                startInfo.Arguments = arguments;
                process.StartInfo = startInfo;
                process.Start();
                idProcess = process.Id;
                procesoCreado = process;
                escribirTraceLog(TraceEventType.Information, " Proceso creado :" + " filename, " + fileName + ", args:" + arguments + ", Id:" + process.Id + ", ProcessName:" + process.ProcessName);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void killProcess(Process process, ref string msj)
        {
            try
            {
                process.Kill();
                escribirTraceLog(TraceEventType.Information, " Proceso detenido: " + "Id, " + process.Id + ", Process Name: " + process.ProcessName);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void killProcessById(int idProcess, ref string msj)
        {
            try
            {
                Process[] process = Process.GetProcesses();
                //Process current = Process.GetCurrentProcess();
                bool pk = false;
                foreach (Process p in process)
                {
                    if (p.Id == idProcess)
                    {
                        p.Kill();
                        pk = true;
                    }
                }
                if (pk)
                {
                    escribirTraceLog(TraceEventType.Information, " Proceso detenido: " + "Id, " + idProcess);
                }
                else
                {
                    escribirTraceLog(TraceEventType.Information, " Error deteniendo proceso: " + "Id, " + idProcess);
                }

            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void killProcessByName(string processName, ref string msj)
        {
            try
            {
                Process[] process = Process.GetProcessesByName(processName);
                //Process current = Process.GetCurrentProcess();
                bool pk = false;
                foreach (Process p in process)
                {
                    if (p.ProcessName == processName)
                    {
                        p.Kill();
                        pk = true;
                    }
                }
                if (pk)
                {
                    escribirTraceLog(TraceEventType.Information, " Proceso detenido: " + "processName, " + processName);
                }
                else
                {
                    escribirTraceLog(TraceEventType.Information, " Error deteniendo proceso: " + "processName, " + processName);
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }

        }

        private void contarRegistroDeExcelJob(object absolutePathSrcXlsxObj)
        {
            Microsoft.Office.Interop.Excel.Application myExcel = null;
            Microsoft.Office.Interop.Excel.Workbook myWorkbook = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;
            try
            {
                btnRun.Enabled = false;
                string absolutePathSrcXlsx = "";
                try
                {
                    absolutePathSrcXlsx = (string)absolutePathSrcXlsxObj;
                }
                catch (Exception ex) { return; }
                if (string.IsNullOrEmpty(absolutePathSrcXlsx)) { return; }
                absolutePathSrcXlsx = Path.GetFullPath(absolutePathSrcXlsx);
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(absolutePathSrcXlsx);
                myWorkbook = myExcel.ActiveWorkbook;
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)myWorkbook.Worksheets[1];
                Range rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1];
                int numRegs = -1; //para omitir contar la cabecera
                string numRegMsj = "";
                for (int r = 1; r <= rg.EntireColumn.Rows.Count; r++)
                {
                    rg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[r, 1];
                    dynamic valorCeldaRFecha = rg.Value;
                    if (valorCeldaRFecha != null)
                    {
                        if (!string.IsNullOrEmpty(valorCeldaRFecha.ToString()))
                        {
                            numRegs++;
                        }
                        else
                        {
                            myWorkbook.Close(false);
                            myExcel.Quit();
                            numRegMsj = absolutePathSrcXlsx + ", #Registros: " + numRegs;
                            escribirTraceLog(TraceEventType.Information, numRegMsj);
                            return;
                        }
                    }
                    else
                    {
                        myWorkbook.Close(false);
                        myExcel.Quit();
                        numRegMsj = absolutePathSrcXlsx + ", #Registros: " + numRegs;
                        escribirTraceLog(TraceEventType.Information, numRegMsj);
                        return;
                    }
                }
                numRegMsj = absolutePathSrcXlsx + ", #Registros: " + numRegs;
                escribirTraceLog(TraceEventType.Information, numRegMsj);
                myWorkbook.Close(false);
                myExcel.Quit();
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
                if (myWorkbook != null) { myWorkbook.Close(false); }
                if (myExcel != null) { myExcel.Quit(); }
            }
            finally
            {
                btnRun.Enabled = true;
            }
        }
        private void abrirArchivo(ref string msj, ref string absolutePathFile, ref string fileName, string filter = null)
        {
            try
            {
                if (filter == null)
                {
                    //filter ="Excel Files (*.xlsx)|*.xlsx"
                    filter = "All files (*.*)|*.*";
                }

                absolutePathFile = "";
                fileName = "";
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "Seleccionar un Archivo";
                openDialog.Filter = filter;
                openDialog.Multiselect = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    absolutePathFile = openDialog.FileName;
                    fileName = Path.GetFileName(absolutePathFile);
                }
                long lengthSrc = new System.IO.FileInfo(absolutePathFile).Length;
                string tamSrc = " (" + sizeSuffix(lengthSrc) + ")";

                msj = " Archivo Seleccionado " + absolutePathFile + tamSrc;
                escribirTraceLog(TraceEventType.Information, msj);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
            }
        }
        //*cargar data*//
        private void importarDataToSqlServerBD(ref string msj)
        {
            try
            {

                string dataImportar = "D:\\compensacion\\2019-11-05\\gvwResultados01.txt";


                escribirTraceLog(TraceEventType.Information, " data cargada a la base SRC: " + dataImportar);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
            }
        }

        /*
         * Imports data to the database with SqlBulkCopy.
         * This method doesn't use a temporary dataset, it loads
         * data immediately from the ODBC connection
         */

        //VALIDAR NO DUPLICAR TABLA DEL DIA
        private void saveToDatabaseDirectly(string sqlConnString, string dbq, string fileName, string tableOwner, string tableName, ref string msj)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(sqlConnString);


                //string dbq = "D:\\compensacion\\" + dtpComp.Value.ToString("yyyy") + "-" + dtpComp.Value.ToString("MM") + "-" + dtpComp.Value.ToString("dd") + "\\";
                //string fileName = "gvwResultados01.txt";
                // Creates and opens an ODBC connection

                DialogResult r = MessageBox.Show("¿" + "Cargar archivo " + fileName + " sobre la tabla " + tableName + " en la BD " + sqlConn.Database + "?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (r == DialogResult.Yes)
                {
                    escribirTraceLog(TraceEventType.Information, "INFO Se cargara el archivo " + fileName + " sobre la tabla " + tableName + " en la BD " + sqlConn.Database);
                    string strConnString = "Driver={Microsoft Text Driver (*.txt; *.csv)}; " +
                    "Dbq=" + dbq.Trim() + ";" +
                    "Extensions=asc,csv,tab,txt;" +
                    "Persist Security Info=False";

                    string sql_select;
                    //OdbcConnection conn = new OdbcConnection();
                    //conn.ConnectionString = strConnString.Trim();
                    //conn.Open();
                    System.Data.DataTable dtHead = new System.Data.DataTable();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int rowCount = 0;
                    StreamReader sr = new StreamReader(Path.Combine(dbq, fileName));
                    using (CsvReader csv = new CsvReader(sr, false, '\t'))
                    {
                        dtHead.Load(csv);
                        // Creates a new and empty table in the sql database
                        bool ret = createTableInDatabase(dtHead, tableOwner, tableName, sqlConnString);
                        dtHead.Clear();
                        dtHead = null;
                        //
                    }
                    sr = new StreamReader(Path.Combine(dbq, fileName));
                    using (CsvReader csv = new CsvReader(sr, false, '\t'))
                    {
                        // Copies all rows to the database from the data reader.
                        using (SqlBulkCopy bc = new SqlBulkCopy(sqlConnString))
                        {
                            dt.Load(csv);
                            dt.AcceptChanges();
                            dt.Rows[0].Delete(); // remove header
                            dt.AcceptChanges();
                            rowCount = dt.Rows.Count;

                            foreach (DataColumn dc in dt.Columns)
                            {
                                dc.ReadOnly = false;
                            }

                            // replace dor by comma in RValor column 7 (6 from 0) and NULL by "" (column 11) and NULL RAutoriza(column 19) by ""
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string RValor = dt.Rows[i][6].ToString();
                                RValor = RValor.Replace(".", ",");
                                dt.Rows[i][6] = RValor;

                                string Reversado = dt.Rows[i][11] != null ? dt.Rows[i][11].ToString() : "";
                                dt.Rows[i][11] = Reversado;

                                string RAutoriza = dt.Rows[i][19] != null ? dt.Rows[i][19].ToString() : "";
                                dt.Rows[i][19] = RAutoriza;
                            }

                            dt.AcceptChanges();
                            bc.DestinationTableName = "[" + tableOwner + "].[" + tableName + "]";
                            bc.NotifyAfter = 10000;
                            bc.WriteToServer(dt);
                            bc.Close();
                        }
                        dt.Clear();
                        dt = null;
                        // Writes the number of imported rows to the form
                        string notif = "Imported: " + rowCount.ToString() + "/" + rowCount.ToString() + " row(s)";
                        msj = "EXITO carga data en BD " + sqlConn.Database + ", Tabla: " + tableOwner + "." + tableName + " exitosa: " + notif;
                        escribirTraceLog(TraceEventType.Information, msj);
                    }
                }
                else if (r == DialogResult.No)
                {
                    escribirTraceLog(TraceEventType.Information, "INFO No se cargara el archivo " + fileName + " sobre la tabla " + tableName + " en la BD " + sqlConn.Database);
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION importando data en BD: " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
            }
        }

        /*
         * Generates the create table command using the schema table, and
         * runs it in the SQL database.
         */
        private bool createTableInDatabase(System.Data.DataTable dtSchemaTable, string tableOwner, string tableName, string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string mensaje = "";
            bool exit = false;
            try
            {
                // Generates the create table command.
                // The first column of schema table contains the column names.
                // The data type is [varchar](50) NULL in all columns.
                if (tableExistsInDatabase(connectionString, tableOwner, tableName))
                {
                    DialogResult r = MessageBox.Show("¿" + "Desea reemplazar la tabla " + tableName + " en la BD " + conn.Database + "?", "Confirmar reemplazo", MessageBoxButtons.YesNoCancel);
                    if (r == DialogResult.Yes)
                    {
                        escribirTraceLog(TraceEventType.Warning, "WARN Se reemplazara la tabla " + tableName + " en la BD " + conn.Database);
                        //eliminar para volver a crear
                        string msjElim = "";
                        dropTableFromDatabase(connectionString, tableOwner, tableName, ref msjElim);
                        //crear
                        string ctStr = "CREATE TABLE [" + tableOwner + "].[" + tableName + "](\r\n";
                        for (int i = 0; i < dtSchemaTable.Columns.Count; i++)
                        {
                            ctStr += "  [" + dtSchemaTable.Rows[0][i].ToString() + "][varchar](50) NULL";

                            if (i < dtSchemaTable.Columns.Count)
                            {
                                ctStr += ",";
                            }
                            ctStr += "\r\n";
                        }
                        ctStr += ")";
                        // Runs the SQL command to make the destination table.		
                        SqlCommand command = conn.CreateCommand();
                        command.CommandText = ctStr;
                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();
                        escribirTraceLog(TraceEventType.Information, " tabla " + tableOwner + "." + tableName + " creada en BD: " + conn.Database.ToString());
                        exit = true;
                    }
                    else if (r == DialogResult.No)
                    {
                        mensaje = " usuario decidio NO reemplazar la tabla " + tableName + " en la BD " + conn.Database;
                        escribirTraceLog(TraceEventType.Warning, mensaje);
                        exit = false;
                    }
                    else if (r == DialogResult.Cancel)
                    {
                        mensaje = " Se CANCELO la operacion reemplazar Tabla " + tableName + " en la BD " + conn.Database;
                        escribirTraceLog(TraceEventType.Warning, mensaje);
                        exit = false;
                    }
                    else { exit = false; }
                }
                else
                {
                    //crear
                    string ctStr = "CREATE TABLE [" + tableOwner + "].[" + tableName + "](\r\n";
                    for (int i = 0; i < dtSchemaTable.Columns.Count; i++)
                    {
                        ctStr += "  [" + dtSchemaTable.Rows[0][i].ToString() + "][varchar](50) NULL";

                        if (i < dtSchemaTable.Columns.Count)
                        {
                            ctStr += ",";
                        }
                        ctStr += "\r\n";
                    }
                    ctStr += ")";
                    // Runs the SQL command to make the destination table.		
                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = ctStr;
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    escribirTraceLog(TraceEventType.Information, " tabla " + tableOwner + "." + tableName + " creada en BD: " + conn.Database.ToString());
                    exit = true;
                }
                return exit;
            }
            catch (Exception ex)
            {
                conn.Close();
                escribirTraceLog(TraceEventType.Error, "EXCEPCION creando tabla en BD: " + conn.Database.ToString() + " " + ex.Message);
                return exit;
            }
        }
        private bool dropTableFromDatabase(string connectionString, string tableOwner, string tableName, ref string msj)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                string pQuery = "DROP TABLE [" + tableOwner + "].[" + tableName + "]";
                SqlCommand cmd = new SqlCommand(pQuery, conn);
                // if you pass just query text
                cmd.CommandType = CommandType.Text;
                // if you pass stored procedure name
                // cmd.CommandType = CommandType.StoredProcedure;   
                cmd.ExecuteNonQuery();
                conn.Close();
                msj = " Tabla " + tableName + " eliminada en BD: " + conn.Database.ToString();
                escribirTraceLog(TraceEventType.Information, msj);
                return true;
            }
            catch (Exception ex)
            {
                conn.Close();
                msj = " Tabla " + tableName + " NO eliminada en BD: " + conn.Database.ToString() + ", " + ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
                return false;
            }
        }
        private bool tableExistsInDatabase(string connectionString, string tableOwner, string tableName)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                bool exists = false;
                try
                {
                    // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                    //select table_schema, table_name from information_schema.tables where table_schema = 'database_name_here' and table_name = 'table_name_here';
                    //var cmd = new OdbcCommand("select case when exists((select * from information_schema.tables where table_name = '" + tableName + "')) then 1 else 0 end");
                    //string q = "select table_schema, table_name from information_schema.tables where table_schema = '" + conn.Database + "' and table_name = '" + tableName + "'";
                    //"CREATE TABLE [" + tableOwner + "].[" + tableName + "]";
                    string q = "SELECT CASE WHEN OBJECT_ID('" + tableOwner + "." + tableName + "', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                    var cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = q;
                    exists = (int)cmd.ExecuteScalar() == 1;
                    conn.Close();
                }
                catch (Exception ex0)
                {
                    try
                    {
                        // Other RDBMS.  Graceful degradation
                        exists = true;
                        var cmdOthers = new SqlCommand("select 1 from " + tableName + " where 1 = 0", conn);
                        cmdOthers.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex1)
                    {
                        exists = false;
                    }
                }
                return exists;
            }
            catch (Exception ex)
            {
                conn.Close();
                escribirTraceLog(TraceEventType.Critical, "EXCEPCION tableExistsInDatabase, Tabla: " + tableName + ", DB" + conn.Database + ", " + ex.Message);
                return false;
            }
        }
        private bool verificarSiRegTablaHijaFueronInsertadosEnTablaPadre(string connectionString, string tableOwnerHija, string tableNameHija, string tableOwnerPadre, string tableNamePadre)
        {
            int minDT1 = 1, maxDT1 = 100;
            DataSet dsDT1 = null, dsDT2 = null;
            string nomRowNum = "NumRow";
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                string sqDT1 = "SELECT RFecha,RHora,RBIN,RTarjeta,RAutoriza,RFchcontable FROM( SELECT ROW_NUMBER() OVER(order by RFecha) AS " + nomRowNum + ",* FROM " + tableOwnerHija + "." + tableNameHija + ") AS tbl WHERE tbl." + nomRowNum + " BETWEEN " + minDT1 + " AND " + maxDT1;
                executeReader(connectionString, sqDT1, ref dsDT1, ref msj);

                int numColsDT1 = dsDT1.Tables[0].Columns.Count;
                int ncDT1 = dsDT1.Tables[0].Columns.Count; //Nro Columnas Data Table 1
                int nrDT1 = dsDT1.Tables[0].Rows.Count; // Nro Filas Data Table 1
                int numMatchs = 0;
                for (int rDT1 = 0; rDT1 < dsDT1.Tables[0].Rows.Count; rDT1++)
                {
                    string RFecha = dsDT1.Tables[0].Rows[rDT1][0].ToString(); // RFecha = 0
                    string RHora = dsDT1.Tables[0].Rows[rDT1][1].ToString(); // RHora = 1
                    string RBIN = dsDT1.Tables[0].Rows[rDT1][2].ToString(); // RBIN = 2
                    string RTarjeta = dsDT1.Tables[0].Rows[rDT1][3].ToString(); // RTarjeta = 3
                    string RAutoriza = dsDT1.Tables[0].Rows[rDT1][4].ToString(); // RAutoriza = 4
                    string RFchcontable = dsDT1.Tables[0].Rows[rDT1][5].ToString(); // RFchcontable = 5
                    string q = "SELECT * FROM [" + conn.Database + "].[" + tableOwnerPadre + "].[" + tableNamePadre + "] where RFecha='" + RFecha + "' and RHora='" + RHora + "' and RBIN='" + RBIN + "' and RTarjeta='" + RTarjeta + "' and RAutoriza='" + RAutoriza + "'	and RFchcontable='" + RFchcontable + "'";
                    executeReader(connectionString, q, ref dsDT2, ref msj);
                    if (dsDT2.Tables[0].Rows.Count > 0)
                    {
                        numMatchs += dsDT2.Tables[0].Rows.Count;
                    }
                }
                if (numMatchs > 0)
                {
                    agregarLineaEnArchivo("D:\\compensacion\\logs\\", "match.txt", numMatchs.ToString());
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                msj = ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
                return false;
            }
        }
        private void countMatchRowsBetweenDataTables(System.Data.DataTable dt1, System.Data.DataTable dt2, ref long numMatchs, ref string msj)
        {
            try
            {
                ////hacer esto para que se pueda leer y cambiar la data de la DataTable
                //foreach (DataColumn dc in dt1.Columns)
                //{
                //    dc.ReadOnly = false;
                //}
                int ncDT1 = dt1.Columns.Count; //Nro Columnas Data Table 1
                int nrDT1 = dt1.Rows.Count; // Nro Filas Data Table 1
                //
                int ncDT2 = dt2.Columns.Count; //Nro Columnas Data Table 2
                int nrDT2 = dt2.Rows.Count; // Nro Filas Data Table 2
                //
                numMatchs = 0;
                for (int rDT1 = 0; rDT1 < dt1.Rows.Count; rDT1++)
                {
                    string rowReadDT1 = "";
                    for (int cDT1 = 0; cDT1 < dt1.Columns.Count; cDT1++)
                    {
                        rowReadDT1 += dt1.Rows[rDT1][cDT1].ToString();
                    }
                    //
                    for (int rDT2 = 0; rDT2 < dt2.Rows.Count; rDT2++)
                    {
                        string rowReadDT2 = "";
                        for (int cDT2 = 0; cDT2 < dt2.Columns.Count; cDT2++)
                        {
                            rowReadDT2 += dt2.Rows[rDT2][cDT2].ToString();
                        }
                        //comparar si las dos filas linealizadas como string coinciden
                        if (rowReadDT1 == rowReadDT2)
                        {
                            numMatchs++;
                        }
                    }
                }
                if (numMatchs > 0)
                {
                    agregarLineaEnArchivo("D:\\compensacion\\logs\\", "match.txt", numMatchs.ToString());
                }
            }
            catch (Exception ex)
            {
                msj = ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void agregarLineasEnArchivo(string dir, string filename, ref string[] lineas)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(Path.Combine(dir, filename)))
                {
                    File.Create(Path.Combine(dir, filename));
                }
                File.AppendAllLines(Path.Combine(dir, filename), lineas);
            }
            catch (Exception ex)
            {
                escribirTraceLog(TraceEventType.Error, ex.Message);
            }
        }
        private void leerLineasDeArchivo(string dir, string filename, ref string[] lineas)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    if (File.Exists(Path.Combine(dir, filename)))
                    {
                        lineas = File.ReadAllLines(Path.Combine(dir, filename));
                    }
                    else
                    {
                        escribirTraceLog(TraceEventType.Information, "Archivo no existe: " + filename);
                    }
                }
                else
                {
                    escribirTraceLog(TraceEventType.Information, "Directorio no existe: " + dir);
                }

            }
            catch (Exception ex)
            {
                escribirTraceLog(TraceEventType.Error, ex.Message);
            }
        }
        public void agregarLineaEnArchivo(string dir, string filename, string linea)
        {
            //dir = "D:\\compensacion\\logs\\";
            //filename = "match.txt";
            if (!Directory.Exists(dir))
            {
                DirectoryInfo di = Directory.CreateDirectory(dir);
            }
            filename = dir + filename;
            using (StreamWriter sw = File.AppendText(filename))
            {
                sw.WriteLine(linea);
            }
        }
        private void contarRegistrosCoincidentesEntreBD(string connectionString, string tableOwner1, string tableName1, string tableOwner2, string tableName2, int numHilos, ref Thread[] hilosLanzados)
        {

            DataSet ds1 = null, ds2 = null;
            string sqls1 = "SELECT COUNT(*) FROM " + tableOwner1 + "." + tableName1;
            string sqls2 = "SELECT COUNT(*) FROM " + tableOwner2 + "." + tableName2;
            int numRowsTbl1 = 0;
            executeScalar(connectionString, sqls1, ref numRowsTbl1, ref msj);
            int numRowsTbl2 = 0;
            executeScalar(connectionString, sqls2, ref numRowsTbl2, ref msj);

            int rangoDT1 = numRowsTbl1 / numHilos;
            int moduloDT1 = numRowsTbl1 % numHilos;
            int[,] rangosDT1 = new int[numHilos, 2];
            for (int i = 0; i < numHilos; i++)
            {
                if (i == 0) // first
                {
                    rangosDT1[i, 0] = 1;
                    rangosDT1[i, 1] = rangoDT1;
                }
                else if (i == numHilos - 1) // last
                {
                    rangosDT1[i, 0] = rangosDT1[i - 1, 1] + 1;
                    rangosDT1[i, 1] = rangosDT1[i, 0] + rangoDT1 - 1 + moduloDT1;
                }
                else
                {
                    rangosDT1[i, 0] = rangosDT1[i - 1, 1] + 1;
                    rangosDT1[i, 1] = rangosDT1[i, 0] + rangoDT1 - 1;
                }
            }

            int rangoDT2 = numRowsTbl2 / numHilos;
            int moduloDT2 = numRowsTbl2 % numHilos;
            int[,] rangosDT2 = new int[numHilos, 2];
            for (int i = 0; i < numHilos; i++)
            {
                if (i == 0) // first
                {
                    rangosDT2[i, 0] = 1;
                    rangosDT2[i, 1] = rangoDT2;
                }
                else if (i == numHilos - 1) // last
                {
                    rangosDT2[i, 0] = rangosDT2[i - 1, 1] + 1;
                    rangosDT2[i, 1] = rangosDT2[i, 0] + rangoDT2 - 1 + moduloDT2;
                }
                else
                {
                    rangosDT2[i, 0] = rangosDT2[i - 1, 1] + 1;
                    rangosDT2[i, 1] = rangosDT2[i, 0] + rangoDT2 - 1;
                }
            }
            //SELECT* FROM(
            //    SELECT*, ROW_NUMBER() OVER (order by RFecha) AS RowNum FROM dbo.Trx_Bco_Austro
            //) AS tbl
            //WHERE tbl.RowNum BETWEEN 11121 AND 13903
            int minDT1, maxDT1, minDT2, maxDT2;
            DataSet dsDT1 = null, dsDT2 = null;
            int contadorGeneral = 0;

            //eliminar "D:\\compensacion\\logs\\", "match.txt"
            eliminarArchivo("D:\\compensacion\\logs\\", "match.txt", ref cr, ref msj);
            for (int i = 0; i < numHilos; i++)
            {
                minDT1 = rangosDT1[i, 0];
                maxDT1 = rangosDT1[i, 1];
                string attribs = "[RFecha],[RHora],[RBIN],[RTarjeta],[RTipo Trx Cod],[RTipo Cuenta],[RValor],[RCod Error],[RReferencia],[RPos Entry Mode],[RProcesador],[Reversado],[RErrado],[RTerminal Id],[RIndica EMV],[ROrg Link],[RDest Link],[RAcquierer Code],[RIssuer Code],[RAutoriza],[RFchContable],[R_id]";
                string nomRowNum = "RowNum";
                string sqDT1 = "SELECT * FROM( SELECT ROW_NUMBER() OVER(order by RFecha) AS " + nomRowNum + ",* FROM " + tableOwner1 + "." + tableName1 + ") AS tbl WHERE tbl." + nomRowNum + " BETWEEN " + minDT1 + " AND " + maxDT1;
                executeReader(connectionString, sqDT1, ref dsDT1, ref msj);
                dsDT1.Tables[0].Columns.RemoveAt(0); // remove nomRowNum
                for (int j = 0; j < numHilos; j++)
                {
                    minDT2 = rangosDT2[j, 0];
                    maxDT2 = rangosDT2[j, 1];
                    string sqDT2 = "SELECT * FROM( SELECT ROW_NUMBER() OVER(order by RFecha) AS " + nomRowNum + ",* FROM " + tableOwner2 + "." + tableName2 + ") AS tbl WHERE tbl." + nomRowNum + " BETWEEN " + minDT2 + " AND " + maxDT2;
                    executeReader(connectionString, sqDT2, ref dsDT2, ref msj);
                    dsDT2.Tables[0].Columns.RemoveAt(0); // remove nomRowNum
                    long numMatchsThread = 0;

                    Thread thread = new Thread(delegate ()
                    {
                        countMatchRowsBetweenDataTables(dsDT1.Tables[0], dsDT2.Tables[0], ref numMatchsThread, ref msj);
                    });
                    thread.Start();
                    hilosLanzados[contadorGeneral] = thread;
                    contadorGeneral++;
                }
            }


            //string sql1 = "SELECT * FROM " + tableOwner1 + "." + tableName1;
            //string sql2 = "SELECT * FROM " + tableOwner2 + "." + tableName2;
            //executeReader(connectionString, sql1, ref ds1, ref msj);
            //executeReader(connectionString, sql2, ref ds2, ref msj);
            //long numMatchs = 0;
            //countMatchRowsBetweenDataTables(ds1.Tables[0], ds2.Tables[0], ref numMatchs, ref msj);
            //MessageBox.Show("Reg Coincidentes: " + numMatchs);
        }
        private void executeScalar(string connString, string sql, ref int result, ref string msj)
        {
            try
            {
                SqlConnection connection;
                SqlTransaction transaction = null;
                SqlCommand command = new SqlCommand(sql);
                using (connection = new SqlConnection(connString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        try
                        {
                            command.Connection = connection;
                            result = (int)command.ExecuteScalar();
                            msj = "Query ExecuteScalar SQL Ejecutada correctamente: " + sql;
                            escribirTraceLog(TraceEventType.Information, msj);
                        }
                        catch (Exception ex)
                        {
                            string exsql = ex.Message;
                            connection.Close();
                            msj = "EXCEPCION ExecuteScalar exec SQL; " + exsql + ", En: " + sql;
                            escribirTraceLog(TraceEventType.Error, msj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION ExecuteScalar exec SQL; " + ex.Message + ", En: " + sql;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void executeReader(string connString, string sql, ref DataSet ds, ref string msj)
        {
            try
            {
                SqlConnection connection;
                SqlTransaction transaction = null;
                SqlCommand command = new SqlCommand(sql);
                using (connection = new SqlConnection(connString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        try
                        {
                            command.Connection = connection;
                            SqlDataReader oracleDataReader = command.ExecuteReader();
                            DataSet dataSet = new DataSet("ROWSET");
                            dataSet.Load((IDataReader)oracleDataReader, LoadOption.OverwriteChanges, new string[1] { "ROW" });
                            msj = "Query ExecuteReader SQL Ejecutada correctamente: " + sql;
                            escribirTraceLog(TraceEventType.Information, msj);
                            ds = dataSet.Copy();
                        }
                        catch (Exception ex)
                        {
                            string exsql = ex.Message;
                            connection.Close();
                            msj = "EXCEPCION ExecuteReader exec SQL; " + exsql + ", En: " + sql;
                            escribirTraceLog(TraceEventType.Error, msj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION ExecuteReader exec SQL; " + ex.Message + ", En: " + sql;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        private void executeNonQuery(string connStringOracle, string sql, ref int enqResult, ref string msj)
        {
            try
            {
                SqlConnection connection;
                SqlTransaction transaction = null;
                SqlCommand command = new SqlCommand(sql);
                command.CommandType = CommandType.Text;

                using (connection = new SqlConnection(connStringOracle))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        try
                        {
                            command.Connection = connection;
                            enqResult = command.ExecuteNonQuery();
                            connection.Close();
                            msj = "EXITO " + sql + "; filas afectadas: " + enqResult;
                            escribirTraceLog(TraceEventType.Information, msj);
                        }
                        catch (Exception ex)
                        {
                            string exsql = ex.Message;
                            connection.Close();
                            msj = "EXCEPCION " + exsql + ", Ejecutando: " + sql;
                            escribirTraceLog(TraceEventType.Error, msj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msj = ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }
        //*Fin cagar data*//

        //* SQL Scripts *//
        private void ejecutarScriptSQL(string tablaOrigen, string tablaDestino, string connString, ref string msj)
        {
            //string baseDatosDestino = "octopus_pci_comp";
            //string tablaDestino = "Trx_Bco_Austro";
            //string tablaOrigen = "Trx_Bco_Austro_" + dtpComp.Value.ToString("yy") + dtpComp.Value.ToString("MM") + dtpComp.Value.ToString("dd");
            int numRows = 0;
            using (SqlConnection connection = new SqlConnection())
            {

                //--24 / 01 / 2020
                msj = "--" + dtpComp.Value.ToString("dd") + " / " + dtpComp.Value.ToString("MM") + " / " + dtpComp.Value.ToString("yyyy");
                escribirTraceLog(TraceEventType.Information, msj);

                SqlTransaction transaction;
                connection.ConnectionString = connString;
                connection.Open();

                string baseDatos = connection.Database;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction" + baseDatos);

                try
                {
                    string stmt = @"insert into " + baseDatos + ".." + tablaDestino +
                    " select RFecha, RHora, RBIN, RTarjeta, [RTipo Trx Cod], [RTipo Cuenta], " +
                    "RValor = replace(replace(RValor, '\"', ''), ',', '.'), [RCod Error], RReferencia, " +
                    "[RPos Entry Mode], RProcesador, Reversado, RErrado, [RTerminal Id], [RIndica EMV], [ROrg Link], " +
                    "[RDest Link], [RAcquierer Code], [RIssuer Code], RAutoriza, RFchcontable from " + baseDatos + ".." + tablaOrigen;
                    escribirTraceLog(TraceEventType.Information, stmt);
                    SqlCommand command = new SqlCommand(stmt, connection);
                    command.Connection = connection;
                    command.Transaction = transaction;
                    numRows = command.ExecuteNonQuery();
                    // Attempt to commit the transaction.
                    transaction.Commit();
                    connection.Close();
                    //--13745
                    msj = "--" + numRows.ToString();
                    escribirTraceLog(TraceEventType.Information, msj);
                }
                catch (Exception ex)
                {
                    msj = "EXCEPCION " + ex.Message;
                    escribirTraceLog(TraceEventType.Critical, msj);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        msj = "Rollback Exception Type: " + ex2.GetType() + "\n" + "  Message: " + ex2.Message;
                        escribirTraceLog(TraceEventType.Error, msj);
                    }
                }
            }
        }
        //* Fin SQL Scripts*//
        private bool conectarPathRemoto(string srvRemoto, string driveVirtual, string usrRemoto, string pwdRemoto, ref int codRetMap, ref string msj)
        {
            try
            {
                bool successMapped = false;
                DriveSettings.MapNetworkDrive(srvRemoto, driveVirtual, usrRemoto, pwdRemoto, ref successMapped, ref codRetMap, ref msj);
                if (successMapped)
                {
                    //Mapped!
                    msj = "Map Drive Network Mapped " + "SrvRemoto: " + srvRemoto + " ; " + "DriveVirtual: " + driveVirtual + " ; " + "USR: " + usrRemoto + " ; " + "PWD: " + pwdRemoto + ", " + msj + ", code: " + codRetMap;
                    escribirTraceLog(TraceEventType.Information, msj);
                    return true;
                }
                else
                {
                    //Failed!
                    msj = "Map Drive Network Failed " + "SrvRemoto: " + srvRemoto + " ; " + "DriveVirtual: " + driveVirtual + " ; " + "USR: " + usrRemoto + " ; " + "PWD: " + pwdRemoto + ", " + msj + ", code: " + codRetMap;
                    escribirTraceLog(TraceEventType.Error, msj);
                    return false;
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
                return false;
            }
        }
        private bool desconectarPathRemoto(string driveVirtual, ref int codRetMap, ref string msj)
        {
            try
            {
                bool successDisccon = false;
                codRetMap = DriveSettings.DisconnectNetworkDrive(driveVirtual, true, ref successDisccon, ref msj);
                if (successDisccon)
                {
                    //Mapped!
                    msj = "Map Drive Network Discconected Succesfully" + ", " + driveVirtual + ", " + msj + ", code: " + codRetMap;
                    escribirTraceLog(TraceEventType.Information, msj);
                    return true;
                }
                else
                {
                    //Failed!
                    msj = "Map Drive Discconected Failed " + ", " + driveVirtual + ", " + msj + ", code: " + codRetMap;
                    escribirTraceLog(TraceEventType.Error, msj);
                    return false;
                }
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
                return false;
            }
        }
        //**//
        private void correrCompensacion(ref string msj)
        {
            long codRet = 0;
            msj = "";
            //a. Luego creamos una carpeta dentro de la ruta: D:\Compensacion con el siguiente formato
            //   YYYY - MM - DD de la fecha que está ejecutándose la compensación
            msj = "";
            string rutaComp = ConfigurationManager.AppSettings["pathFldrCompensacion"];
            string nombreCarpeta = dtpComp.Value.ToString("yyyy") + "-" + dtpComp.Value.ToString("MM") + "-" + dtpComp.Value.ToString("dd");
            string pathCarpetaCreada = "";
            crearCarpeta(rutaComp, nombreCarpeta, ref pathCarpetaCreada, ref codRet, ref msj);


            //b. Dentro de la carpeta copiamos el archivo generado por el SEP
            msj = "";
            string rutaAbsolutaArchivoSEP = lblPathArchivo.Text;
            string fileNameSEP = Path.GetFileName(rutaAbsolutaArchivoSEP);
            string rutaArchivoSEP = rutaAbsolutaArchivoSEP.Replace(fileNameSEP, "");
            copiarArchivo(rutaArchivoSEP, fileNameSEP, ref codRet, ref msj, pathCarpetaCreada);


            //c. Luego de abrir el archivo procedemos a guardarlo con un formato: Texto (delimitado por tabulaciones) (*.txt) Y con el siguiente nombre: Trx_Bco_Austro_180607.txt
            //   180607 es la fecha de ejecución de la compensación YYMMDD
            msj = "";
            string formatoNombreArchivoSEP = ConfigurationManager.AppSettings["formatoNombreArchivoSEP"]; // Trx_Bco_Austro_
            string formatoFechaArchivoSEP = ConfigurationManager.AppSettings["formatoFechaArchivoSEP"]; //yyMMdd
            string nombreFecha = fchComp.ToString(formatoFechaArchivoSEP); // 180607
            string extSEP = ConfigurationManager.AppSettings["extNombreArchivoSEP"]; // .txt
            string nombreGuardarArchivoSEP = formatoNombreArchivoSEP + nombreFecha + extSEP;
            string absolutePathScrXlsx = Path.Combine(pathCarpetaCreada, fileNameSEP);
            string absolutePathDstTxt = Path.Combine(pathCarpetaCreada, nombreGuardarArchivoSEP);
            xlsxToTextDelimitedTab(absolutePathScrXlsx, absolutePathDstTxt, ref msj);


            //d. import txt file to database octopus_pci
            msj = "";
            string octopus_pci_ConnString = ConfigurationManager.AppSettings["octopus_pci"];
            string dbq = pathCarpetaCreada; // D:\\compensacion\\2020-01-29
            string fileName = nombreGuardarArchivoSEP; // Trx_Bco_Austro_200129.txt
            string tableOwner = ConfigurationManager.AppSettings["tableOwner"]; // dbo
            string tableName = formatoNombreArchivoSEP + dtpComp.Value.ToString("yy") + dtpComp.Value.ToString("MM") + dtpComp.Value.ToString("dd"); //Trx_Bco_Austro_200129
            saveToDatabaseDirectly(octopus_pci_ConnString, dbq, fileName, tableOwner, tableName, ref msj);

            //e. import txt file to database octopus_pci_comp
            msj = "";
            string octopus_pci_comp_ConnString = ConfigurationManager.AppSettings["octopus_pci_comp"];
            saveToDatabaseDirectly(octopus_pci_comp_ConnString, dbq, fileName, tableOwner, tableName, ref msj);

            //f. Run scripts on database octopus_pci
            msj = "";
            string tablaOrigen = tableName; //Trx_Bco_Austro_200129
            string tablaDestino = ConfigurationManager.AppSettings["tableAllComp"]; //Trx_Bco_Austro
            DialogResult rRunScript = MessageBox.Show("¿" + "Ejecutar Script para carga de data de: " + tablaOrigen + "-->" + tablaDestino + ", En:" + new SqlConnection(octopus_pci_ConnString).Database + "?", "Confirmar Accion Irreversible", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rRunScript == DialogResult.Yes)
            {
                ejecutarScriptSQL(tablaOrigen, tablaDestino, octopus_pci_ConnString, ref msj);
            }
            //f. Run scripts on database octopus_pci_comp
            msj = "";
            DialogResult rRunScript2 = MessageBox.Show("¿" + "Ejecutar Script para carga de data de: " + tablaOrigen + "-->" + tablaDestino + ", En:" + new SqlConnection(octopus_pci_comp_ConnString).Database + "?", "Confirmar Accion Irreversible", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rRunScript2 == DialogResult.Yes)
            {
                ejecutarScriptSQL(tablaOrigen, tablaDestino, octopus_pci_comp_ConnString, ref msj);
            }

            //g. Copiar el rar enviado hacia la carpeta de compensación en la ruta del disco D:
            msj = "";
            string rutaAbsolutaArchivoCompContable = lblPathArchCompContable.Text;
            string fileNameCompContable = Path.GetFileName(rutaAbsolutaArchivoCompContable);
            string rutaArchivoCompContable = rutaAbsolutaArchivoCompContable.Replace(fileNameCompContable, "");
            copiarArchivo(rutaArchivoCompContable, fileNameCompContable, ref codRet, ref msj, pathCarpetaCreada);

            //h. extraer los archivos en una carpeta del mismo nombre del archivo
            msj = "";
            string pathAUSSrc = Path.Combine(pathCarpetaCreada, fileNameCompContable);
            string pathAUSExtraerDst = Path.Combine(pathCarpetaCreada, Path.GetFileNameWithoutExtension(fileNameCompContable));
            extraerArchivosDeZip(pathAUSSrc, pathAUSExtraerDst, ref msj);

            //i. Compensación AUSTPC, Primera en ejecutarse, para esto es necesario copiar los archivos • AUSTPC180607.TXT • MAUS0607.RPT
            // Hacia el servidor 10.1.99.81 , Usuario de acceso: BAUSTRO\..., Password: ad...., Dentro de la ruta: C:\Files_BRD\ 
            string pathSrvRemotoFiles_BRD = ConfigurationManager.AppSettings["pathSrvRemotoFiles_BRD"];
            string driveVirtualFiles_BRD = ConfigurationManager.AppSettings["driveVirtualFiles_BRD"];
            string usrRemoto = ConfigurationManager.AppSettings["usrRemoto"];
            string pwdRemoto = ConfigurationManager.AppSettings["pwdRemoto"];
            int codRetMap = 0;
            bool conPR = conectarPathRemoto(pathSrvRemotoFiles_BRD, driveVirtualFiles_BRD, usrRemoto, pwdRemoto, ref codRetMap, ref msj);

            string archivoAUSTP = ConfigurationManager.AppSettings["formatFileAUSTP"] + dtpComp.Value.ToString(ConfigurationManager.AppSettings["formatFileAUSTPFch"]) + ConfigurationManager.AppSettings["formatFileAUSTPExt"];
            string archivoAUSTPC = ConfigurationManager.AppSettings["formatFileAUSTPC"] + dtpComp.Value.ToString(ConfigurationManager.AppSettings["formatFileAUSTPCFch"]) + ConfigurationManager.AppSettings["formatFileAUSTPCExt"];
            string archivoMAUS = ConfigurationManager.AppSettings["formatFileMAUS"] + dtpComp.Value.ToString(ConfigurationManager.AppSettings["formatFileMAUSFch"]) + ConfigurationManager.AppSettings["formatFileMAUSExt"];

            string pathArchivoAUTP = Path.Combine(pathAUSExtraerDst, archivoAUSTP);
            string pathArchivoAUTPC = Path.Combine(pathAUSExtraerDst, archivoAUSTPC);
            string pathArchivoMAUS = Path.Combine(pathAUSExtraerDst, archivoMAUS);

            if (conPR)
            {
                msj = "";
                copiarArchivo(pathAUSExtraerDst, archivoAUSTPC, ref cr, ref msj, driveVirtualFiles_BRD + ":");
                msj = "";
                copiarArchivo(pathAUSExtraerDst, archivoMAUS, ref cr, ref msj, driveVirtualFiles_BRD + ":");
            }

            //j. Luego de colocar los archivos renombrar el archivo:•	AUSTPC180607.TXT a AUSTP180607.TXT  
            msj = "";
            renombrarArchivo(driveVirtualFiles_BRD + ":", archivoAUSTPC, archivoAUSTP, ref cr, ref msj);
            ////Y la carpeta 1806 se cambia a 1806 v AUSTP
            //msj = "";
            //string nombreOldCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture);
            //string nombreNewCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTP";
            //este paso se obvia por que cuando se acaba los procesos AUSTPC y AUSTP deja con el nombre yyMM V AUSTPC o bien yyMM V AUSTP respectivamente.
            //renombrarCarpeta(driveVirtualFiles_BRD + ":", nombreOldCarpeta, nombreNewCarpeta, ref cr, ref msj);
            //La carpeta 1806 v AUSTPC a 1806
            msj = "";
            string nombreOldCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTPC";
            string nombreNewCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture);
            cr = -1; //codigo retorno, si realiza todo correcto es 0
            do
            {
                renombrarCarpeta(driveVirtualFiles_BRD + ":", nombreOldCarpeta, nombreNewCarpeta, ref cr, ref msj);
                if (cr != 0)
                {
                    MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + nombreOldCarpeta + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while (cr != 0);
            //k. Luego de realizar estos cambios nos ubicamos en el disco D:
            msj = "";
            string pathSrvRemotoREPORTES_COMP = ConfigurationManager.AppSettings["pathSrvRemotoREPORTES_COMP"];
            string driveVirtualREPORTES_COMP = ConfigurationManager.AppSettings["driveVirtualREPORTES_COMP"];
            codRetMap = 0;
            conPR = conectarPathRemoto(pathSrvRemotoREPORTES_COMP, driveVirtualREPORTES_COMP, usrRemoto, pwdRemoto, ref codRetMap, ref msj);
            //y cambiamos el nombre a la siguiente carpeta: REPORTES_COMP a REPORTES_COMP v AUSTP 
            //msj = "";
            //string nombreOldCarpetaRepComp = "REPORTES_COMP";
            //string nombreNewCarpetaRepComp = "REPORTES_COMP V AUSTP";
            //este paso se obvia por que cuando se acaba los procesos AUSTPC y AUSTP deja con el nombre V AUSTPC o bien V AUSTP respectivamente.
            //renombrarCarpeta(driveVirtualREPORTES_COMP + ":", nombreOldCarpetaRepComp, nombreNewCarpetaRepComp, ref cr, ref msj);
            //Y  REPORTES_COMP v AUSTPC a REPORTES_COMP
            msj = "";
            string nombreOldCarpetaRepComp = "REPORTES_COMP V AUSTPC";
            string nombreNewCarpetaRepComp = "REPORTES_COMP";
            cr = -1; //codigo retorno, si realiza todo correcto es 0
            do
            {
                renombrarCarpeta(driveVirtualREPORTES_COMP + ":", nombreOldCarpetaRepComp, nombreNewCarpetaRepComp, ref cr, ref msj);
                if (cr != 0)
                {
                    MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + nombreOldCarpetaRepComp + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while (cr != 0);
            ////l. Luego de realizados los cambios nos vamos a C:\Users\ad01000680\Desktop\Compensacion\Compensacion AUSTPC\AS_DCompensa.vbp Y ejecutamos el proyecto AS_DCompensa.vbp
            ////C: \Users\ba0100063g\Desktop
            //string pathSrvRemotoUSERS = ConfigurationManager.AppSettings["pathSrvRemotoUSERS"];
            //string driveVirtualUSERS = ConfigurationManager.AppSettings["driveVirtualUSERS"];

            //conPR = conectarPathRemoto(pathSrvRemotoUSERS, driveVirtualUSERS, usrRemoto, pwdRemoto, ref codRetMap, ref msj);
            //int idProceso = 0;
            //Process p = null;
            //correrNuevoProceso("cmd.exe", "/C " + "\"" + driveVirtualUSERS + ConfigurationManager.AppSettings["pathRunCompAUSTP"] + "\"", ref idProceso, ref p, ref msj);

            //EJECUTAR EL PROYECTO .VBP AUSTPC (C:\Users\ba0100063g\Desktop\Compensacion\Compensacion AUSTPC\AS_DCompensa.vbp)
            /*
            antes de mandar a correr vpb AUSTPC
            verificar que exista:
            archivos
            - 1.G:/MAUS0901.RPT ;2.G:/AUSTP200901.TXT
            carpetas
            - 3. G:/2009 ;4. F:/REPORTES_COMP
            */
            string verifPrevio = "";
            if (!File.Exists(driveVirtualFiles_BRD + ":" + "//" + archivoMAUS))
            {
                verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + archivoMAUS + "\n";
            }
            if (!File.Exists(driveVirtualFiles_BRD + ":" + "//" + archivoAUSTP))
            {
                verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + archivoAUSTP + "\n";
            }
            if (!Directory.Exists(driveVirtualFiles_BRD + ":" + "//" + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture)))
            {
                verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + "\n";
            }
            if (!Directory.Exists(driveVirtualREPORTES_COMP + ":" + "REPORTES_COMP"))
            {
                verifPrevio += " No existe " + driveVirtualREPORTES_COMP + ":" + "REPORTES_COMP";
            }
            if (!string.IsNullOrEmpty(verifPrevio.Trim()))
            {
                MessageBox.Show("ERROR: Antes de correr el proceso, se debe corregir lo siguiente: " + verifPrevio);
            }

            DialogResult r = MessageBox.Show(this, "Ejecutar el Proceso AUSTPC (Desktop//Compensacion//Compensacion AUSTPC//AS_DCompensa.vbp), Cuando se mande a correr presionar SI", "Ejecutar Proceso AUSTPC", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            escribirTraceLog(TraceEventType.Information, "INFO ejecutando proceso AUSTPC");

            if (r == DialogResult.Yes)
            {
                escribirTraceLog(TraceEventType.Information, "INFO Se confirmo ejecucion del proceso AUSTPC");
                //// Luego dentro de la ruta: D:\ExtremeLog\Compensalog\  luego ir a la carpeta con Año y mes actual EJM 1806 – YYMM Dentro abra un log con los logs de la compensación:
                //// Si sale Analizando sistema significa que ya termino. Y para ello debe haber generado dentro de la ruta: D:\REPORTES_COMP\Compensacion\ 
                //// m. Una carpeta con el formato YYMMDD  en el caso de prueba 180613 Dentro de esta carpeta se encontraran los archivos resultantes de la compensación. Ruta: D:\REPORTES_COMP\Compensacion\180613\BRD que son 5:

                string rutaArchivosGenCompAUSTP = driveVirtualREPORTES_COMP + ":" + "\\REPORTES_COMP\\Compensacion\\" + dtpComp.Value.ToString("yyMMdd", CultureInfo.InvariantCulture) + "\\BRD\\";
                FileInfo[] filesGenComp = null;
                int numArchsGen = 0;
                int tiempoEsperaSegundos = int.Parse(ConfigurationManager.AppSettings["tiempoEsperaSegundosVerifArchGen"]);
                int tiempoTranscurridoSegundos = 0 * 60;
                while (!(numArchsGen >= 5 || (tiempoTranscurridoSegundos > tiempoEsperaSegundos)))
                {
                    int verifEnMillis = 10000; //verificar cada tanto tiempo el numero de archivos generados
                    Thread.Sleep(verifEnMillis); // espero 1 segundo
                    tiempoTranscurridoSegundos += verifEnMillis / 1000;
                    obtenerArchivosEnPath(rutaArchivosGenCompAUSTP, ref numArchsGen, ref filesGenComp);
                }
                if ((tiempoTranscurridoSegundos > tiempoEsperaSegundos) && numArchsGen < 5)
                {
                    MessageBox.Show(this, "WARN: Se supero el tiempo transcurrido de Espera (" + (tiempoTranscurridoSegundos / 60) + " Min.)" + "\nArchivos Generados: " + numArchsGen, "Informacion AUSTPC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    float mins = (tiempoTranscurridoSegundos / 60);
                    MessageBox.Show(this, "EXITO: " + numArchsGen + " Archivos Generados " + " En " + mins + " Min." + "\nRuta: " + rutaArchivosGenCompAUSTP + "\nDetenga y Cierre el proyecto .vbp", "Informacion AUSTPC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // n. Los mismos que se deben copiar dentro del pc no el servidor en la ruta: D:\Compensacion\2018-06-13
                // Crear una nueva carpeta 'comp' 
                string rutaCopiarArchivosGenCompAUSTP = "D:\\Compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "\\";
                string pathCarpetaCreadaComp_comp = "";
                crearCarpeta(rutaCopiarArchivosGenCompAUSTP, "comp", ref pathCarpetaCreadaComp_comp, ref cr, ref msj);

                // o. dentro de esta carpeta una AUSTPC ahí colocar los archivos copiados. ahí colocar los archivos copiados
                copiarCarpeta(driveVirtualREPORTES_COMP + ":" + "\\REPORTES_COMP\\Compensacion\\" + dtpComp.Value.ToString("yyMMdd", CultureInfo.InvariantCulture) + "\\", "BRD", pathCarpetaCreadaComp_comp, "AUSTPC", ref cr, ref msj);

                // p. Observar que una vez colocados los archivos se los debe comprimir en un rar con el nombre: CompensacionAUSTPC180612.rar
                string pathCarpetaCreadaComp_AUSTPC = Path.Combine(pathCarpetaCreadaComp_comp, "AUSTPC");
                string extArchivoComprimido = ConfigurationManager.AppSettings["extArchivoComprimido"];
                if (string.IsNullOrEmpty(extArchivoComprimido))
                {
                    extArchivoComprimido = ".zip";//default
                }
                comprimirArchivosEnZip(Path.Combine(pathCarpetaCreadaComp_AUSTPC, "CompensacionAUSTPC" + dtpComp.Value.ToString("yyMMdd") + extArchivoComprimido), filesGenComp, ref cr, ref msj);

                /*
                Una vez corrido AUSTPC dejar cambiando el nombre de las carpetas
                - 1. G:/2009 --> G:/2009 V AUSTPC
                - 2. F:/REPORTES_COMP --> F:/REPORTES_COMP V AUSTPC
                */
                cr = -1; //codigo retorno, si realiza todo correcto es 0
                do
                {
                    renombrarCarpeta(driveVirtualFiles_BRD + ":", dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture), dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTPC", ref cr, ref msj);
                    if (cr != 0)
                    {
                        MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } while (cr != 0);
                cr = -1; //codigo retorno, si realiza todo correcto es 0
                do
                {
                    renombrarCarpeta(driveVirtualREPORTES_COMP + ":", "REPORTES_COMP", "REPORTES_COMP V AUSTPC", ref cr, ref msj);
                    if (cr != 0)
                    {
                        MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + "REPORTES_COMP" + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } while (cr != 0);

                //Luego se ejecuta la segunda parte
                //renombrar
                //La carpeta 1806 v AUSTP a 1806
                nombreOldCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTP";
                nombreNewCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture);
                cr = -1; //codigo retorno, si realiza todo correcto es 0
                do
                {
                    renombrarCarpeta(driveVirtualFiles_BRD + ":", nombreOldCarpeta, nombreNewCarpeta, ref cr, ref msj);
                    if (cr != 0)
                    {
                        MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + nombreOldCarpeta + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } while (cr != 0);
                //y cambiamos el nombre a la siguiente carpeta: REPORTES_COMP v AUSTP a REPORTES_COMP 
                nombreOldCarpetaRepComp = "REPORTES_COMP V AUSTP";
                nombreNewCarpetaRepComp = "REPORTES_COMP";
                cr = -1; //codigo retorno, si realiza todo correcto es 0
                do
                {
                    renombrarCarpeta(driveVirtualREPORTES_COMP + ":", nombreOldCarpetaRepComp, nombreNewCarpetaRepComp, ref cr, ref msj);
                    if (cr != 0)
                    {
                        MessageBox.Show(this, "ERROR RENOMBRANDO: " + msj + ", Intente cerrar la carpeta " + nombreOldCarpetaRepComp + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } while (cr != 0);

                //COPIAR MAUS0901.RPT y AUSTP200901.TXT
                if (conPR)
                {
                    copiarArchivo(pathAUSExtraerDst, archivoAUSTP, ref cr, ref msj, driveVirtualFiles_BRD + ":");
                    copiarArchivo(pathAUSExtraerDst, archivoMAUS, ref cr, ref msj, driveVirtualFiles_BRD + ":");
                }

                //EJECUTAR EL PROYECTO .VBP AUSTP C:\Users\ba0100063g\Desktop\Compensacion\Compensacion AUSTP\AS_DCompensa.vbp
                /*
                antes de mandar a correr vpb AUSTP
                verificar que exista:
                archivos
                - 1.G:/MAUS0901.RPT ;2.G:/AUSTP200901.TXT
                carpetas
                - 3. G:/2009 ;4. F:/REPORTES_COMP
                */
                verifPrevio = "";
                if (!File.Exists(driveVirtualFiles_BRD + ":" + "//" + archivoMAUS))
                {
                    verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + archivoMAUS + "\n";
                }
                if (!File.Exists(driveVirtualFiles_BRD + ":" + "//" + archivoAUSTP))
                {
                    verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + archivoAUSTP + "\n";
                }
                if (!Directory.Exists(driveVirtualFiles_BRD + ":" + "//" + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture)))
                {
                    verifPrevio += " No existe " + driveVirtualFiles_BRD + ":" + "//" + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + "\n";
                }
                if (!Directory.Exists(driveVirtualREPORTES_COMP + ":" + "REPORTES_COMP"))
                {
                    verifPrevio += " No existe " + driveVirtualREPORTES_COMP + ":" + "REPORTES_COMP";
                }
                if (!string.IsNullOrEmpty(verifPrevio.Trim()))
                {
                    MessageBox.Show("ERROR: Antes de correr el proceso, se debe corregir lo siguiente: " + verifPrevio);
                }

                DialogResult rAUSTP = MessageBox.Show(this, "Ejecutar el Proceso AUSTP (Desktop//Compensacion//Compensacion AUSTP//AS_DCompensa.vbp), Cuando se mande a correr presionar SI", "Ejecutar Proceso AUSTP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                escribirTraceLog(TraceEventType.Information, "INFO ejecutando proceso AUSTP");

                if (r == DialogResult.Yes)
                {
                    escribirTraceLog(TraceEventType.Information, "INFO Se confirmo ejecucion del proceso AUSTP");
                    //
                    rutaArchivosGenCompAUSTP = driveVirtualREPORTES_COMP + ":" + "\\REPORTES_COMP\\Compensacion\\" + dtpComp.Value.ToString("yyMMdd", CultureInfo.InvariantCulture) + "\\BRD\\";
                    filesGenComp = null;
                    numArchsGen = 0;
                    tiempoEsperaSegundos = int.Parse(ConfigurationManager.AppSettings["tiempoEsperaSegundosVerifArchGen"]);
                    tiempoTranscurridoSegundos = 0 * 60;
                    while (!(numArchsGen >= 5 || (tiempoTranscurridoSegundos > tiempoEsperaSegundos)))
                    {
                        int verifEnMillis = 10000; //verificar cada tanto tiempo el numero de archivos generados
                        Thread.Sleep(verifEnMillis); // espero 1 segundo
                        tiempoTranscurridoSegundos += verifEnMillis / 1000;
                        obtenerArchivosEnPath(rutaArchivosGenCompAUSTP, ref numArchsGen, ref filesGenComp);
                    }
                    if ((tiempoTranscurridoSegundos > tiempoEsperaSegundos) && numArchsGen < 5)
                    {
                        MessageBox.Show(this, "Se supero el tiempo transcurrido de Espera (" + (tiempoTranscurridoSegundos / 60) + " Min.)" + "\nArchivos Generados: " + numArchsGen, "Informacion AUSTP");
                    }
                    else
                    {
                        float mins = (tiempoTranscurridoSegundos / 60);
                        MessageBox.Show(this, "EXITO: " + numArchsGen + " Archivos Generados " + " En " + mins + " Min." + "\nRuta: " + rutaArchivosGenCompAUSTP + "\nDetenga y Cierre el proyecto .vbp", "Informacion AUSTP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // n. Los mismos que se deben copiar dentro del pc no el servidor en la ruta: D:\Compensacion\2018-06-13
                    // la carpeta 'comp' ya se creo
                    // o. dentro de esta carpeta una AUSTP ahí colocar los archivos copiados. ahí colocar los archivos copiados
                    copiarCarpeta(driveVirtualREPORTES_COMP + ":" + "\\REPORTES_COMP\\Compensacion\\" + dtpComp.Value.ToString("yyMMdd", CultureInfo.InvariantCulture) + "\\", "BRD", pathCarpetaCreadaComp_comp, "AUSTP", ref cr, ref msj);

                    // p. Observar que una vez colocados los archivos se los debe comprimir en un rar con el nombre: CompensacionAUSTPC180612.rar
                    pathCarpetaCreadaComp_AUSTPC = Path.Combine(pathCarpetaCreadaComp_comp, "AUSTP");
                    extArchivoComprimido = ConfigurationManager.AppSettings["extArchivoComprimido"];
                    if (string.IsNullOrEmpty(extArchivoComprimido))
                    {
                        extArchivoComprimido = ".zip";//default
                    }
                    comprimirArchivosEnZip(Path.Combine(pathCarpetaCreadaComp_AUSTPC, "CompensacionAUSTP" + dtpComp.Value.ToString("yyMMdd") + extArchivoComprimido), filesGenComp, ref cr, ref msj);

                    /*
                    Una vez corrido AUSTP dejar cambiando el nombre de las carpetas
                    - 1. G:/2009 --> G:/2009 V AUSTP
                    - 2. F:/REPORTES_COMP --> F:/REPORTES_COMP V AUSTP
                    */
                    cr = -1; //codigo retorno, si realiza todo correcto es 0
                    do
                    {
                        renombrarCarpeta(driveVirtualFiles_BRD + ":", dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture), dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTP", ref cr, ref msj);
                        if (cr != 0)
                        {
                            MessageBox.Show(this, "ERROR: " + msj + ", Intente cerrar la carpeta " + dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } while (cr != 0);
                    cr = -1; //codigo retorno, si realiza todo correcto es 0
                    do
                    {
                        renombrarCarpeta(driveVirtualREPORTES_COMP + ":", "REPORTES_COMP", "REPORTES_COMP V AUSTP", ref cr, ref msj);
                        if (cr != 0)
                        {
                            MessageBox.Show(this, "ERROR: " + msj + ", Intente cerrar la carpeta " + "REPORTES_COMP" + " o los procesos que hagan uso de la misma", "Solucionar Problema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } while (cr != 0);
                    //FIN

                    //Enviar MAIL CON LOS DOS ARCHIVOS COMPRIMIDOS
                }
                else if (r == DialogResult.No)
                {
                    escribirTraceLog(TraceEventType.Warning, "WARN: No se confirmo terminacion del proceso AUSTP");
                }
                else { }
            }
            else if (r == DialogResult.No)
            {
                escribirTraceLog(TraceEventType.Warning, "WARN: No se confirmo terminacion del proceso AUSTPC");
            }
            else { }
            //FIN, ABRIR LOS ARCHIVOS Y PEDIR AL USUARIO QUE REVISE SI LA DATA ESTA ENMASCARADA
            //cmd.exe / C notepad D:\compensacion\2020-09-03\comp\AUSTP\AS0010_CompensaTP_ADQ_200903.txt
            msj = "";
            int idProcess = 0;
            Process pr = null;
            MessageBox.Show(this, "Por favor Revisar si los reportes generados tienen las TDs enmascaradas.", "Revisar Archivos Generados", MessageBoxButtons.OK, MessageBoxIcon.Question);
            correrNuevoProceso("cmd.exe", "/C notepad D:\\compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp\\AUSTPC\\AS0010_CompensaTP_ADQ_" + dtpComp.Value.ToString("yyMMdd") + ".txt", ref idProcess, ref pr, ref msj);
            correrNuevoProceso("cmd.exe", "/C notepad D:\\compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp\\AUSTP\\AS0010_CompensaTP_ADQ_" + dtpComp.Value.ToString("yyMMdd") + ".txt", ref idProcess, ref pr, ref msj);
            //enviar mail
            DialogResult rMail = MessageBox.Show(this, "Enviar Mail", "Enviar Mail", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            escribirTraceLog(TraceEventType.Information, "INFO enviando mail");
            if (rMail == DialogResult.Yes)
            {
                enviarMailConAdjunto();
            }
        }
        //**//
        private void dtpCompFechActual_ValueChanged(object sender, EventArgs e)
        {
            msj = "";
            DateTime fchComp = DateTime.Now;
            calcularFechaCompensacion(dtpCompFechActual.Value, ref fchComp, ref msj);
            dtpComp.Value = fchComp;

        }
        private void btnXlsxToTextDelimTab_Click(object sender, EventArgs e)
        {
            msj = "";
            string absolutePathSrcXlsx = "D:\\compensacion\\2019-11-05\\gvwResultados01.xlsx";
            string absolutePathDstTxt = "D:\\compensacion\\2019-11-05\\gvwResultados01.txt";
            xlsxToTextDelimitedTab(absolutePathSrcXlsx, absolutePathDstTxt, ref msj);
        }
        private void btnOpenNotepad_Click(object sender, EventArgs e)
        {
            msj = "";
            int idProcess = 0;
            Process pr = null;
            //correrNuevoProceso("cmd.exe", "/C notepad", ref idProcess, ref pr, ref msj);
            MessageBox.Show(this, "Por favor Revisar si los reportes generados tienen las TDs enmascaradas.", "Revisar Archivos Generados", MessageBoxButtons.OK, MessageBoxIcon.Question);
            correrNuevoProceso("cmd.exe", "/C notepad D:\\compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp\\AUSTPC\\AS0010_CompensaTP_ADQ_" + dtpComp.Value.ToString("yyMMdd") + ".txt", ref idProcess, ref pr, ref msj);
            correrNuevoProceso("cmd.exe", "/C notepad D:\\compensacion\\" + dtpComp.Value.ToString("yyyy-MM-dd") + "\\comp\\AUSTP\\AS0010_CompensaTP_ADQ_" + dtpComp.Value.ToString("yyMMdd") + ".txt", ref idProcess, ref pr, ref msj);
        }
        private void btnImportDataBD_Click(object sender, EventArgs e)
        {
            msj = "";
            string dbq = "D:\\compensacion\\2020-01-29";
            string fileName = "Trx_Bco_Austro_200129.txt";
            string tableOwner = "dbo";
            string tableName = "Trx_Bco_Austro_200129";
            saveToDatabaseDirectly("octopus_pci", dbq, fileName, tableOwner, tableName, ref msj);

        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            msj = "";
            fchActual = dtpCompFechActual.Value;
            fchComp = dtpComp.Value;
            correrCompensacion(ref msj);
        }
        private void btnRunScriptSQL_Click(object sender, EventArgs e)
        {
            msj = "";
            fchActual = dtpCompFechActual.Value;
            fchComp = dtpComp.Value;
            //ejecutarScriptSQL(ref msj);

        }
        private void btnOpenFileCompContable_Click(object sender, EventArgs e)
        {
            msj = "";
            string absPathFile = "", fileName = "";
            string filter = "Compressed files (.rar)|*.rar;*.zip";
            abrirArchivo(ref msj, ref absPathFile, ref fileName, filter);
            lblPathArchCompContable.Text = absPathFile;
            if (!fileName.Contains(dtpComp.Value.ToString("MMdd")))
            {
                MessageBox.Show(this, "El archivo " + fileName + " debe contener: ..." + dtpComp.Value.ToString("MMdd") + "... como parte de su nombre", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (!absPathFile.Contains(dtpComp.Value.ToString("yyyy-MM-dd")))
            {
                MessageBox.Show(this, "La ruta " + absPathFile + " debe contener: ..." + dtpComp.Value.ToString("yyyy-MM-dd") + "... como parte de su ruta", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void btnPathRemoto_Click(object sender, EventArgs e)
        {
            string pathSrvRemotoUSERS = ConfigurationManager.AppSettings["pathSrvRemotoUSERS"];
            string driveVirtualUSERS = ConfigurationManager.AppSettings["driveVirtualUSERS"];
            string usrRemoto = ConfigurationManager.AppSettings["usrRemoto"];
            string pwdRemoto = ConfigurationManager.AppSettings["pwdRemoto"];
            int codRetMap = -1;
            conectarPathRemoto(pathSrvRemotoUSERS, driveVirtualUSERS, usrRemoto, pwdRemoto, ref codRetMap, ref msj);
        }

        private void btnOpenFileSEP_Click(object sender, EventArgs e)
        {
            msj = "";
            string absPathFile = "", fileName = "";
            string filter = "Excel Files (*.xlsx)|*.xlsx";
            abrirArchivo(ref msj, ref absPathFile, ref fileName, filter);
            lblPathArchivo.Text = absPathFile;
            if (!absPathFile.Contains(dtpComp.Value.ToString("yyyy-MM-dd")))
            {
                MessageBox.Show(this, "La ruta " + absPathFile + " debe contener: ..." + dtpComp.Value.ToString("yyyy-MM-dd") + "... como parte de su ruta", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show(this, "Se contara los registros de: " + fileName + ", esperar 2 min aprox.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //contar registros de excel
                Thread t = new Thread(new ParameterizedThreadStart(contarRegistroDeExcelJob));
                t.Start(absPathFile);
            }
        }
        private void btnDisconnPathRemoto_Click(object sender, EventArgs e)
        {
            string driveVirtualUSERS = ConfigurationManager.AppSettings["driveVirtualUSERS"];
            int codRetMap = -1;
            desconectarPathRemoto(driveVirtualUSERS, ref codRetMap, ref msj);
        }
        private void btnCountRegMatchBD_Click(object sender, EventArgs e)
        {
            string octopus_pci_ConnString = ConfigurationManager.AppSettings["octopus_pci"];
            string to = "dbo";
            string tn1 = "Trx_Bco_Austro";
            string tn2 = "Trx_Bco_Austro_200205";
            //
            //int numHilos = 5;
            //Thread[] hilosLanzados = new Thread[numHilos * numHilos];

            //contarRegistrosCoincidentesEntreBD(octopus_pci_ConnString, to, tn1, to, tn2, numHilos, ref hilosLanzados);

            //int hilosTotales = numHilos * numHilos;

            //Thread tf = new Thread(delegate ()
            //{
            //    finHilos(hilosLanzados);
            //});
            //tf.Start();

            verificarSiRegTablaHijaFueronInsertadosEnTablaPadre(octopus_pci_ConnString, to, tn2, to, tn1);



        }
        private void finHilos(Thread[] hilosLanzados)
        {
            int T = hilosLanzados.Count();
            int P = 0;

            while (P < T)
            {
                P = 0;
                foreach (Thread t in hilosLanzados)
                {
                    if (t.IsAlive == false)
                    {
                        P++;
                    }
                }
            }
            string[] lineas = null;
            string dir = "D:\\compensacion\\logs\\";
            string filenameLeer = "match.txt";
            string filenameEscribir = "matchResultado.txt";
            leerLineasDeArchivo(dir, filenameLeer, ref lineas);
            int coincidenciasTotales = 0;
            string[] lineasSum = new string[1];
            if (lineas != null)
            {
                foreach (string l in lineas)
                {
                    int coindicenciasParciales = int.Parse(l);
                    coincidenciasTotales += coindicenciasParciales;
                }
            }
            eliminarArchivo(dir, filenameEscribir, ref cr, ref msj);
            agregarLineaEnArchivo(dir, filenameEscribir, coincidenciasTotales.ToString());

        }

        private void btnReversarInsercionTabla_Click(object sender, EventArgs e)
        {
            //string connString = ConfigurationManager.AppSettings["octopus_pci_comp"];
            string connString = ConfigurationManager.AppSettings["octopus_pci"];

            DataSet dsDT1 = null, dsDT2 = null;
            SqlConnection conn = new SqlConnection(connString);
            string tableOwnerHija = "dbo";
            string tableNameHija = "Trx_Bco_Austro_200901";
            string tableOwnerPadre = "dbo";
            string tableNamePadre = "Trx_Bco_Austro";
            try
            {
                string sqDT1 = "SELECT * FROM " + tableOwnerHija + "." + tableNameHija;
                executeReader(connString, sqDT1, ref dsDT1, ref msj);

                int numColsDT1 = dsDT1.Tables[0].Columns.Count;
                int ncDT1 = dsDT1.Tables[0].Columns.Count; //Nro Columnas Data Table 1
                int nrDT1 = dsDT1.Tables[0].Rows.Count; // Nro Filas Data Table 1
                int numMatchs = 0;
                int numMatchsElim = 0;

                for (int rDT1 = 0; rDT1 < dsDT1.Tables[0].Rows.Count; rDT1++)
                {
                    string RFecha = dsDT1.Tables[0].Rows[rDT1]["RFecha"].ToString(); // RFecha = 0
                    string RHora = dsDT1.Tables[0].Rows[rDT1]["RHora"].ToString(); // 
                    string RBIN = dsDT1.Tables[0].Rows[rDT1]["RBIN"].ToString(); // 
                    string RTarjeta = dsDT1.Tables[0].Rows[rDT1]["RTarjeta"].ToString(); // 
                    string RTipoTrxCod = dsDT1.Tables[0].Rows[rDT1]["RTipo Trx Cod"].ToString(); //
                    string RTipoCuenta = dsDT1.Tables[0].Rows[rDT1]["RTipo Cuenta"].ToString(); //
                    string RCodError = dsDT1.Tables[0].Rows[rDT1]["RCod Error"].ToString(); //
                    string RReferencia = dsDT1.Tables[0].Rows[rDT1]["RReferencia"].ToString(); //
                    string RPosEntryMode = dsDT1.Tables[0].Rows[rDT1]["RPos Entry Mode"].ToString(); // 
                    string RProcesador = dsDT1.Tables[0].Rows[rDT1]["RProcesador"].ToString(); // 
                    string Reversado = dsDT1.Tables[0].Rows[rDT1]["Reversado"].ToString(); // 
                    string RErrado = dsDT1.Tables[0].Rows[rDT1]["RErrado"].ToString(); // 
                    string RTerminalId = dsDT1.Tables[0].Rows[rDT1]["RTerminal Id"].ToString(); // 
                    string RIndicaEMV = dsDT1.Tables[0].Rows[rDT1]["RIndica EMV"].ToString(); // 
                    string ROrgLink = dsDT1.Tables[0].Rows[rDT1]["ROrg Link"].ToString(); // 
                    string RDestLink = dsDT1.Tables[0].Rows[rDT1]["RDest Link"].ToString(); // 
                    string RAcquiererCode = dsDT1.Tables[0].Rows[rDT1]["RAcquierer Code"].ToString(); //
                    string RIssuerCode = dsDT1.Tables[0].Rows[rDT1]["RIssuer Code"].ToString(); //
                    string RAutoriza = dsDT1.Tables[0].Rows[rDT1]["RAutoriza"].ToString(); //
                    string RFchcontable = dsDT1.Tables[0].Rows[rDT1]["RFchcontable"].ToString(); // 

                    string q = "SELECT * FROM [" + conn.Database + "].[" + tableOwnerPadre + "].[" + tableNamePadre + "] where " +
                        "RFecha = '" + RFecha
                        + "' and RHora = '" + RHora
                        + "' and RBIN = '" + RBIN
                        + "' and RTarjeta = '" + RTarjeta
                        + "' and RTipoTrxCod = '" + RTipoTrxCod
                        + "' and RTipoCuenta = '" + RTipoCuenta
                        + "' and RCodError = '" + RCodError
                        + "' and RReferencia = '" + RReferencia
                        + "' and RPosEntryMode = '" + RPosEntryMode
                        + "' and RProcesador = '" + RProcesador
                        + "' and Reversado = '" + Reversado
                        + "' and RErrado = '" + RErrado
                        + "' and RTerminalId = '" + RTerminalId
                        + "' and RIndicaEMV = '" + RIndicaEMV
                        + "' and ROrgLink = '" + ROrgLink
                        + "' and RDestLink = '" + RDestLink
                        + "' and RAcquiererCode = '" + RAcquiererCode
                        + "' and RIssuerCode = '" + RIssuerCode
                        + "' and RAutoriza = '" + RAutoriza
                        + "' and RFchcontable = '" + RFchcontable + "'";
                    executeReader(connString, q, ref dsDT2, ref msj);
                    if (dsDT2.Tables[0].Rows.Count > 0)
                    {
                        numMatchs += dsDT2.Tables[0].Rows.Count;
                    }
                }
                if (numMatchs > 0)
                {
                    string msj1 = "Existen " + nrDT1 + " Registros en " + tableOwnerHija + "." + tableNameHija + ", BD:" + conn.Database;
                    string msj2 = "Se eliminaran " + numMatchs + " Registros de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database;
                    DialogResult res = MessageBox.Show(this, msj1 + "\n" + msj2, "Confirmar", MessageBoxButtons.YesNo);
                    escribirTraceLog(TraceEventType.Information, msj1 + "\n" + msj2);
                    if (res == DialogResult.Yes)
                    {
                        for (int rDT1 = 0; rDT1 < dsDT1.Tables[0].Rows.Count; rDT1++)
                        {
                            string RFecha = dsDT1.Tables[0].Rows[rDT1]["RFecha"].ToString(); // RFecha = 0
                            string RHora = dsDT1.Tables[0].Rows[rDT1]["RHora"].ToString(); // 
                            string RBIN = dsDT1.Tables[0].Rows[rDT1]["RBIN"].ToString(); // 
                            string RTarjeta = dsDT1.Tables[0].Rows[rDT1]["RTarjeta"].ToString(); // 
                            string RTipoTrxCod = dsDT1.Tables[0].Rows[rDT1]["RTipo Trx Cod"].ToString(); //
                            string RTipoCuenta = dsDT1.Tables[0].Rows[rDT1]["RTipo Cuenta"].ToString(); //
                            string RCodError = dsDT1.Tables[0].Rows[rDT1]["RCod Error"].ToString(); //
                            string RReferencia = dsDT1.Tables[0].Rows[rDT1]["RReferencia"].ToString(); //
                            string RPosEntryMode = dsDT1.Tables[0].Rows[rDT1]["RPos Entry Mode"].ToString(); // 
                            string RProcesador = dsDT1.Tables[0].Rows[rDT1]["RProcesador"].ToString(); // 
                            string Reversado = dsDT1.Tables[0].Rows[rDT1]["Reversado"].ToString(); // 
                            string RErrado = dsDT1.Tables[0].Rows[rDT1]["RErrado"].ToString(); // 
                            string RTerminalId = dsDT1.Tables[0].Rows[rDT1]["RTerminal Id"].ToString(); // 
                            string RIndicaEMV = dsDT1.Tables[0].Rows[rDT1]["RIndica EMV"].ToString(); // 
                            string ROrgLink = dsDT1.Tables[0].Rows[rDT1]["ROrg Link"].ToString(); // 
                            string RDestLink = dsDT1.Tables[0].Rows[rDT1]["RDest Link"].ToString(); // 
                            string RAcquiererCode = dsDT1.Tables[0].Rows[rDT1]["RAcquierer Code"].ToString(); //
                            string RIssuerCode = dsDT1.Tables[0].Rows[rDT1]["RIssuer Code"].ToString(); //
                            string RAutoriza = dsDT1.Tables[0].Rows[rDT1]["RAutoriza"].ToString(); //
                            string RFchcontable = dsDT1.Tables[0].Rows[rDT1]["RFchcontable"].ToString(); // 

                            string q = "DELETE FROM [" + conn.Database + "].[" + tableOwnerPadre + "].[" + tableNamePadre + "] where " +
                                "RFecha = '" + RFecha
                                + "' and RHora = '" + RHora
                                + "' and RBIN = '" + RBIN
                                + "' and RTarjeta = '" + RTarjeta
                                + "' and RTipoTrxCod = '" + RTipoTrxCod
                                + "' and RTipoCuenta = '" + RTipoCuenta
                                + "' and RCodError = '" + RCodError
                                + "' and RReferencia = '" + RReferencia
                                + "' and RPosEntryMode = '" + RPosEntryMode
                                + "' and RProcesador = '" + RProcesador
                                + "' and Reversado = '" + Reversado
                                + "' and RErrado = '" + RErrado
                                + "' and RTerminalId = '" + RTerminalId
                                + "' and RIndicaEMV = '" + RIndicaEMV
                                + "' and ROrgLink = '" + ROrgLink
                                + "' and RDestLink = '" + RDestLink
                                + "' and RAcquiererCode = '" + RAcquiererCode
                                + "' and RIssuerCode = '" + RIssuerCode
                                + "' and RAutoriza = '" + RAutoriza
                                + "' and RFchcontable = '" + RFchcontable + "'";
                            int filasAfectadas = 0;
                            executeNonQuery(connString, q, ref filasAfectadas, ref msj);
                            if (filasAfectadas > 0)
                            {
                                numMatchsElim += filasAfectadas;
                            }
                        }
                        MessageBox.Show("Se eliminaron: " + numMatchsElim + " Registros, de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database);
                    }
                    else
                    {
                        escribirTraceLog(TraceEventType.Information, "Se cancelo la operacion Eliminar Registros de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database);
                    }
                }
            }
            catch (Exception ex)
            {
                msj = ex.Message;
                escribirTraceLog(TraceEventType.Error, msj);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnRevertInsertDB_Click(object sender, EventArgs e)
        {
            if (txtBDBorrarRegCoincid.Text == null
                || txtTablaHija.Text == null
                || txtTablaPadre.Text == null)
            {
                MessageBox.Show(this, "WARN: Datos Nulos o Vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtBDBorrarRegCoincid.Text.Trim().Length == 0
                || txtTablaHija.Text.Trim().Length == 0
                || txtTablaPadre.Text.Trim().Length == 0)
            {
                MessageBox.Show(this, "WARN: Datos Nulos o Vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connString = "";
            if (txtBDBorrarRegCoincid.Text == "octopus_pci_comp")
            {
                connString = ConfigurationManager.AppSettings["octopus_pci_comp"];
            }
            if (txtBDBorrarRegCoincid.Text == "octopus_pci")
            {
                connString = ConfigurationManager.AppSettings["octopus_pci"];
            }
            if (string.IsNullOrEmpty(connString))
            {
                MessageBox.Show(this, "WARN: key no existe en config: " + txtBDBorrarRegCoincid.Text, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult resConf = MessageBox.Show(this, "Esta operacion no debe ser interrumpida.\nDependiendo del numero de registros este proceso puede tardar 15 min para la verificacion y conteo de 30000 registros coincidentes y 30 min para la eliminacion de los mismos (aprox.).\n¿Desea continuar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resConf == DialogResult.Yes)
            {
                DataSet dsDT1 = null, dsDT2 = null;
                SqlConnection conn = new SqlConnection(connString);
                string tableOwnerHija = "dbo";
                string tableNameHija = txtTablaHija.Text;
                string tableOwnerPadre = "dbo";
                string tableNamePadre = txtTablaPadre.Text;
                try
                {
                    string sqDT1 = "SELECT * FROM " + tableOwnerHija + "." + tableNameHija;
                    executeReader(connString, sqDT1, ref dsDT1, ref msj);

                    int numColsDT1 = dsDT1.Tables[0].Columns.Count;
                    int ncDT1 = dsDT1.Tables[0].Columns.Count; //Nro Columnas Data Table 1
                    int nrDT1 = dsDT1.Tables[0].Rows.Count; // Nro Filas Data Table 1
                    int numMatchs = 0;
                    int numMatchsElim = 0;

                    for (int rDT1 = 0; rDT1 < dsDT1.Tables[0].Rows.Count; rDT1++)
                    {
                        string RFecha = dsDT1.Tables[0].Rows[rDT1]["RFecha"].ToString(); // RFecha = 0
                        string RHora = dsDT1.Tables[0].Rows[rDT1]["RHora"].ToString(); // 
                        string RBIN = dsDT1.Tables[0].Rows[rDT1]["RBIN"].ToString(); // 
                        string RTarjeta = dsDT1.Tables[0].Rows[rDT1]["RTarjeta"].ToString(); // 
                        string RTipoTrxCod = dsDT1.Tables[0].Rows[rDT1]["RTipo Trx Cod"].ToString(); //
                        string RTipoCuenta = dsDT1.Tables[0].Rows[rDT1]["RTipo Cuenta"].ToString(); //
                        string RCodError = dsDT1.Tables[0].Rows[rDT1]["RCod Error"].ToString(); //
                        string RReferencia = dsDT1.Tables[0].Rows[rDT1]["RReferencia"].ToString(); //
                        string RPosEntryMode = dsDT1.Tables[0].Rows[rDT1]["RPos Entry Mode"].ToString(); // 
                        string RProcesador = dsDT1.Tables[0].Rows[rDT1]["RProcesador"].ToString(); // 
                        string Reversado = dsDT1.Tables[0].Rows[rDT1]["Reversado"].ToString(); // 
                        string RErrado = dsDT1.Tables[0].Rows[rDT1]["RErrado"].ToString(); // 
                        string RTerminalId = dsDT1.Tables[0].Rows[rDT1]["RTerminal Id"].ToString(); // 
                        string RIndicaEMV = dsDT1.Tables[0].Rows[rDT1]["RIndica EMV"].ToString(); // 
                        string ROrgLink = dsDT1.Tables[0].Rows[rDT1]["ROrg Link"].ToString(); // 
                        string RDestLink = dsDT1.Tables[0].Rows[rDT1]["RDest Link"].ToString(); // 
                        string RAcquiererCode = dsDT1.Tables[0].Rows[rDT1]["RAcquierer Code"].ToString(); //
                        string RIssuerCode = dsDT1.Tables[0].Rows[rDT1]["RIssuer Code"].ToString(); //
                        string RAutoriza = dsDT1.Tables[0].Rows[rDT1]["RAutoriza"].ToString(); //
                        string RFchcontable = dsDT1.Tables[0].Rows[rDT1]["RFchcontable"].ToString(); // 

                        string q = "SELECT * FROM [" + conn.Database + "].[" + tableOwnerPadre + "].[" + tableNamePadre + "] where " +
                            "RFecha = '" + RFecha
                            + "' and RHora = '" + RHora
                            + "' and RBIN = '" + RBIN
                            + "' and RTarjeta = '" + RTarjeta
                            + "' and RTipoTrxCod = '" + RTipoTrxCod
                            + "' and RTipoCuenta = '" + RTipoCuenta
                            + "' and RCodError = '" + RCodError
                            + "' and RReferencia = '" + RReferencia
                            + "' and RPosEntryMode = '" + RPosEntryMode
                            + "' and RProcesador = '" + RProcesador
                            + "' and Reversado = '" + Reversado
                            + "' and RErrado = '" + RErrado
                            + "' and RTerminalId = '" + RTerminalId
                            + "' and RIndicaEMV = '" + RIndicaEMV
                            + "' and ROrgLink = '" + ROrgLink
                            + "' and RDestLink = '" + RDestLink
                            + "' and RAcquiererCode = '" + RAcquiererCode
                            + "' and RIssuerCode = '" + RIssuerCode
                            + "' and RAutoriza = '" + RAutoriza
                            + "' and RFchcontable = '" + RFchcontable + "'";
                        executeReader(connString, q, ref dsDT2, ref msj);
                        if (dsDT2.Tables[0].Rows.Count > 0)
                        {
                            numMatchs += dsDT2.Tables[0].Rows.Count;
                        }
                        escribirTraceLog(TraceEventType.Information, "Verificando registros coincidentes; Procesados: " + rDT1 + ", Coincidencias: " + numMatchs);
                    }
                    if (numMatchs > 0)
                    {
                        string msj1 = "Existen " + nrDT1 + " Registros en " + tableOwnerHija + "." + tableNameHija + ", BD:" + conn.Database;
                        string msj2 = "Se eliminaran " + numMatchs + " Registros de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database;
                        DialogResult res = MessageBox.Show(this, msj1 + "\n" + msj2, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        escribirTraceLog(TraceEventType.Information, msj1 + "\n" + msj2);
                        if (res == DialogResult.Yes)
                        {
                            for (int rDT1 = 0; rDT1 < dsDT1.Tables[0].Rows.Count; rDT1++)
                            {
                                string RFecha = dsDT1.Tables[0].Rows[rDT1]["RFecha"].ToString(); // RFecha = 0
                                string RHora = dsDT1.Tables[0].Rows[rDT1]["RHora"].ToString(); // 
                                string RBIN = dsDT1.Tables[0].Rows[rDT1]["RBIN"].ToString(); // 
                                string RTarjeta = dsDT1.Tables[0].Rows[rDT1]["RTarjeta"].ToString(); // 
                                string RTipoTrxCod = dsDT1.Tables[0].Rows[rDT1]["RTipo Trx Cod"].ToString(); //
                                string RTipoCuenta = dsDT1.Tables[0].Rows[rDT1]["RTipo Cuenta"].ToString(); //
                                string RCodError = dsDT1.Tables[0].Rows[rDT1]["RCod Error"].ToString(); //
                                string RReferencia = dsDT1.Tables[0].Rows[rDT1]["RReferencia"].ToString(); //
                                string RPosEntryMode = dsDT1.Tables[0].Rows[rDT1]["RPos Entry Mode"].ToString(); // 
                                string RProcesador = dsDT1.Tables[0].Rows[rDT1]["RProcesador"].ToString(); // 
                                string Reversado = dsDT1.Tables[0].Rows[rDT1]["Reversado"].ToString(); // 
                                string RErrado = dsDT1.Tables[0].Rows[rDT1]["RErrado"].ToString(); // 
                                string RTerminalId = dsDT1.Tables[0].Rows[rDT1]["RTerminal Id"].ToString(); // 
                                string RIndicaEMV = dsDT1.Tables[0].Rows[rDT1]["RIndica EMV"].ToString(); // 
                                string ROrgLink = dsDT1.Tables[0].Rows[rDT1]["ROrg Link"].ToString(); // 
                                string RDestLink = dsDT1.Tables[0].Rows[rDT1]["RDest Link"].ToString(); // 
                                string RAcquiererCode = dsDT1.Tables[0].Rows[rDT1]["RAcquierer Code"].ToString(); //
                                string RIssuerCode = dsDT1.Tables[0].Rows[rDT1]["RIssuer Code"].ToString(); //
                                string RAutoriza = dsDT1.Tables[0].Rows[rDT1]["RAutoriza"].ToString(); //
                                string RFchcontable = dsDT1.Tables[0].Rows[rDT1]["RFchcontable"].ToString(); // 

                                string q = "DELETE FROM [" + conn.Database + "].[" + tableOwnerPadre + "].[" + tableNamePadre + "] where " +
                                    "RFecha = '" + RFecha
                                    + "' and RHora = '" + RHora
                                    + "' and RBIN = '" + RBIN
                                    + "' and RTarjeta = '" + RTarjeta
                                    + "' and RTipoTrxCod = '" + RTipoTrxCod
                                    + "' and RTipoCuenta = '" + RTipoCuenta
                                    + "' and RCodError = '" + RCodError
                                    + "' and RReferencia = '" + RReferencia
                                    + "' and RPosEntryMode = '" + RPosEntryMode
                                    + "' and RProcesador = '" + RProcesador
                                    + "' and Reversado = '" + Reversado
                                    + "' and RErrado = '" + RErrado
                                    + "' and RTerminalId = '" + RTerminalId
                                    + "' and RIndicaEMV = '" + RIndicaEMV
                                    + "' and ROrgLink = '" + ROrgLink
                                    + "' and RDestLink = '" + RDestLink
                                    + "' and RAcquiererCode = '" + RAcquiererCode
                                    + "' and RIssuerCode = '" + RIssuerCode
                                    + "' and RAutoriza = '" + RAutoriza
                                    + "' and RFchcontable = '" + RFchcontable + "'";
                                int filasAfectadas = 0;
                                executeNonQuery(connString, q, ref filasAfectadas, ref msj);
                                if (filasAfectadas > 0)
                                {
                                    numMatchsElim += filasAfectadas;
                                }
                                escribirTraceLog(TraceEventType.Information, "Eliminando registros coincidentes; Procesados: " + rDT1 + ", Eliminados: " + numMatchsElim);
                            }
                            MessageBox.Show(this, "Se eliminaron: " + numMatchsElim + " Registros, de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database, "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            escribirTraceLog(TraceEventType.Information, "Se cancelo la operacion Eliminar Registros de " + tableOwnerPadre + "." + tableNamePadre + ", BD:" + conn.Database);
                        }
                    }
                }
                catch (Exception ex)
                {
                    msj = ex.Message;
                    escribirTraceLog(TraceEventType.Error, msj);
                }
            }
        }
        private void tabCtrlEjecComp_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtBDBorrarRegCoincid.Text = "octopus_pci";//octopus_pci_comp
            txtTablaPadre.Text = "Trx_Bco_Austro";
            txtTablaHija.Text = "Trx_Bco_Austro_" + dtpComp.Value.ToString("yyMMdd");
        }

        private void btnOpenVB6Exe_Click(object sender, EventArgs e)
        {

            msj = "";
            int idProcess = 0;
            Process pr = null;
            //correrNuevoProceso("cmd.exe", "/C notepad", ref idProcess, ref pr, ref msj);
            MessageBox.Show(this, "Exec Pr", "Run Process VB6.exe", MessageBoxButtons.OK, MessageBoxIcon.Question);
            correrNuevoProceso("D:\\_testVB6_\\ProyectoTest.exe", "", ref idProcess, ref pr, ref msj);
            MessageBox.Show(this, "Kill Exec Pr", "Kill Process VB6.exe", MessageBoxButtons.OK, MessageBoxIcon.Question);
            msj = "";
            killProcess(pr, ref msj);
        }
    }
}
