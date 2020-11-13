using LumenWorks.Framework.IO.Csv;
using Microsoft.Office.Interop.Excel;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        private string diaComp = "";
        private string mesComp = "";
        private string anioComp2Digits = "";
        private string anioComp4Digits = "";
        //fch actual
        private DateTime fchActual;
        private string diaAct = "";
        private string mesAct = "";
        private string anioAct2Digits = "";
        private string anioAct4Digits = "";
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
            loadConfig();

            dtpCompFechActual.Value = DateTime.Now;
            DateTime fchComp = DateTime.Now;
            calcularFechaCompensacion(dtpCompFechActual.Value, ref fchComp, ref msj);
            dtpComp.Value = fchComp;

            diaComp = dtpComp.Value.ToString("dd");
            mesComp = dtpComp.Value.ToString("MM");
            anioComp2Digits = dtpComp.Value.ToString("yy");
            anioComp4Digits = dtpComp.Value.ToString("yyyy");

            diaAct = dtpCompFechActual.Value.ToString("dd");
            mesAct = dtpCompFechActual.Value.ToString("MM");
            anioAct2Digits = dtpCompFechActual.Value.ToString("yy");
            anioAct4Digits = dtpCompFechActual.Value.ToString("yyyy");
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

            string src = pathSrc + "\\" + fileNameSrc;

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
                codigoRetorno = 444; mensaje = " El archivo: " + src + tamSrc + ", ya existe en: " + pathSrc;
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
                        escribirTraceLog(TraceEventType.Warning, " No existe destino: " + pathDst);
                        escribirTraceLog(TraceEventType.Information, " Creando " + pathDst);
                        Directory.CreateDirectory(pathDst);
                        escribirTraceLog(TraceEventType.Information, "EXITO " + pathDst + " Creado");
                    }

                    if (File.Exists(dst)) // existe archivo en el destino
                    {
                        escribirTraceLog(TraceEventType.Warning, " El archivo: " + fileNameDst + tamDst + " existe en el destino: " + pathDst);
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
        private void crearCarpeta(string path, string folderName, ref string pathCarpetaCreada, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                string absolutePath = Path.Combine(path, folderName);
                escribirTraceLog(TraceEventType.Information, " Creando Directorio: " + absolutePath);
                if (Directory.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Directorio ya Existe: " + absolutePath;
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

                string srcAbsolutePath = pathSrc + folderNameSrc;
                string dstAbsolutePath = pathDst + folderNameDst;

                escribirTraceLog(TraceEventType.Information, " Copiando Directorio, DE: " + srcAbsolutePath + ", A: " + dstAbsolutePath);
                if (Directory.Exists(dstAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Directorio con el nombre " + dstAbsolutePath + " ya Existe";
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

                string oldAbsolutePath = path + folderName;
                string newAbsolutePath = path + newFolderName;

                escribirTraceLog(TraceEventType.Information, " Renombrando Directorio, DE: " + oldAbsolutePath + ", A: " + newAbsolutePath);
                if (Directory.Exists(newAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR nombre nuevo del Directorio ya Existe: " + newAbsolutePath;
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
                        crearCarpeta(path, folderName, ref pathDirCre,ref cr, ref msj);
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

                string absolutePath = path + folderName;
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

                string absolutePath = path + fileName;
                escribirTraceLog(TraceEventType.Information, " Creando Archivo: " + absolutePath);
                if (File.Exists(absolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Archivo ya Existe: " + absolutePath;
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

                string oldAbsolutePath = path + fileName;
                string newAbsolutePath = path + newFileName;

                escribirTraceLog(TraceEventType.Information, " Renombrando Archivo, DE: " + oldAbsolutePath + ", A: " + newAbsolutePath);
                if (File.Exists(newAbsolutePath))
                {
                    codigoRetorno = 111; mensaje = "ERROR Archivo ya Existe: " + newAbsolutePath;
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

                string absolutePath = path + fileName;
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
        public void comprimirArchivosEnZip(string pathNombreZip, System.Collections.Generic.List<FileInfo> listArchivos, ref long codigoRetorno, ref string mensaje)
        {
            try
            {

                // Crear o sobreescribir el archivo zip
                FileStream outZipFile = File.Create(pathNombreZip); outZipFile.Close();
                //agregar archivos al .ZIP
                if (listArchivos == null || listArchivos.Count == 0)
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
                mensaje = "EXITO Archivos comprimidos";
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
        private void correrEjecutable()
        {

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
        public void enviarCorreo(string from, string to, string subject, string body, ArrayList attachments, string smtpHost, int portSmtp, bool usarssl, bool validarcertificadodigital, bool requiereautentificacion, string dominio, string nombreusuario, string password, ref long codigoRetorno, ref string mensaje)
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
                string str = to;
                char[] chArray = new char[1] { ';' };
                foreach (string addresses in str.Split(chArray))
                {
                    message.To.Add(addresses);
                }
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
            if (message.Length > 0) { txtBoxLogs.AppendText(DateTime.Now.ToString() + " " + message); txtBoxLogs.AppendText(Environment.NewLine); txtBoxLogs.AppendText(Environment.NewLine); }
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
            enviarCorreo("david-040994@hotmail.com", "mmatute@baustro.fin.ec", "test", "bodyTest", adjuntos, "smtp.sendgrid.net", 587,
                true, false, true, null, "azure_a82b80688e8c3267671082f24e5297e9@azure.com", "040994.-stewoz", ref cr, ref msj);

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
        private void ejecutarComandoPrompt(ref string msj)
        {
            try
            {
                string fileName = "cmd.exe";
                string arguments = "/C notepad";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //para ocultar la ventana cmd
                startInfo.FileName = fileName;
                startInfo.Arguments = arguments;
                process.StartInfo = startInfo;
                process.Start();
                escribirTraceLog(TraceEventType.Information, " Ejecutado " + fileName + " " + arguments);
            }
            catch (Exception ex)
            {
                msj = "EXCEPCION " + ex.Message;
                escribirTraceLog(TraceEventType.Critical, msj);
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
                msj = " Archivo Seleccionado " + absolutePathFile;
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
        private void saveToDatabaseDirectly(string sqlConnString, string dbq, string fileName, string tableOwner, string tableName, ref string msj)
        {
            try
            {

                SqlConnection sqlConn = new SqlConnection(sqlConnString);
                //string dbq = "D:\\compensacion\\" + anioComp4Digits + "-" + mesComp + "-" + diaComp + "\\";
                //string fileName = "gvwResultados01.txt";

                // Creates and opens an ODBC connection
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
                        dt.Rows[0].Delete();
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
                }
                // Writes the number of imported rows to the form
                string notif = "Imported: " + rowCount.ToString() + "/" + rowCount.ToString() + " row(s)";
                msj = "EXITO carga data en BD " + sqlConn.Database + ", Tabla: " + tableOwner + "." + tableName + " exitosa: " + notif;
                escribirTraceLog(TraceEventType.Information, msj);
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
            try
            {
                // Generates the create table command.
                // The first column of schema table contains the column names.
                // The data type is nvarcher(4000) in all columns.

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
                return true;
            }
            catch (Exception ex)
            {
                escribirTraceLog(TraceEventType.Critical, "EXCEPCION creando tabla en BD: " + conn.Database.ToString() + " " + ex.Message);
                return false;
            }
        }

        //*Fin cagar data*//

        //* SQL Scripts *//
        private void ejecutarScriptSQL(string tablaOrigen, string tablaDestino, string connString, ref string msj)
        {
            //string baseDatosDestino = "octopus_pci_comp";
            //string tablaDestino = "Trx_Bco_Austro";
            //string tablaOrigen = "Trx_Bco_Austro_" + anioComp2Digits + mesComp + diaComp;
            int numRows = 0;
            using (SqlConnection connection = new SqlConnection())
            {

                //--24 / 01 / 2020
                msj = "--" + diaComp + " / " + mesComp + " / " + anioComp4Digits;
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
                NetworkDrive nd = new NetworkDrive();
                nd.MapNetworkDrive(srvRemoto, driveVirtual, usrRemoto, pwdRemoto, ref codRetMap, ref msj);
                if (codRetMap == 0 || codRetMap == 85)
                {
                    //Mapped!
                    msj = "Map Drive Network Mapped " + srvRemoto + ", " + driveVirtual + ", " + msj + ", code: " + codRetMap;
                    escribirTraceLog(TraceEventType.Information, msj);
                    return true;
                }
                else
                {
                    //Failed!
                    msj = "Map Drive Network Failed " + srvRemoto + ", " + driveVirtual + ", " + msj + ", code: " + codRetMap;
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
            string nombreCarpeta = anioComp4Digits + "-" + mesComp + "-" + diaComp;
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
            string tableName = formatoNombreArchivoSEP + anioComp2Digits + mesComp + diaComp; //Trx_Bco_Austro_200129
            saveToDatabaseDirectly(octopus_pci_ConnString, dbq, fileName, tableOwner, tableName, ref msj);


            //e. import txt file to database octopus_pci_comp
            msj = "";
            string octopus_pci_comp_ConnString = ConfigurationManager.AppSettings["octopus_pci_comp"];
            saveToDatabaseDirectly(octopus_pci_comp_ConnString, dbq, fileName, tableOwner, tableName, ref msj);


            //f. Run scripts on database octopus_pci
            msj = "";
            string tablaOrigen = tableName; //Trx_Bco_Austro_200129
            string tablaDestino = ConfigurationManager.AppSettings["tableAllComp"]; //Trx_Bco_Austro
            ejecutarScriptSQL(tablaOrigen, tablaDestino, octopus_pci_ConnString, ref msj);


            //f. Run scripts on database octopus_pci_comp
            msj = "";
            ejecutarScriptSQL(tablaOrigen, tablaDestino, octopus_pci_comp_ConnString, ref msj);


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
                copiarArchivo(pathAUSExtraerDst, archivoAUSTPC, ref cr, ref msj, driveVirtualFiles_BRD);
                copiarArchivo(pathAUSExtraerDst, archivoMAUS, ref cr, ref msj, driveVirtualFiles_BRD);
            }

            //j. Luego de colocar los archivos renombrar e archivo:•	AUSTPC180607.TXT a AUSTP180607.TXT  
            renombrarArchivo(driveVirtualFiles_BRD, archivoAUSTPC, archivoAUSTP, ref cr, ref msj);
            //Y la carpeta 1806 se cambia a 1806 v AUSTP
            string nombreOldCarpeta = dtpComp.Value.ToString("yyMM",CultureInfo.InvariantCulture);
            string nombreNewCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture)+ " V AUSTP";
            renombrarCarpeta(driveVirtualFiles_BRD, nombreOldCarpeta, nombreNewCarpeta, ref cr, ref msj);
            //La carpeta 1806 v AUSTPC a 1806
            nombreOldCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture) + " V AUSTPC";
            nombreNewCarpeta = dtpComp.Value.ToString("yyMM", CultureInfo.InvariantCulture);
            renombrarCarpeta(driveVirtualFiles_BRD, nombreOldCarpeta, nombreNewCarpeta, ref cr, ref msj);

            //k. Luego de realizar estos cambios nos ubicamos en el disco D:
            string pathSrvRemotoREPORTES_COMP = ConfigurationManager.AppSettings["pathSrvRemotoREPORTES_COMP"];
            string driveVirtualREPORTES_COMP = ConfigurationManager.AppSettings["driveVirtualREPORTES_COMP"];
            codRetMap = 0;
            conPR = conectarPathRemoto(pathSrvRemotoREPORTES_COMP, driveVirtualREPORTES_COMP, usrRemoto, pwdRemoto, ref codRetMap, ref msj);
            //y cambiamos el nombre a la siguiente carpeta: REPORTES_COMP a REPORTES_COMP v AUSTP 
            string nombreOldCarpetaRepComp = "REPORTES_COMP";
            string nombreNewCarpetaRepComp = "REPORTES_COMP V AUSTP";
            renombrarCarpeta(driveVirtualREPORTES_COMP, nombreOldCarpetaRepComp, nombreNewCarpetaRepComp, ref cr, ref msj);
            //Y  REPORTES_COMP v AUSTPC a REPORTES_COMP
            nombreOldCarpetaRepComp = "REPORTES_COMP V AUSTPC";
            nombreNewCarpetaRepComp = "REPORTES_COMP";
            renombrarCarpeta(driveVirtualREPORTES_COMP, nombreOldCarpetaRepComp, nombreNewCarpetaRepComp, ref cr, ref msj);

            //l. Luego de realizados los cambios nos vamos a C:\Users\ad01000680\Desktop\Compensacion\Compensacion AUSTPC\AS_DCompensa.vbp Y ejecutamos el proyecto AS_DCompensa.vbp


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
            ejecutarComandoPrompt(ref msj);
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

        }

        private void btnPathRemoto_Click(object sender, EventArgs e)
        {
            //conectarPathRemoto();
        }

        private void btnOpenFileSEP_Click(object sender, EventArgs e)
        {
            msj = "";
            string absPathFile = "", fileName = "";
            string filter = "Excel Files (*.xlsx)|*.xlsx";
            abrirArchivo(ref msj, ref absPathFile, ref fileName, filter);
            lblPathArchivo.Text = absPathFile;
        }
    }
}
