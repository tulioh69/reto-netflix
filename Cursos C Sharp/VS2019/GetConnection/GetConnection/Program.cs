using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GetConnection
{
    class Program
    {
        const string path = @"D:\Archivos";
        static void Main(string[] args)
        {
            Connection oConnection = new Connection();
            oConnection.GetData();
            Console.WriteLine(oConnection.Server);
            Console.WriteLine(oConnection.DB);
            
            //USO DE LOGS
            //string path = HttpContext.Current.Request.MapPath("~");
            //Logs oLog = new Logs(path);
            //oLog.Add("Hola mundo");
            //********************
            //USO DE ENCRYPT
            // string cadenaEncriptada = Encrypt.GetSha256("patito");
            //********************
            //numero de hilos disponibles
            //int max, c = 0;
            //ThreadPool.GetMaxThreads(out max, out c);
            //Console.WriteLine(max);
            for (int i=0; i<50; i++)
            {
                ThreadPool.QueueUserWorkItem(Create, i);
            }
            //while (ThreadPool.  > 0) ;
            Console.ReadLine();

        }
        static void Create(object data)
        {
            int i = (int)data;
            using (var sw = new StreamWriter(path+i+"txt"))
            {
                sw.WriteLine("Hola soy el hilo" + Thread.CurrentThread.ManagedThreadId);
            }
            Console.WriteLine("Hola soy el hilo" + Thread.CurrentThread.ManagedThreadId);

        }
    }
}
