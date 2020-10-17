using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CrudWindowsFormAdo
{
    public class PeopleDB
    {
        private string connectionString= @"Data Source=FENIX\SQLEXPRESS2019;Initial Catalog=FundamentosCSharp;User ID=sa;Password=123456789";

        public bool Ok()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public List<People> Get()
        {
            List<People> people = new List<People>();
            string query = "select id, name, age from CrudWindowsForm.dbo.people";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        People oPeople = new People();
                        oPeople.Id = reader.GetInt32(0);
                        oPeople.Name = reader.GetString(1);
                        oPeople.Age = reader.GetInt32(2);
                        people.Add(oPeople);
                    }
                    reader.Close();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    throw new Exception("Hay un error en la bd " + ex.Message);
                }
            }
            return people;
        }
    }
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
