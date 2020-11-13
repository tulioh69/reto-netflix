using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsoleUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string connectionString = @"Data Source=FENIX\SQLEXPRESS2019;Initial Catalog=FundamentosCSharp;User ID=sa;Password=123456789";

            // Provide the query string with a parameter placeholder.
            string queryString = "SELECT * from [Parametros].[Configuration].[DiasFeriados] ";
           // + "WHERE idDiaFeriado > @idDiaFeriado ";
            // Specify the parameter value.
            int paramValue = 4;
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@idDiaFeriado", paramValue);
                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.

                try
                {

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                        reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }
            */
            var currentDirectory = Directory.GetCurrentDirectory();
            var storesDirectory = Path.Combine(currentDirectory,"Archivos");

            var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

            Directory.CreateDirectory(salesTotalDir);

            var salesFiles = FindFiles(storesDirectory);

            var salesTotal = CalculateSalesTotal(salesFiles);

            File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");


            //fin
        }
        static IEnumerable<string> FindFiles(string folderName)
        {
            List<string> salesFiles = new List<string>();

            var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

            foreach (var file in foundFiles)
            {
                var extension = Path.GetExtension(file);

                if (extension==".json")
                {
                    salesFiles.Add(file);
                }
            }

            return salesFiles;
        }
        static double CalculateSalesTotal(IEnumerable<string> salesFiles)
        {
            double salesTotal = 0;

            // Loop over each file path in salesFiles
            foreach (var file in salesFiles)
            {
                // Read the contents of the file
                string salesJson = File.ReadAllText(file);

                // Parse the contents as JSON
                SalesData data = JsonConvert.DeserializeObject<SalesData>(salesJson);

                // Add the amount in found in the Total field to the salesTotal variable
                salesTotal += data.Total;
            }

            return salesTotal;
        }
        class SalesData
        {
            public double Total { get; set; }
        }
    }
    
}
